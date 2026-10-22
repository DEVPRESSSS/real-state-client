$(function () {
    if (document.getElementById('agentTable')) {
        new DataTable('#agentTable', {
            pageLength: 10,
            order: [],
            columnDefs: [{ orderable: false, targets: -1 }],
            language: {
                search: 'Search:',
                emptyTable: 'No agents found.',
                zeroRecords: 'No matching agents found.'
            }
        });
    }

    const deleteUrl = '/Admin/Agent/Delete';

    $(document).on('click', '.js-delete', function () {
        const btn = this;
        const id = btn.dataset.id;
        const title = btn.dataset.title;

        Swal.fire({
            title: 'Delete agent?',
            text: `"${title}" will be permanently deleted.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it',
            reverseButtons: true,
            showLoaderOnConfirm: true,
            allowOutsideClick: () => !Swal.isLoading(),
            preConfirm: async () => {
                try {
                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

                    const res = await fetch(deleteUrl, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded',
                            'RequestVerificationToken': token
                        },
                        body: new URLSearchParams({ id })
                    });

                    const data = await res.json();
                    if (!res.ok || !data.success) {
                        throw new Error(data.message || 'Delete failed.');
                    }
                    return data;
                } catch (err) {
                    Swal.showValidationMessage(err.message);
                }
            }
        }).then(result => {
            if (!result.isConfirmed) return;

            $('#agentTable').DataTable().row($(btn).closest('tr')).remove().draw(false);

            Swal.fire({
                icon: 'success',
                title: 'Deleted',
                text: result.value.message,
                timer: 1500,
                showConfirmButton: false
            });
        });
    });
});