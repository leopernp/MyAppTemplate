/* =============================================================
   MyAppTemplate Admin — admin.js
   Handles: active nav, theme toggle, sidebar collapse,
            alert auto-dismiss, DataTables global defaults.
   Depends on: Bootstrap 5, jQuery, DataTables (loaded before
   this file in _Layout.cshtml).
   ============================================================= */

(function () {
    'use strict';

    const ROOT = document.documentElement;
    const THEME_KEY = 'theme';
    const SIDEBAR_KEY = 'sidebar_collapsed';
    const themeBtn = document.getElementById('themeToggle');

    // -- 1. Theme ----------------------------------------------
    //
    // Two separate responsibilities kept explicit:
    //   applyTheme(t)  — writes the attribute + updates the button icon
    //   setTheme(t)    — persists to localStorage then calls applyTheme
    //
    // The icon state is driven by a CSS class on the button itself
    // (.is-light / .is-dark), NOT by a [data-bs-theme] attribute selector.
    // This avoids any conflict with Bootstrap's own dark-mode styles or
    // with any other script that also reads/writes data-bs-theme.

    function applyTheme(theme) {
        ROOT.setAttribute('data-bs-theme', theme);

        if (themeBtn) {
            themeBtn.classList.toggle('is-dark', theme === 'dark');
            themeBtn.classList.toggle('is-light', theme === 'light');
        }

        // Redraw all live DataTables so their rows re-inherit
        // the updated CSS custom properties immediately.
        if (window.$ && $.fn.dataTable) {
            $.fn.dataTable.tables({ visible: true, api: true }).draw(false);
        }
    }

    function setTheme(theme) {
        localStorage.setItem(THEME_KEY, theme);
        applyTheme(theme);
    }

    // Apply saved (or OS-preferred) theme immediately on script load.
    // The anti-flash inline script in <head> already set data-bs-theme
    // on the HTML element before paint — this call syncs the button icon
    // to match that already-applied theme.
    (function initTheme() {
        var saved = localStorage.getItem(THEME_KEY)
            || (matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
        applyTheme(saved);
    }());

    themeBtn?.addEventListener('click', function () {
        var next = ROOT.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        setTheme(next);
    });

    // -- 2. Active nav link ------------------------------------
    //
    // Longest-prefix match: /admin/users/edit/5 highlights the
    // Users link (/admin/users) without needing an exact match.

    (function setActiveNav() {
        var currentPath = window.location.pathname.toLowerCase();
        var links = document.querySelectorAll('#sidebarNav .nav-link');
        var bestMatch = null;
        var bestLength = 0;

        links.forEach(function (link) {
            var href = link.getAttribute('href');
            if (!href) return;
            var linkPath = href.toLowerCase();

            if (currentPath === linkPath || currentPath.startsWith(linkPath + '/')) {
                if (linkPath.length > bestLength) {
                    bestLength = linkPath.length;
                    bestMatch = link;
                }
            }
        });

        if (bestMatch) {
            bestMatch.classList.add('active');
        }
    }());

    // -- 3. Sidebar collapse -----------------------------------

    var sidebar = document.getElementById('sidebar');
    var topbar = document.getElementById('topbar');
    var mainEl = document.getElementById('mainContent');
    var collapsed = localStorage.getItem(SIDEBAR_KEY) === 'true';

    function applyCollapse(animate) {
        if (!animate) {
            [sidebar, topbar, mainEl].forEach(function (el) {
                if (el) el.style.transition = 'none';
            });
            requestAnimationFrame(function () {
                [sidebar, topbar, mainEl].forEach(function (el) {
                    if (el) el.style.transition = '';
                });
            });
        }
        if (sidebar) sidebar.classList.toggle('collapsed', collapsed);
        if (topbar) topbar.classList.toggle('sidebar-collapsed', collapsed);
        if (mainEl) mainEl.classList.toggle('sidebar-collapsed', collapsed);
    }

    applyCollapse(false); // instant on first paint

    var sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            collapsed = !collapsed;
            localStorage.setItem(SIDEBAR_KEY, collapsed);
            applyCollapse(true);
        });
    }

    // -- 4. Auto-dismiss alerts --------------------------------

    document.querySelectorAll('.alert-dismissible').forEach(function (el) {
        setTimeout(function () {
            var instance = bootstrap.Alert.getOrCreateInstance(el);
            if (instance) instance.close();
        }, 5000);
    });

    // -- 5. DataTables global defaults ------------------------
    // Set once here. Individual pages override only what they need.

    $(function () {
        $.extend(true, $.fn.dataTable.defaults, {
            pageLength: 15,
            language: {
                search: '',
                searchPlaceholder: 'Search\u2026',
                lengthMenu: 'Show _MENU_',
                info: '_START_\u2013_END_ of _TOTAL_',
                infoEmpty: '0 records',
                infoFiltered: '(filtered from _MAX_)',
                paginate: { previous: '\u2039', next: '\u203a' },
                emptyTable: 'No records found.',
                zeroRecords: 'No matching records.',
                processing: '<span class="spinner-border spinner-border-sm me-2"></span>Processing\u2026'
            },
            dom:
                "<'row mb-3 px-3 align-items-center'" +
                "<'col-sm-6 d-flex align-items-center gap-3'lB>" +
                "<'col-sm-6'f>" +
                ">" +
                "<'row'<'col-12'tr>>" +
                "<'row mt-3 px-3'<'col-sm-5'i><'col-sm-7 d-flex justify-content-end'p>>",
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="bi bi-file-earmark-spreadsheet me-1"></i>Excel',
                    className: 'btn btn-sm',
                    title: 'MyAppTemplate Export'
                },
                {
                    extend: 'csv',
                    text: '<i class="bi bi-filetype-csv me-1"></i>CSV',
                    className: 'btn btn-sm'
                },
                {
                    extend: 'print',
                    text: '<i class="bi bi-printer me-1"></i>Print',
                    className: 'btn btn-sm',
                    autoPrint: false
                }
            ]
        });
    });

}());