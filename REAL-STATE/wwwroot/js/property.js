document.addEventListener('click', function (e) {
    const btn = e.target.closest('.js-delete');
    if (!btn) return;

    const id = btn.dataset.id;
    const title = btn.dataset.title;
    const deleteUrl = '/Admin/Property/Delete';

    Swal.fire({
        title: 'Delete property?',
        text: `"${title}" will be permanently deleted.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#b91c1c',
        cancelButtonColor: '#1B1B2F',
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

        document.dispatchEvent(new CustomEvent('property:deleted', { detail: { id } }));

        Swal.fire({
            icon: 'success',
            title: 'Deleted',
            text: result.value.message,
            timer: 1500,
            showConfirmButton: false
        });
    });
});