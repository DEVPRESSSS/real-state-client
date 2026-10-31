document.addEventListener('click', function (e) {
    const btn = e.target.closest('.js-lock');
    if (!btn) return;

    const id = btn.dataset.id;
    const title = btn.dataset.title;
    const locked = btn.dataset.locked === 'true';   // true = lock, false = unlock
    const lockUrl = '/Admin/User/ToggleLock';

    Swal.fire({
        title: locked ? 'Lock account?' : 'Unlock account?',
        text: locked
            ? `"${title}" will no longer be able to sign in.`
            : `"${title}" will be able to sign in again.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3F72AF',
        cancelButtonColor: '#1B1B2F',
        confirmButtonText: locked ? 'Yes, lock it' : 'Yes, unlock it',
        reverseButtons: true,
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: async () => {
            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const res = await fetch(lockUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': token
                    },
                    body: new URLSearchParams({ id, locked })
                });
                const data = await res.json();
                if (!res.ok || !data.success) throw new Error(data.message || 'Action failed.');
                return data;
            } catch (err) {
                Swal.showValidationMessage(err.message);
            }
        }
    }).then(result => {
        if (!result.isConfirmed) return;
        Swal.fire({
            icon: 'success',
            title: locked ? 'Locked' : 'Unlocked',
            text: result.value.message,
            timer: 1200,
            showConfirmButton: false
        }).then(() => location.reload());
    });
}); document.addEventListener('click', function (e) {
    const btn = e.target.closest('.js-lock');
    if (!btn) return;

    const id = btn.dataset.id;
    const title = btn.dataset.title;
    const locked = btn.dataset.locked === 'true';   // true = lock, false = unlock
    const lockUrl = '/Admin/User/ToggleLock';

    Swal.fire({
        title: locked ? 'Lock account?' : 'Unlock account?',
        text: locked
            ? `"${title}" will no longer be able to sign in.`
            : `"${title}" will be able to sign in again.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3F72AF',
        cancelButtonColor: '#1B1B2F',
        confirmButtonText: locked ? 'Yes, lock it' : 'Yes, unlock it',
        reverseButtons: true,
        showLoaderOnConfirm: true,
        allowOutsideClick: () => !Swal.isLoading(),
        preConfirm: async () => {
            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const res = await fetch(lockUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': token
                    },
                    body: new URLSearchParams({ id, locked })
                });
                const data = await res.json();
                if (!res.ok || !data.success) throw new Error(data.message || 'Action failed.');
                return data;
            } catch (err) {
                Swal.showValidationMessage(err.message);
            }
        }
    }).then(result => {
        if (!result.isConfirmed) return;
        Swal.fire({
            icon: 'success',
            title: locked ? 'Locked' : 'Unlocked',
            text: result.value.message,
            timer: 1200,
            showConfirmButton: false
        }).then(() => location.reload());
    });
});