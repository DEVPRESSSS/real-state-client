# Backend and frontend hardening

The existing P0 administrator bootstrap identity and password are intentionally retained. They remain a known security risk. No production database or deployed account was changed by this work.

## Development

Use .NET 10 and Node.js. Restore packages, initialize the local SQL Server database explicitly, then start the HTTPS profile:

```powershell
dotnet restore REAL-STATE.slnx
dotnet run --project REAL-STATE --launch-profile https -- --initialize-database
dotnet run --project REAL-STATE --launch-profile https
```

Normal application startup no longer migrates or seeds databases. Initialization errors stop the initialization command with a nonzero exit code. Repeated initialization retains the existing bootstrap account.

Development settings use the existing local SQLEXPRESS database and a local SMTP catcher at localhost:1025 without TLS. Start a mail catcher separately, or override the SMTP settings. Confirmation links use https://localhost:7005, matching the HTTPS launch profile. New accounts cannot sign in until their email is confirmed; new agents also require administrator activation. The login page provides a resend-confirmation link if delivery fails after account creation.

## Production configuration

Supply the following using environment variables or the hosting secret store; do not put credentials in source files:

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | SQL Server connection with `Encrypt=True;TrustServerCertificate=False` and a certificate-valid server name |
| `AllowedHosts` | Deployment host names, separated by semicolons; unrestricted `*` is rejected |
| `PublicSite__BaseUrl` | Public HTTPS base URL, without a query, fragment, or embedded credentials |
| `Smtp__Host`, `Smtp__Port` | SMTP submission host and STARTTLS port, normally 587 |
| `Smtp__From` | Valid sender address |
| `Smtp__UserName`, `Smtp__Password` | SMTP credentials when required |
| `Smtp__EnableSsl` | `true` outside Development |

Use the initialization command above with the production environment and a deployment identity allowed to migrate the database. Run the web application with a separate account that does not need schema-changing permissions. Check `/health/ready` before routing traffic: it returns 503 if the database is unavailable, migrations are pending, or required roles are absent. Invalid required production configuration prevents startup.

Terminate or forward HTTPS correctly at the hosting boundary. If a reverse proxy is used, configure trusted proxies and forwarded headers for that deployment; do not trust arbitrary forwarded headers. Rate limits currently use the connection IP and in-process counters: 10 sensitive Identity POSTs per minute, and 10 public registration/appointment submissions per minute per IP. At multiple instances, add a shared edge limiter. Users behind a shared proxy or NAT share that IP budget.

Account locking, password changes, and role changes update the Identity security stamp. Existing cookies are revalidated within one minute. Active-agent authorization is additionally checked against the database on restricted agent endpoints, and appointment approval verifies assignment and active status in the atomic update. Previously stored `EmailConfirmed` values are preserved: review and reverify legacy accounts that were automatically marked confirmed before this change.

Machine-specific publish credentials were removed, password saving disabled, and `*.user` files ignored. Review any historical deployment-credential exposure and rotate those credentials through the hosting provider when appropriate. Git history has not been rewritten.

## Data access and frontend behavior

- Management lists search all records through the GET search form and return a database page (default 20, maximum 100). The separate table filter and sorting affect only that page. Shared pagination preserves the query string.
- Public properties, developers, and agents use database pagination. Property type is an exact category match; city search uses a prefix and the database's configured collation. Property search sorts featured listings first, then creation date and ID for stable paging.
- Dashboards aggregate counts in SQL and fetch six recent properties. Public directory cards project listing counts. User lists project roles in the paged SQL query rather than loading roles once per user.
- Read-only repository calls honor the tracking flag. Listing forms bind validated scalar inputs; related records and generated IDs are created by the server. Appointment approval uses one conditional update, so only one request can transition a pending appointment.
- SweetAlert2 and Lucide are pinned and served locally. Tabulator is loaded once per relevant page. Searches are debounced, and table icon initialization is scoped to the changed area.
- Responses carry a nonce-based script CSP, frame restrictions, MIME-sniffing protection, and a referrer policy. Inline styles remain allowed for Tabulator and existing layout styles. HTTP/HTTPS URL allowlists are checked on stored developer links and image input; legacy public developer links are checked again before rendering.

## Verification

```powershell
dotnet test REAL-STATE.slnx --no-restore
node --test REAL-STATE.Tests/frontend.test.cjs
npm.cmd audit
dotnet list REAL-STATE/REAL-STATE.csproj package --vulnerable --include-transitive --no-restore
```

The backend suite uses isolated in-memory SQLite databases for relational transactions, scoped pagination, conditional approvals, account rollback, and security-stamp changes. ASP.NET test-host requests cover crafted listing payloads, inactive agents, cross-agent approvals, anti-forgery enforcement, public confirmation pages, nonce rendering, and throttling. No tests connect to the application's SQL Server database or send email.

Before production rollout, smoke-test real SMTP delivery, confirmation/resend and password reset, administrative account changes, HTTPS/proxy behavior, and the SQL Server migration command in staging. SQLite tests do not replace SQL Server query-plan or concurrency/load testing. Measure query counts, response sizes, allocations and p95 latency using representative listing and user volumes before changing the existing indexes or adding caching.
