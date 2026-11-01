(function () {
    var toggles = document.querySelectorAll('[data-toggle]');

    function setOpen(btn, menu, open) {
        menu.classList.toggle('hidden', !open);
        btn.setAttribute('aria-expanded', open ? 'true' : 'false');
        var iconOpen = btn.querySelector('[data-icon-open]');
        var iconClose = btn.querySelector('[data-icon-close]');
        if (iconOpen && iconClose) {
            iconOpen.classList.toggle('hidden', open);
            iconClose.classList.toggle('hidden', !open);
        }
    }

    toggles.forEach(function (btn) {
        var menu = document.querySelector(btn.getAttribute('data-toggle'));
        if (!menu) return;

        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            setOpen(btn, menu, menu.classList.contains('hidden'));
        });
    });

    // Close the user dropdown when clicking elsewhere or pressing Escape
    document.addEventListener('click', function (e) {
        document.querySelectorAll('[data-dropdown]').forEach(function (dd) {
            if (dd.contains(e.target)) return;
            var btn = dd.querySelector('[data-toggle]');
            var menu = dd.querySelector('div[id]');
            if (btn && menu) setOpen(btn, menu, false);
        });
    });

    document.addEventListener('keydown', function (e) {
        if (e.key !== 'Escape') return;
        toggles.forEach(function (btn) {
            var menu = document.querySelector(btn.getAttribute('data-toggle'));
            if (menu && !menu.classList.contains('hidden')) {
                setOpen(btn, menu, false);
                btn.focus();
            }
        });
    });
})();


try {
    if (localStorage.getItem('adminSidebarCollapsed') === '1') {
        document.documentElement.classList.add('sidebar-collapsed');
    }
} catch (e) { }