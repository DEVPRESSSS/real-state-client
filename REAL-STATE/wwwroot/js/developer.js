$(function () {
    if (document.getElementById('developerTable')) {
        new DataTable('#developerTable', {
            pageLength: 10,
            order: [],
            columnDefs: [{ orderable: false, targets: -1 }],
            language: {
                search: 'Search:',
                emptyTable: 'No developers found.',
                zeroRecords: 'No matching developers found.'
            }
        });
    }

    const deleteUrl = '/Admin/Developer/Delete';

    $(document).on('click', '.js-delete', function () {
        const btn = this;
        const id = btn.dataset.id;
        const title = btn.dataset.title;

        Swal.fire({
            title: 'Delete developer?',
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

            $('#developerTable').DataTable().row($(btn).closest('tr')).remove().draw(false);

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