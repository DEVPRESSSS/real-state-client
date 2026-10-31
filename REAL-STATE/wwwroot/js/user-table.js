(function () {
    var el = document.getElementById('userTable');
    var dataEl = document.getElementById('userData');
    if (!el || !dataEl) return;

    var data = JSON.parse(dataEl.textContent);

    function esc(s) {
        return String(s == null ? '' : s)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;')
            .replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    var badgeActive = 'inline-flex rounded bg-azure px-2 py-0.5 text-xs font-medium text-white';
    var badgeLocked = 'inline-flex items-center gap-1 rounded border border-navy bg-white px-2 py-0.5 text-xs font-medium text-navy';
    var badgeRole = 'inline-flex rounded border border-azure bg-white px-2 py-0.5 text-xs font-medium text-azure';
    var badgeYou = 'inline-flex rounded bg-ice px-2 py-0.5 text-xs font-medium text-navy';
    var dash = '<span class="text-navy/40">&mdash;</span>';

    var table = new Tabulator('#userTable', {
        data: data,
        index: 'id',
        layout: 'fitColumns',
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        placeholder: 'No matching users found.',
        columns: [
            {
                title: 'User', field: 'user', minWidth: 220, widthGrow: 2,
                formatter: function (cell) {
                    var d = cell.getRow().getData();
                    var initial = esc((d.user || '?').trim().charAt(0).toUpperCase());
                    return '<div class="flex items-center gap-3">' +
                        '<div class="flex size-9 shrink-0 items-center justify-center rounded-full bg-ice font-semibold text-azure">' + initial + '</div>' +
                        '<span class="font-semibold text-navy">' + esc(d.user) + '</span>' +
                        (d.isCurrentUser ? '<span class="' + badgeYou + '">You</span>' : '') +
                        '</div>';
                }
            },
            { title: 'Email', field: 'email', minWidth: 200 },
            {
                title: 'Phone', field: 'phone',
                formatter: function (cell) { return cell.getValue() ? esc(cell.getValue()) : dash; }
            },
            {
                title: 'Role', field: 'role',
                formatter: function (cell) {
                    return '<span class="' + badgeRole + '">' + esc(cell.getValue()) + '</span>';
                }
            },
            {
                title: 'Status', field: 'status',
                formatter: function (cell) {
                    var v = cell.getValue();
                    return v === 'Locked'
                        ? '<span class="' + badgeLocked + '"><i data-lucide="lock" class="size-3" aria-hidden="true"></i>Locked</span>'
                        : '<span class="' + badgeActive + '">' + esc(v) + '</span>';
                }
            },
            {
                title: 'Actions', headerSort: false, hozAlign: 'right', minWidth: 210,
                formatter: function (cell) {
                    var d = cell.getRow().getData();

                    var edit = '<a href="' + esc(d.editUrl) + '" class="inline-flex items-center gap-1 bg-azure px-3 py-1.5 text-sm font-medium text-white transition-colors hover:bg-azure-dark active:bg-azure-darker focus:outline-none focus-visible:ring-2 focus-visible:ring-azure focus-visible:ring-offset-2">' +
                        '<i data-lucide="pencil" class="size-4" aria-hidden="true"></i>Edit</a>';

                    var lock = '';
                    if (!d.isCurrentUser) {
                        var btn = 'js-lock inline-flex items-center gap-1 border border-azure px-3 py-1.5 text-sm font-medium text-azure transition-colors hover:bg-azure hover:text-white focus:outline-none focus-visible:ring-2 focus-visible:ring-azure focus-visible:ring-offset-2';
                        lock = d.isLocked
                            ? '<button type="button" class="' + btn + '" data-id="' + esc(d.id) + '" data-title="' + esc(d.user) + '" data-locked="false">' +
                            '<i data-lucide="lock-open" class="size-4" aria-hidden="true"></i>Unlock</button>'
                            : '<button type="button" class="' + btn + '" data-id="' + esc(d.id) + '" data-title="' + esc(d.user) + '" data-locked="true">' +
                            '<i data-lucide="lock" class="size-4" aria-hidden="true"></i>Lock</button>';
                    }

                    return '<div class="flex justify-end gap-2">' + edit + lock + '</div>';
                }
            }
        ]
    });

    var fields = ['user', 'email', 'phone', 'role', 'status'];
    var search = document.getElementById('userSearch');
    var count = document.getElementById('userCount');

    function updateCount() {
        var n = table.getDataCount('active');
        count.textContent = n + (n === 1 ? ' user' : ' users');
    }

    if (search) {
        search.addEventListener('input', function () {
            var term = search.value.trim().toLowerCase();
            if (!term) { table.clearFilter(); return; }
            table.setFilter(function (row) {
                return fields.some(function (f) {
                    return String(row[f] || '').toLowerCase().indexOf(term) !== -1;
                });
            });
        });
    }

    table.on('renderComplete', function () { if (window.lucide) lucide.createIcons(); });
    table.on('tableBuilt', updateCount);
    table.on('dataFiltered', updateCount);
})();