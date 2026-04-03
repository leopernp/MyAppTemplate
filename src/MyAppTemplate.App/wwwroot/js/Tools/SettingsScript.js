$(document).ready(function () {
    loadSettings();
    loadBackupList();
});

function loadSettings() {
    $.ajax({
        url: '/api/appsettings',
        type: 'GET',
        success: function (data) {
            $('#json-editor').val(JSON.stringify(data, null, 4));
        }
    });
}

function loadBackupList() {
    $.ajax({
        url: '/api/appsettings/backups',
        type: 'GET',
        success: function (backups) {
            const $list = $('#backup-list').empty();
            if (backups.length === 0) {
                $list.append('<div class="p-3 text-center text-muted small">No backups found</div>');
                return;
            }
            $.each(backups, function (i, b) {
                $list.append(`
                    <div class="backup-item list-group-item d-flex justify-content-between align-items-center">
                        <div class="d-flex flex-column">
                            <span class="small fw-bold">${b.date}</span>
                            <span class="text-muted" style="font-size: 0.7rem;">${b.fileName.split('.').slice(1, 2)}</span>
                        </div>
                        <button onclick="restoreBackup('${b.fileName}')" class="btn btn-restore btn-xs btn-outline-warning py-0 px-2" style="font-size: 0.7rem;">
                            Restore
                        </button>
                    </div>
                `);
            });
        }
    });
}

function saveSettings() {
    const rawValue = $('#json-editor').val();
    let jsonData;
    try { jsonData = JSON.parse(rawValue); } catch (e) { alert("Invalid JSON!"); return; }

    showStatus('Saving...', 'primary');

    $.ajax({
        url: '/api/appsettings/save',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(jsonData),
        success: function (res) {
            showStatus(res.message, 'success');
            loadBackupList(); // Refresh history
        },
        error: function () { showStatus('Error saving!', 'danger'); }
    });
}

function restoreBackup(fileName) {
    if (!confirm("Are you sure? This will overwrite the current settings and restart the app.")) return;

    showStatus('Restoring...', 'warning');

    $.ajax({
        url: `/api/appsettings/restore/${fileName}`,
        type: 'POST',
        success: function (res) {
            showStatus(res.message, 'success');
            setTimeout(() => location.reload(), 2000);
        },
        error: function () { showStatus('Restore failed!', 'danger'); }
    });
}

function showStatus(msg, type) {
    $('#save-status').html(`<span class="text-${type} small"><i class="bi bi-info-circle me-1"></i>${msg}</span>`);
}