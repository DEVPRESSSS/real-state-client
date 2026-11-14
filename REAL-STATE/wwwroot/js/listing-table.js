(function () {
    var el = document.getElementById('listingTable');
    var dataEl = document.getElementById('listingData');
    if (!el || !dataEl) return;

    var data = JSON.parse(dataEl.textContent);

    function esc(s) {
        return String(s == null ? '' : s)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;')
            .replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    var badge = 'inline-flex rounded bg-ice px-2 py-0.5 text-xs font-medium text-navy';
    var badgeOutline = 'inline-flex rounded border border-azure bg-white px-2 py-0.5 text-xs font-medium text-azure';
    var badgeAvailable = 'inline-flex rounded bg-azure px-2 py-0.5 text-xs font-medium text-white';

    var table = new Tabulator('#listingTable', {
        data: data,
        index: 'id',
        layout: 'fitColumns',
        pagination: false,
        placeholder: 'No matching listings found.',
        columns: [
            {
                title: 'Title', field: 'title', minWidth: 220, widthGrow: 2,
                formatter: function (cell) {
                    return '<span class="font-semibold text-navy">' + esc(cell.getValue()) + '</span>';
                }
            },
            { title: 'Type', field: 'propertyType' },
            {
                title: 'Listing', field: 'listingType',
                formatter: function (cell) {
                    var v = cell.getValue();
                    return '<span class="' + (v === 'Rent' ? badgeOutline : badge) + '">' + esc(v) + '</span>';
                }
            },
            {
                title: 'Price', field: 'price', sorter: 'number', hozAlign: 'right',
                formatter: function (cell) {
                    return '₱' + Number(cell.getValue()).toLocaleString('en-PH', {
                        minimumFractionDigits: 2, maximumFractionDigits: 2
                    });
                }
            },
            { title: 'City', field: 'city' },
            { title: 'Developer', field: 'developer' },
            {
                title: 'Status', field: 'status',
                formatter: function (cell) {
                    var v = cell.getValue();
                    return '<span class="' + (v === 'Available' ? badgeAvailable : badge) + '">' + esc(v) + '</span>';
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
                        'data-id="' + esc(d.id) + '" data-title="' + esc(d.title) + '">' +
                        '<i data-lucide="trash-2" class="size-4" aria-hidden="true"></i>Delete</button>' +
                        '</div>';
                }
            }
        ]
    });

    var fields = ['title', 'propertyType', 'listingType', 'city', 'developer', 'status'];
    var search = document.getElementById('listingSearch');
    var count = document.getElementById('listingCount');

    function updateCount() {
        var n = table.getDataCount('active');
        count.textContent = n + (n === 1 ? ' listing' : ' listings');
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

    document.addEventListener('listing:deleted', function (e) {
        table.deleteRow(e.detail.id).then(updateCount).catch(function () { });
    });
})();