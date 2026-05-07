(function () {
    const html = document.documentElement;
    const storageKey = 'eqc-theme';

    function getPreferredTheme() {
        const savedTheme = localStorage.getItem(storageKey);
        if (savedTheme === 'light' || savedTheme === 'dark') {
            return savedTheme;
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function setTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem(storageKey, theme);

        document.querySelectorAll('[data-theme-icon]').forEach((icon) => {
            icon.classList.toggle('fa-sun', theme === 'dark');
            icon.classList.toggle('fa-moon', theme !== 'dark');
        });

        window.dispatchEvent(new CustomEvent('eqc:themechanged', { detail: { theme } }));
    }

    setTheme(getPreferredTheme());

    document.addEventListener('DOMContentLoaded', function () {
        const toggle = document.getElementById('themeToggle');
        const sidebarToggle = document.getElementById('sidebarToggle');
        const sidebar = document.getElementById('appSidebar');
        const sidebarBackdrop = document.getElementById('sidebarBackdrop');

        function setSidebarOpen(isOpen) {
            if (!sidebar) {
                return;
            }

            sidebar.classList.toggle('show', isOpen);
            document.body.classList.toggle('sidebar-open', isOpen);

            if (sidebarToggle) {
                sidebarToggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
            }
        }

        if (toggle) {
            toggle.addEventListener('click', function () {
                const nextTheme = html.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
                setTheme(nextTheme);
            });
        }

        if (sidebarToggle && sidebar) {
            sidebarToggle.addEventListener('click', function () {
                setSidebarOpen(!sidebar.classList.contains('show'));
            });
        }

        if (sidebarBackdrop) {
            sidebarBackdrop.addEventListener('click', function () {
                setSidebarOpen(false);
            });
        }

        document.querySelectorAll('.sidebar-link').forEach((link) => {
            link.addEventListener('click', function () {
                setSidebarOpen(false);
            });
        });

        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                setSidebarOpen(false);
            }
        });

        document.querySelectorAll('[data-countup]').forEach((element) => {
            const target = Number(element.getAttribute('data-countup'));
            if (!Number.isFinite(target)) {
                return;
            }

            const duration = 700;
            const start = performance.now();

            function tick(now) {
                const progress = Math.min((now - start) / duration, 1);
                element.textContent = Math.round(target * progress).toLocaleString('vi-VN');

                if (progress < 1) {
                    requestAnimationFrame(tick);
                }
            }

            requestAnimationFrame(tick);
        });
    });
})();
