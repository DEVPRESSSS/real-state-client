(function () {
    var el = document.getElementById('agentTable');
    var dataEl = document.getElementById('agentData');
    if (!el || !dataEl) return;

    var data = JSON.parse(dataEl.textContent);

    function esc(s) {
        return String(s == null ? '' : s)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;')
            .replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    function safeUrl(value) {
        if (!value) return '';
        try {
            var url = new URL(value, window.location.origin);
            return (url.protocol === 'http:' || url.protocol === 'https:') ? url.href : '';
        } catch (e) {
            return '';
        }
    }

    var badgeActive = 'inline-flex rounded bg-azure px-2 py-0.5 text-xs font-medium text-white';
    var badgeInactive = 'inline-flex rounded bg-ice px-2 py-0.5 text-xs font-medium text-navy';
    var dash = '<span class="text-navy/40">&mdash;</span>';

    var table = new Tabulator('#agentTable', {
        data: data,
        index: 'id',
        layout: 'fitColumns',
        pagination: false,
        placeholder: 'No matching agents found.',
        columns: [
            {
                title: 'Agent', field: 'agent', minWidth: 220, widthGrow: 2,
                formatter: function (cell) {
                    var d = cell.getRow().getData();
                    var photo = safeUrl(d.photoUrl);
                    var initial = esc((d.agent || '?').trim().charAt(0).toUpperCase());
                    var avatar = photo
                        ? '<img src="' + esc(photo) + '" alt="" class="size-9 shrink-0 rounded-full border border-ice object-cover" />'
                        : '<div class="flex size-9 shrink-0 items-center justify-center rounded-full bg-ice font-semibold text-azure">' + initial + '</div>';
                    return '<div class="flex items-center gap-3">' + avatar +
                        '<span class="font-semibold text-navy">' + esc(d.agent) + '</span></div>';
                }
            },
            { title: 'Email', field: 'email', minWidth: 200 },
            {
                title: 'Phone', field: 'phone',
                formatter: function (cell) { return cell.getValue() ? esc(cell.getValue()) : dash; }
            },
            {
                title: 'License', field: 'license',
                formatter: function (cell) { return cell.getValue() ? esc(cell.getValue()) : dash; }
            },
            {
                title: 'Role', field: 'role',
                formatter: function (cell) {
                    return '<span class="inline-flex rounded border border-azure bg-white px-2 py-0.5 text-xs font-medium text-azure">' + esc(cell.getValue()) + '</span>';
                }
            },
            {
                title: 'Status', field: 'status',
                formatter: function (cell) {
                    var v = cell.getValue();
                    return '<span class="' + (v === 'Active' ? badgeActive : badgeInactive) + '">' + esc(v) + '</span>';
                }
            },
            {
                title: 'Actions', headerSort: false, hozAlign: 'right', minWidth: 110,
                formatter: function (cell) {
                    var d = cell.getRow().getData();
                    return '<div class="flex justify-end gap-2">' +
                        '<a href="' + esc(d.editUrl) + '" class="inline-flex items-center gap-1 bg-azure px-3 py-1.5 text-sm font-medium text-white transition-colors hover:bg-azure-dark active:bg-azure-darker focus:outline-none focus-visible:ring-2 focus-visible:ring-azure focus-visible:ring-offset-2">' +
                        '<i data-lucide="pencil" class="size-4" aria-hidden="true"></i>Edit</a>' +
                        '</div>';
                }
            }
        ]
    });

    var fields = ['agent', 'email', 'phone', 'license', 'role', 'status'];
    var search = document.getElementById('agentSearch');
    var count = document.getElementById('agentCount');

    function updateCount() {
        var n = table.getDataCount('active');
        count.textContent = n + (n === 1 ? ' agent' : ' agents');
    }

    if (search) {
        search.addEventListener('input', tableUtils.debounce(function () {
            var term = search.value.trim().toLowerCase();
            if (!term) { table.clearFilter(); return; }
            table.setFilter(function (row) {
                return fields.some(function (f) {
                    return String(row[f] || '').toLowerCase().indexOf(term) !== -1;
                });
            });
        }, 200));
    }

    table.on('renderComplete', function () { if (window.lucide) tableUtils.icons(el); });
    table.on('tableBuilt', updateCount);
    table.on('dataFiltered', updateCount);
})();