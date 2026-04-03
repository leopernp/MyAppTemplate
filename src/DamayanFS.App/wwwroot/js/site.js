// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    // Attach antiforgery token to all AJAX POST requests
    $.ajaxSetup({
        beforeSend: function (xhr, settings) {
            if (settings.type === 'POST' || settings.type === 'post') {
                xhr.setRequestHeader(
                    'RequestVerificationToken',
                    $('input[name="__RequestVerificationToken"]').val()
                );
            }
        }
    });
});