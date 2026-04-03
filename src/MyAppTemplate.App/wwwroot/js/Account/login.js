$(function () {

    // ── Theme ──────────────────────────────────────────────────────────────────
    var THEME_KEY = 'theme';

    function applyLoginTheme(theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
        var icon = $('#loginThemeIcon');
        if (theme === 'dark') {
            icon.attr('class', 'bi bi-moon');
        } else {
            icon.attr('class', 'bi bi-sun');
        }
    }

    (function initLoginTheme() {
        var saved = localStorage.getItem(THEME_KEY) ||
            (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
        applyLoginTheme(saved);
    }());

    $('#loginThemeToggle').on('click', function () {
        var current = document.documentElement.getAttribute('data-bs-theme');
        var next = current === 'dark' ? 'light' : 'dark';
        localStorage.setItem(THEME_KEY, next);
        applyLoginTheme(next);
    });

    // ── Password Toggle ────────────────────────────────────────────────────────
    $('#togglePassword').on('click', function () {
        var input = $('#passwordInput');
        var icon = $('#togglePasswordIcon');
        var isPassword = input.attr('type') === 'password';
        input.attr('type', isPassword ? 'text' : 'password');
        icon.attr('class', isPassword ? 'bi bi-eye-slash' : 'bi bi-eye');
    });

    // ── Lockout Countdown ──────────────────────────────────────────────────────
    var countdownEl = $('#lockoutCountdown');
    if (countdownEl.length) {
        var seconds = parseInt(countdownEl.text(), 10);

        var timer = setInterval(function () {
            seconds--;
            countdownEl.text(seconds);

            if (seconds <= 0) {
                clearInterval(timer);
                countdownEl.closest('.alert').remove();
            }
        }, 1000);
    }

});