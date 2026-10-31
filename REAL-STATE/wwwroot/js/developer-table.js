(function () {
    var el = document.getElementById('developerTable');
    var dataEl = document.getElementById('developerData');
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
            var url = new URL(value);
            return (url.protocol === 'http:' || url.protocol === 'https:') ? url.href : '';
        } catch (e) {
            return '';
        }
    }

    var badgeActive = 'inline-flex rounded bg-azure px-2 py-0.5 text-xs font-medium text-white';
    var badgeInactive = 'inline-flex rounded bg-ice px-2 py-0.5 text-xs font-medium text-navy';

    var table = new Tabulator('#developerTable', {
        data: data,
        index: 'id',
        layout: 'fitColumns',
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        placeholder: 'No matching developers found.',
        columns: [
            {
                title: 'Developer', field: 'developer', minWidth: 220, widthGrow: 2,
                formatter: function (cell) {
                    var d = cell.getRow().getData();
                    var logo = safeUrl(d.logoUrl);
                    var avatar = logo
                        ? '<img src="' + esc(logo) + '" alt="" class="size-9 shrink-0 rounded border border-ice bg-white object-contain p-1" />'
                        : '<div class="flex size-9 shrink-0 items-center justify-center rounded bg-ice font-semibold text-azure">' +
                        esc((d.developer || '?').charAt(0).toUpperCase()) + '</div>';
                    return '<div class="flex items-center gap-3">' + avatar +
                        '<span class="font-semibold text-navy">' + esc(d.developer) + '</span></div>';
                }
            },
            {
                title: 'Slug', field: 'slug', minWidth: 150,
                formatter: function (cell) {
                    return '<span class="text-navy/70">' + esc(cell.getValue()) + '</span>';
                }
            },
            {
                title: 'Website', field: 'website', minWidth: 180,
                formatter: function (cell) {
                    var url = safeUrl(cell.getValue());
                    if (!url) return '<span class="text-navy/40">&mdash;</span>';
                    return '<a href="' + esc(url) + '" target="_blank" rel="noopener noreferrer" ' +
                        'class="text-azure hover:text-azure-dark hover:underline focus:outline-none focus-visible:ring-2 focus-visible:ring-azure">' +
                        esc(cell.getValue()) + '</a>';
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
                title: 'Created', field: 'created',
                sorter: function (a, b, aRow, bRow) {
                    return aRow.getData().createdTicks - bRow.getData().createdTicks;
                }
            },
            {
                title: 'Actions', headerSort: false, hozAlign: 'right', minWidth: 190,
                formatter: function (cell) {
                    var d = cell.getRow().getData();
                    return '<div class="flex justify-end gap-2">' +
                        '<a href="' + esc(d.editUrl) + '" class="inline-flex items-center gap-1 bg-azure px-3 py-1.5 text-sm font-medium text-white transition-colors hover:bg-azure-dark active:bg-azure-darker focus:outline-none focus-visible:ring-2 focus-visible:ring-azure focus-visible:ring-offset-2">' +
                        '<i data-lucide="pencil" class="size-4" aria-hidden="true"></i>Edit</a>' +
                        '<button type="button" class="js-delete inline-flex items-center gap-1 border border-red-700 px-3 py-1.5 text-sm font-medium text-red-700 transition-colors hover:bg-red-700 hover:text-white focus:outline-none focus-visible:ring-2 focus-visible:ring-red-700 focus-visible:ring-offset-2" ' +
                        'data-id="' + esc(d.id) + '" data-title="' + esc(d.developer) + '">' +
                        '<i data-lucide="trash-2" class="size-4" aria-hidden="true"></i>Delete</button>' +
                        '</div>';
                }
            }
        ]
    });

    var fields = ['developer', 'slug', 'website', 'status', 'created'];
    var search = document.getElementById('developerSearch');
    var count = document.getElementById('developerCount');

    function updateCount() {
        var n = table.getDataCount('active');
        count.textContent = n + (n === 1 ? ' developer' : ' developers');
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

    document.addEventListener('developer:deleted', function (e) {
        table.deleteRow(e.detail.id).then(updateCount).catch(function () { });
    });
})();