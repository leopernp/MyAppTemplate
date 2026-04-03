let activeFileName = "";

$(document).ready(function () {
    loadFileList();
});

function loadFileList() {
    $.ajax({
        url: '/api/logviewer/files',
        type: 'GET',
        dataType: 'json',
        success: function (files) {
            const $list = $('#file-list');
            $list.empty();

            if (files && files.length > 0) {
                $.each(files, function (i, item) {
                    // Create a rich button content with filename and date
                    const $btn = $('<button>')
                        .attr('type', 'button')
                        .addClass('log-file-item list-group-item list-group-item-action d-flex flex-column align-items-start')
                        .html(`
                            <div class="d-flex w-100 justify-content-between align-items-center">
                                <span class="text-truncate fw-semibold">${item.FileName}</span>
                            </div>
                            <small class="text-muted" style="font-size: 0.75rem;">
                                <i class="bi bi-clock-history me-1"></i>${item.LastModified}
                            </small>
                        `)
                        .on('click', function () {
                            // Pass the fileName property to the loader
                            loadLogContent(item.FileName, $(this));
                        });

                    $list.append($btn);
                });
            } else {
                $list.html('<div class="p-3 text-center text-muted small">No log files found.</div>');
            }
        },
        error: function (xhr, status, error) {
            console.error("Log Fetch Error:", error);
            $('#file-list').html(`
                <div class="p-3 text-center text-danger small">
                    <i class="bi bi-exclamation-triangle d-block mb-2"></i>
                    Failed to load logs.
                </div>
            `);
        }
    });
}

function loadLogContent(fileName, $element) {
    activeFileName = fileName;

    // UI Feedback
    $('.list-group-item').removeClass('active');
    $element.addClass('active');
    $('#current-file-name').text(fileName);

    const $display = $('#log-content');
    $display.text("Loading...");

    $.ajax({
        url: `/api/logviewer/view/${fileName}`,
        method: 'GET',
        dataType: 'text', // Ensure jQuery treats the response as plain text
        success: function (text) {
            $display.text(text);

            // Auto-scroll to bottom
            $display.scrollTop($display[0].scrollHeight);
        },
        error: function () {
            $display.text("Error loading log content.");
        }
    });
}

function refreshCurrentLog() {
    if (activeFileName) {
        const $activeElement = $('.list-group-item.active');
        loadLogContent(activeFileName, $activeElement);
    }
}