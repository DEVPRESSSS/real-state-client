(function () {
    var root = document.documentElement;
    var sidebar = document.getElementById('adminSidebar');
    var backdrop = document.getElementById('sidebarBackdrop');
    var openBtn = document.getElementById('sidebarOpen');
    var closeBtn = document.getElementById('sidebarClose');

    function openMenu() {
        sidebar.classList.remove('-translate-x-full');
        backdrop.classList.remove('hidden');
        openBtn.setAttribute('aria-expanded', 'true');
    }

    function closeMenu() {
        sidebar.classList.add('-translate-x-full');
        backdrop.classList.add('hidden');
        openBtn.setAttribute('aria-expanded', 'false');
    }

    if (openBtn) openBtn.addEventListener('click', openMenu);
    if (closeBtn) closeBtn.addEventListener('click', closeMenu);
    if (backdrop) backdrop.addEventListener('click', closeMenu);

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeMenu();
    });

    // Collapse (in sidebar) and expand (in header) both toggle the same state
    [document.getElementById('sidebarToggle'),
    document.getElementById('sidebarExpand')].forEach(function (btn) {
        if (!btn) return;
        btn.addEventListener('click', function () {
            var collapsed = root.classList.toggle('sidebar-collapsed');
            try {
                localStorage.setItem('adminSidebarCollapsed', collapsed ? '1' : '0');
            } catch (e) { }
        });
    });
})();

try {
    if (localStorage.getItem('adminSidebarCollapsed') === '1') {
        document.documentElement.classList.add('sidebar-collapsed');
    }
} catch (e) { }