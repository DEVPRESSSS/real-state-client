# Real Estate Platform UI Design Skill

## Purpose

Improve the UI/UX of the existing Real Estate Platform using **ASP.NET Core MVC, Razor Views, Bootstrap, and the project's existing CSS or JavaScript libraries**.

The application is a property-listing platform that brings together properties from different developers, agents, and brokers.

The platform may include:

- Property listings from different developers
- Property search and filtering
- Property details and image galleries
- Agent and broker profiles
- Contact requests and property inquiries
- Property viewing appointments
- User accounts and authentication
- Saved properties and bookmarks
- Agent referral functionality, if implemented

The existing application is the source of truth.

Your job is to **refine and improve the existing interface, not redesign the application's architecture**.

---

## 1. Preserve the Existing Project

Before making changes:

- Inspect the existing file structure.
- Inspect the relevant controllers, Razor Views, partial views, layouts, and ViewModels.
- Inspect existing Bootstrap classes, CSS, JavaScript, and component patterns.
- Understand how the shared layout and navigation work.
- Understand existing routing and controller actions.
- Understand how forms submit data and display validation errors.
- Understand how authentication and authorization are implemented.
- Identify existing reusable partial views and components.
- Check how property images and listing information are rendered.
- Understand how the frontend communicates with the existing backend.

Do NOT reorganize the project unless explicitly requested.

Do NOT rename or move files unnecessarily.

Do NOT introduce a new architecture.

Do NOT replace existing libraries when they already solve the problem.

Do NOT convert the application to React or another frontend framework unless explicitly requested.

Do NOT change the existing ASP.NET Core MVC architecture just to improve the UI.

---

## 2. Preserve Functionality

UI changes must not break:

- Authentication and authorization
- Cookie-based authentication
- ASP.NET Core Identity
- Existing MVC routing
- Controller actions
- Model binding
- ViewModels
- Form submissions
- Server-side validation
- Client-side validation
- Database operations
- Entity Framework Core relationships
- Property search and filtering
- Property details
- Developer listings
- Agent and broker profiles
- Bookmarks and saved properties
- Property inquiries
- Viewing appointment scheduling
- Existing referral functionality, if implemented
- Image uploads and property image display
- Anti-forgery token protection
- Existing pagination and sorting

Do not modify backend code for a UI-only task.

Do not change database schemas, entity relationships, controller routes, or business logic unless the requested feature genuinely requires it.

If a UI improvement appears to require a functional change, explain why before making that change.

Never remove form fields, validation rules, or authorization checks simply to simplify the interface.

---

## 3. Improve, Don't Replace

The existing design is the starting point.

When improving a component, first understand what already exists.

Then improve:

- Spacing
- Typography
- Alignment
- Visual hierarchy
- Colors
- Borders
- Button consistency
- Property image presentation
- Responsive behavior
- Hover and focus states
- Form usability
- Loading states
- Empty states
- Error states
- Validation feedback

Avoid completely replacing a component unless the existing implementation genuinely prevents the requested improvement.

Prefer incremental changes that fit the current project.

Do not redesign the entire website when the request only concerns one page or component.

---

## 4. Keep the Design Simple and Professional

The Real Estate Platform should feel:

- Clean
- Modern
- Professional
- Trustworthy
- Easy to navigate
- Visually consistent
- Suitable for real property buyers and renters
- Appropriate for developers, agents, and brokers

The interface should emphasize property information and make it easy for users to find suitable listings.

Do not automatically add:

- Excessive gradients
- Glassmorphism
- Glowing effects
- Huge shadows
- Excessively rounded cards
- Decorative background blobs
- Unnecessary animations
- Excessive icons
- Unnecessary colors
- Distracting decorative elements
- Fake statistics or promotional claims

Simple does not mean unfinished.

Aim for **simple, polished, trustworthy, and conversion-focused**.

Use whitespace, alignment, consistent typography, and clear visual hierarchy to make the interface feel professional.

---

## 5. Respect the Existing Visual Style

If the existing application already has:

- A color palette
- Typography
- Border radius
- Spacing conventions
- Button styles
- Form styles
- Navbar styling
- Card styling
- Existing CSS variables
- Bootstrap theme overrides

Continue using them.

Do not introduce a completely different visual language.

If something needs improvement, make the smallest change necessary.

If the project already uses a defined brand identity, preserve it unless a redesign is explicitly requested.

For a real-estate platform, favor a restrained palette, readable typography, strong property photography, and clear calls to action.

Do not change the entire color scheme simply because another palette looks more modern.

---

## 6. Component Reuse

Before creating a new component:

1. Check whether an existing partial view can be reused.
2. Check whether an existing layout or component can be extended.
3. Check whether an existing ViewModel already provides the required data.
4. Reuse existing CSS classes and JavaScript behavior where appropriate.
5. Create a new partial view only when it provides a clear benefit.

Potential reusable UI components include:

- Property cards
- Property image galleries
- Search and filter forms
- Price displays
- Property feature lists
- Developer information sections
- Agent profile cards
- Inquiry forms
- Viewing appointment forms
- Bookmark buttons
- Pagination controls
- Breadcrumbs
- Empty-state messages
- Form validation summaries

Do not create duplicate partial views with slightly different names.

Do not introduce unnecessary abstractions for simple UI elements.

Keep business logic out of Razor Views whenever possible. Use the existing controller and ViewModel patterns.

---

## 7. Bootstrap, Razor, CSS, and JavaScript

Use the conventions already established by the project.

If the application uses Bootstrap, prefer existing Bootstrap utilities and components.

Examples:

```html
<div class="d-flex align-items-center gap-3 rounded border p-3">
    <div class="flex-grow-1">
        <h5 class="mb-1">Property title</h5>
        <p class="mb-0 text-muted">Property location</p>
    </div>
</div>
```

Use Bootstrap's existing grid and responsive utilities where appropriate:

```text
container
row
col-12
col-md-6
col-lg-4
d-flex
align-items-center
justify-content-between
gap-*
mb-*
p-*
```

Use readable Razor markup.

Avoid unnecessarily long and difficult-to-maintain HTML or CSS.

Do not introduce Tailwind CSS if the project already relies on Bootstrap, unless explicitly requested.

Do not introduce custom CSS when Bootstrap already provides a suitable utility.

However, use existing CSS when it is the appropriate place for custom styling.

Do not add inline styles everywhere. Prefer existing stylesheets and reusable classes for repeated design rules.

Do not install new frontend libraries unnecessarily.

Preserve existing JavaScript behavior and dependencies.

---

## 8. Responsive Design

Improve the existing interface for:

- Mobile
- Tablet
- Laptop
- Desktop

Do not redesign the desktop interface simply to make it responsive.

Use Bootstrap's responsive grid and breakpoint utilities where appropriate.

Pay particular attention to:

- Navigation and mobile menus
- Property search forms
- Filter controls
- Property card grids
- Property image galleries
- Property detail layouts
- Price and property information
- Agent contact information
- Inquiry forms
- Appointment scheduling
- Bookmark buttons
- Pagination
- Long property titles
- Long addresses
- Large property images
- Footer layout

Ensure that property cards remain readable on narrow screens.

Prevent long text, images, and buttons from overflowing their containers.

Property images should maintain an appropriate aspect ratio and should not appear stretched.

Filters should remain usable on mobile. If the existing design supports a collapsible filter panel, improve that behavior instead of introducing a completely different navigation pattern.

Buttons and form controls should remain easy to use on touchscreens.

---

## 9. Property Search and Filtering UX

Property discovery is one of the platform's primary functions.

Users should easily understand how to find relevant properties.

Preserve and improve existing filters for:

- Location
- Property type
- Minimum price
- Maximum price
- Floor area in square feet
- Listing type, such as sale or rent
- Other filters already supported by the application

When improving search and filters:

- Make field labels clear.
- Use appropriate input types.
- Keep related filters visually grouped.
- Make the search action easy to identify.
- Provide a clear way to reset filters when supported.
- Preserve selected filter values after submission.
- Preserve existing query-string parameters and routing behavior.
- Display the active search criteria when appropriate.
- Ensure the form works on mobile.
- Avoid overwhelming users with too many controls at once.

Do not add filters that the backend does not support.

Do not make a filter appear functional if it does not affect the search results.

Do not invent result counts.

If no properties match the search, display a helpful empty state and suggest adjusting the filters.

Example:

```text
No properties found

Try changing your location, price range, or property type.
```

---

## 10. Property Listing Cards

Property cards should make it easy to compare listings at a glance.

Prioritize:

- Property image
- Property title
- Location
- Price
- Property type
- Listing type
- Floor area
- Bedrooms and bathrooms, when available
- Developer or agent information, when available
- Bookmark action, if supported
- A clear link to view the property details

Keep property cards visually consistent.

Use a consistent image ratio, spacing, title hierarchy, and price treatment.

Ensure long titles and addresses do not break the layout.

Do not display empty labels, meaningless placeholders, or incorrect default values for missing data.

Do not show bedrooms or bathrooms when the property has no corresponding information.

Do not invent property badges such as "Hot Deal", "Verified", "Exclusive", or "Featured" unless they are supported by actual application data and business rules.

If the application supports featured properties, display that status using the existing data.

Avoid making every property card visually dominant. The image, price, and title should receive the most attention.

---

## 11. Property Details Page

The property details page should help users understand the listing and decide what to do next.

Prioritize:

- Property title
- Location
- Price
- Property image gallery
- Description
- Property specifications
- Floor area
- Bedrooms and bathrooms, when available
- Developer information
- Agent or broker information
- Inquiry or contact action
- Viewing appointment action, if supported
- Bookmark action, if supported

Organize the information into clear sections.

Make the primary property image prominent without allowing it to overwhelm the entire page.

Ensure image galleries are responsive and that images maintain their correct proportions.

Use readable line lengths for descriptions.

Keep the price and primary actions easy to find.

If the page contains a long description or many specifications, use a clear information hierarchy rather than placing everything into one large block.

Do not invent floor plans, maps, amenities, property features, or gallery images.

Do not display controls that suggest functionality that does not exist.

Preserve all existing routes and actions for inquiries, bookmarks, and appointments.

---

## 12. Developer Listings

Developers should be presented consistently and professionally.

When the required data exists, developer listings may include:

- Developer logo
- Developer name
- Description
- Website link
- Associated properties
- Listing status, when relevant

Improve readability, alignment, and spacing without inventing company details.

Ensure developer names and logos are displayed correctly on different screen sizes.

If a developer has no logo or description, handle the missing information gracefully.

Do not create fake developer profiles or property counts.

Preserve existing navigation from a developer profile to its associated properties.

---

## 13. Agent and Broker Profiles

Agent and broker profiles should make contact information easy to find and understand.

When available, display:

- Profile photo
- Full name
- Role
- Biography
- Contact information
- License information, if appropriate for the application
- Associated properties
- Inquiry or contact action

Use consistent profile layouts and typography.

Keep contact actions clear without overwhelming the profile.

Do not invent professional qualifications, licenses, experience, ratings, testimonials, or verification status.

Do not expose personal information that the application is not intended to make public.

Preserve the existing authorization and privacy rules.

If profile information is missing, omit the relevant field or display an appropriate neutral fallback.

---

## 14. Inquiry and Contact Forms

Inquiry forms should be easy to understand and complete.

Ensure:

- Labels clearly describe each field.
- Required fields are identifiable.
- Input types are appropriate.
- Validation messages are understandable.
- Validation errors appear near the relevant fields when supported.
- Server-side validation continues to work.
- Existing client-side validation continues to work.
- Submission buttons have clear labels.
- Disabled states are visible when applicable.
- Success and error feedback is clear.
- The form works on mobile.

Preserve the existing field names, model binding, validation rules, and controller actions.

Do not remove fields simply to make the form look cleaner.

Do not display raw backend exceptions to users.

Example of a helpful success message:

```text
Your inquiry has been submitted.

The appropriate contact person can follow up using the details you provided.
```

Only display success messages when the application confirms that the operation succeeded.

Do not claim that an agent has been notified unless the existing application actually supports that behavior.

---

## 15. Viewing Appointment Scheduling

If property viewing appointments are implemented, make the scheduling process clear and straightforward.

Prioritize:

- Property being viewed
- Contact details
- Requested date
- Requested time
- Additional notes, if supported
- Submission action
- Appointment status, when appropriate

Use suitable form controls and clear labels.

Preserve existing date validation, availability rules, and scheduling logic.

Do not allow the UI to imply that an appointment is confirmed if the application only creates a pending request.

Differentiate between:

- Pending
- Confirmed
- Cancelled
- Completed

Only show statuses that the application actually supports.

Do not invent available appointment slots or automatically mark requests as confirmed.

If an appointment cannot be submitted, explain the issue in understandable language and preserve the user's entered information when possible.

---

## 16. Authentication, Accounts, and Bookmarks

Improve the account-related interface without changing authentication behavior.

Preserve:

- Existing login and logout flows
- Registration
- Password validation
- Cookie-based authentication
- Authorization checks
- ASP.NET Core Identity integration
- Existing redirects
- Bookmark persistence

Bookmark controls should clearly communicate their current state using the application's actual data.

Do not create fake saved properties or bookmark counts.

For unauthenticated users, preserve the existing behavior when they attempt to save a property.

Do not bypass authorization or modify server-side checks for visual convenience.

Ensure that icon-only controls have accessible labels and visible focus states.

When improving account pages, keep forms consistent with the rest of the application.

---

## 17. Loading, Empty, Success, and Error States

Do not treat all states as the same.

### Loading

If the existing application supports asynchronous loading, show an appropriate loading indicator or skeleton.

Do not introduce unnecessary loading animations into ordinary server-rendered pages.

### Empty

Explain what the user can do next.

Examples:

```text
No properties available

Try exploring another location or property type.
```

```text
No saved properties yet

Bookmark properties you like to find them here.
```

Only show the second message when the user is viewing the saved-properties page and the application confirms that the list is empty.

### Success

Show clear feedback after a successful operation, such as submitting an inquiry or saving a property.

Only show success feedback when the operation has actually succeeded.

### Error

Explain the issue in user-friendly language and provide a useful action when possible.

Example:

```text
Unable to load the properties.

Please try again.
```

Do not expose stack traces, database errors, raw exception messages, or sensitive technical information in the UI.

Preserve the application's existing logging and error-handling behavior.

---

## 18. Accessibility

Use semantic HTML and accessible form controls.

Ensure:

- Navigation uses appropriate semantic elements.
- Form fields have associated labels.
- Buttons and links have clear purposes.
- Icon-only buttons have accessible names.
- Keyboard navigation works.
- Focus states remain visible.
- Text has sufficient contrast.
- Important information is not communicated through color alone.
- Images have appropriate alternative text when meaningful.
- Validation errors are understandable.
- Interactive controls are usable on touchscreens.

Do not use clickable `<div>` elements when a semantic button or link is appropriate.

Do not remove focus outlines without providing a visible alternative.

Use the existing icon library if one is already installed.

Do not install another icon library unnecessarily.

Do not use icons simply for decoration.

---

## 19. Images and Property Photography

Property imagery is important to the user experience.

When improving image presentation:

- Preserve the existing image source and data flow.
- Maintain appropriate aspect ratios.
- Prevent images from stretching.
- Use consistent image dimensions within listing cards.
- Ensure responsive behavior.
- Provide useful alternative text when appropriate.
- Handle missing or broken images gracefully.
- Avoid layout shifts where practical.

Use only property images provided by the application or explicitly supplied for development.

Do not invent real property photographs or associate unrelated images with actual listings.

Do not replace real property images with random stock images.

If placeholder images are explicitly requested for a demo, clearly treat them as placeholders rather than real listing data.

---

## 20. Typography and Visual Hierarchy

Typography should help users scan property information quickly.

Use the existing fonts if the project already has an established typography system.

If typography needs improvement, use a consistent hierarchy for:

- Main page headings
- Section headings
- Property titles
- Property prices
- Body text
- Secondary information
- Form labels
- Buttons
- Navigation
- Metadata

Property prices should be easy to locate and compare.

Property titles should remain readable without overpowering the rest of the card.

Secondary information, such as floor area and location, should be visually distinct but not distracting.

Avoid using too many font families, font sizes, or font weights.

Do not introduce an external font dependency without a clear benefit.

---

## 21. Animation and Interaction

Use subtle transitions only when they improve usability.

Appropriate examples include:

- Button hover transitions
- Link color transitions
- Bookmark state transitions, if supported
- Focus transitions
- Dropdown interactions
- Existing responsive navigation behavior

Avoid excessive:

- Bouncing
- Scaling
- Floating
- Glowing
- Sliding
- Parallax
- Decorative motion

Do not add animation that delays navigation or interferes with form submission.

Respect reduced-motion preferences when implementing custom animations.

The interface should feel responsive, not animated for the sake of animation.

---

## 22. Do Not Invent Data or Functionality

Do not invent:

- Properties
- Property prices
- Property locations
- Developers
- Agents or brokers
- Contact information
- Property images
- Amenities
- Property availability
- Agent ratings
- Testimonials
- Verification badges
- Viewing appointment slots
- Saved properties
- Inquiry statuses
- Referral rewards
- Statistics
- Search result counts

Use the application's existing data.

If mock data is explicitly requested, clearly separate it from real application data.

Do not create visual controls for features that do not exist.

Do not imply that a feature works merely because its button looks interactive.

If a requested UI improvement depends on missing backend functionality, explain the limitation before implementing the functional change.

---

## 23. Do Not Overengineer

For a simple UI request, do not:

- Create unnecessary abstractions.
- Introduce a new frontend framework.
- Create unnecessary ViewModels or services.
- Add unnecessary JavaScript.
- Install unnecessary dependencies.
- Rewrite existing controllers.
- Change database relationships.
- Rewrite authentication.
- Reorganize folders.
- Replace Bootstrap without a clear reason.
- Rewrite the entire CSS architecture.
- Add unnecessary custom CSS.
- Duplicate existing partial views.
- Change working functionality to match a visual preference.

Make the smallest clean change that solves the problem.

Prefer readable, maintainable Razor markup and consistent Bootstrap classes.

---

## 24. Before Editing

Always follow this process:

```text
1. Inspect the relevant files.
2. Understand the existing implementation.
3. Identify the specific UI problem.
4. Identify reusable views, styles, and components.
5. Make the smallest appropriate change.
6. Preserve existing functionality and data flow.
7. Check responsive behavior.
8. Check accessibility and validation states.
9. Check visual consistency with the rest of the application.
10. Review the changed files for unintended side effects.
11. Run the available build or tests when practical.
12. Report what changed and identify any remaining limitations.
```

Do not claim that the application builds or works correctly unless it has been verified.

If the project files are not available, ask for the relevant files rather than pretending to have inspected the project.

When changing a view, consider whether its shared layout, controller, ViewModel, or stylesheet also needs a small adjustment.

Avoid unrelated changes.

---

## 25. Final Rule

The existing application is the source of truth.

Improve what exists instead of replacing it with an assumed better design.

The goal is:

> **Simple → Clean → Consistent → Responsive → Trustworthy → Polished**

Not:

> Simple → Completely redesigned → Overengineered

Every change should improve the property discovery experience while preserving the existing ASP.NET Core MVC architecture and working functionality.

**Prioritize clarity, usability, maintainability, and a professional real-estate experience over visual trends.**