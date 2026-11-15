
(function () {
    var el = document.getElementById('appointmentTable');
    var dataEl = document.getElementById('appointmentData');

    if (!el || !dataEl) return;

    var data = JSON.parse(dataEl.textContent);

    function esc(s) {
        return String(s == null ? '' : s)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    var badgePending =
        'inline-flex rounded bg-amber-500 px-2 py-0.5 text-xs font-medium text-white';

    var badgeConfirmed =
        'inline-flex rounded bg-green-700 px-2 py-0.5 text-xs font-medium text-white';

    var badgeCompleted =
        'inline-flex rounded border border-azure bg-white px-2 py-0.5 text-xs font-medium text-azure';

    var badgeCancelled =
        'inline-flex rounded bg-red-100 px-2 py-0.5 text-xs font-medium text-red-700';

    var dash = '<span class="text-navy/40">&mdash;</span>';

    function getStatusBadge(status) {
        var value = String(status || '').toLowerCase();

        var badgeClass = badgePending;

        switch (value) {
            case 'approved':
                badgeClass = badgeConfirmed;
                break;

            case 'completed':
                badgeClass = badgeCompleted;
                break;

            case 'cancelled':
                badgeClass = badgeCancelled;
                break;

            case 'pending':
            default:
                badgeClass = badgePending;
                break;
        }

        return '<span class="' + badgeClass + '">' +
            esc(status || 'Pending') +
            '</span>';
    }

    var table = new Tabulator('#appointmentTable', {
        data: data,
        index: 'id',
        layout: 'fitColumns',

        // Pagination is handled by the server/service.
        pagination: false,

        placeholder: 'No matching appointments found.',

        columns: [
            {
                title: 'Property',
                field: 'property',
                minWidth: 200,
                widthGrow: 2,
                formatter: function (cell) {
                    return '<span class="font-semibold text-navy">' +
                        esc(cell.getValue()) +
                        '</span>';
                }
            },
            {
                title: 'Customer',
                field: 'contactName',
                minWidth: 170,
                formatter: function (cell) {
                    return '<span class="font-medium text-navy">' +
                        esc(cell.getValue()) +
                        '</span>';
                }
            },
            {
                title: 'Email',
                field: 'email',
                minWidth: 200,
                formatter: function (cell) {
                    var value = cell.getValue();

                    return value
                        ? '<span class="text-navy/70">' + esc(value) + '</span>'
                        : dash;
                }
            },
            {
                title: 'Phone',
                field: 'phone',
                minWidth: 130,
                formatter: function (cell) {
                    return cell.getValue()
                        ? esc(cell.getValue())
                        : dash;
                }
            },
            {
                title: 'Agent',
                field: 'agent',
                minWidth: 150,
                formatter: function (cell) {
                    var value = cell.getValue();

                    return value && value !== 'Unassigned'
                        ? '<span class="text-white bg-azure px-1 rounded-sm">' + esc(value) + '</span>'
                        : '<span class="bg-ice text-navy px-1 rounded-sm">Unassigned</span>';
                }
            },
            {
                title: 'Scheduled Date',
                field: 'scheduledTicks',
                minWidth: 180,
                sorter: 'number',
                formatter: function (cell) {
                    var d = cell.getRow().getData();

                    return '<div class="flex items-center gap-2">' +
                        '<i data-lucide="calendar-clock" class="size-4 shrink-0 text-azure" aria-hidden="true"></i>' +
                        '<span class="text-sm text-navy">' +
                        esc(d.scheduledAt) +
                        '</span></div>';
                }
            },
            {
                title: 'Status',
                field: 'status',
                minWidth: 120,
                formatter: function (cell) {
                    return getStatusBadge(cell.getValue());
                }
            },
            {
                title: 'Created',
                field: 'createdTicks',
                minWidth: 140,
                sorter: 'number',
                formatter: function (cell) {
                    return '<span class="text-sm text-navy/60">' +
                        esc(cell.getRow().getData().createdAt) +
                        '</span>';
                }
            },
            {
                title: 'Actions',
                headerSort: false,
                hozAlign: 'right',
                minWidth: 100,
                formatter: function (cell) {
                    var d = cell.getRow().getData();

                    return '<div class="flex justify-end">' +
                        '<button type="button"' +
                        ' class="appointment-view inline-flex items-center gap-1 rounded bg-azure px-3 py-1.5 text-sm font-medium text-white transition hover:bg-azure-dark"' +
                        ' data-id="' + esc(d.id) + '">' +
                        '<i data-lucide="eye" class="size-4" aria-hidden="true"></i>' +
                        'View</button>' +
                        '</div>';
                }
            }
        ]
    });
    // Appointment Details Modal
    var modal = document.getElementById('appointmentModal');
    var closeModalBtn = document.getElementById('closeAppointmentModal');
    var cancelModalBtn = document.getElementById('cancelAppointmentModal');
    var approveBtn = document.getElementById('approveAppointmentBtn');

    var selectedAppointmentId = null;

    function setText(id, value) {
        var element = document.getElementById(id);

        if (element) {
            element.textContent = value || '—';
        }
    }

    function openAppointmentModal(appointment) {
        selectedAppointmentId = appointment.id;

        setText('modalProperty', appointment.property);
        // setText('modalPropertyId', appointment.propertyId);
        setText('modalContactName', appointment.contactName);
        setText('modalEmail', appointment.email);
        setText('modalPhone', appointment.phone);
        setText('modalScheduledAt', appointment.scheduledAt);
        setText('modalAgent', appointment.agent);
        setText('modalStatus', appointment.status);
        setText('modalSource', appointment.source);
        setText('modalCreatedAt', appointment.createdAt);
        setText('modalNotes', appointment.notes || 'No additional notes provided.');

        var isPending = String(appointment.status).toLowerCase() === 'pending';

        approveBtn.classList.toggle('hidden', !isPending);

        modal.showModal();

        if (window.lucide) tableUtils.icons(el);
    }

    function closeAppointmentModal() {
        modal.close();
    }

    // Open modal
    document.addEventListener('click', function (e) {
        var button = e.target.closest('.appointment-view');

        if (!button) return;

        var appointmentId = button.dataset.id;
        var row = table.getRow(appointmentId);

        if (!row) return;

        openAppointmentModal(row.getData());
    });

    modal.addEventListener('close', function () { selectedAppointmentId = null; });

    // Close modal
    closeModalBtn.addEventListener('click', closeAppointmentModal);
    cancelModalBtn.addEventListener('click', closeAppointmentModal);

    // Close when clicking outside the modal content
    modal.addEventListener('click', function (e) {
        if (e.target === modal) closeAppointmentModal();
    });

    // Approve appointment
    approveBtn.addEventListener('click', async function () {
        if (!selectedAppointmentId) return;

        const appointmentId = selectedAppointmentId;
        if (!confirm('Are you sure you want to approve this viewing request?')) {
            return;
        }

        var approveUrl = approveBtn.dataset.approveUrl;
        var token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        approveBtn.disabled = true;
        approveBtn.textContent = 'Approving...';

        try {
            var response = await fetch(approveUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: new URLSearchParams({
                    appointmentId: appointmentId
                })
            });

            var result = await response.json().catch(function () { return {}; });

            if (!response.ok || !result.success) {
                throw new Error(result.message || 'Unable to approve appointment.');
            }

            alert(result.message);

            // Update the current Tabulator row.
            var row = table.getRow(appointmentId);

            if (row) {
                await row.update({ status: 'Approved' });
            }

            if (modal.open && selectedAppointmentId === appointmentId) closeAppointmentModal();

        } catch (error) {
            alert(error.message || 'Something went wrong.');
        } finally {
            approveBtn.disabled = false;

            approveBtn.innerHTML =
                '<i data-lucide="check-circle" class="size-4"></i>Approve Request';

            tableUtils.icons(approveBtn);
        }
    });
    table.on('renderComplete', function () {
        if (window.lucide) tableUtils.icons(el);
    });


})();
