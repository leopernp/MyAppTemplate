$(document).ready(function () {
    loadMigrations();
});

function loadMigrations() {
    const $list = $('#migration-list').html('<tr><td colspan="4" class="text-center p-5 text-muted">Checking Schema...</td></tr>');
    const $syncBtn = $('#btn-sync-db');

    $.ajax({
        url: '/api/database/migrations',
        type: 'GET',
        success: function (response) {
            // 1. Check exactly what the server sent in your F12 console
            console.log("Full Server Response:", response);

            $list.empty();
            let pendingCount = 0;

            // 2. Handle both 'migrations' and 'Migrations'
            const migrations = response.migrations || response.Migrations;

            // If it's still undefined, the structure is wrong
            if (!migrations) {
                console.error("Could not find migrations property. Check console log above.");
                $list.html('<tr><td colspan="4" class="text-center p-5 text-danger">Data structure error. See console.</td></tr>');
                return;
            }

            $.each(migrations, function (i, m) {
                // 3. Handle property casing safely
                const isApplied = (m.isApplied !== undefined) ? m.isApplied : m.IsApplied;
                const id = m.id || m.Id;
                const timestamp = m.timestamp || m.Timestamp;

                if (!isApplied) pendingCount++;

                const statusColor = isApplied ? "success" : "warning";
                const rowClass = isApplied ? "" : "table-warning-subtle";
                const rollbackBtn = isApplied ? `
                    <button onclick="rollbackTo('${id}')" class="btn btn-link btn-sm text-danger p-0 shadow-none text-decoration-none small me-3">
                        <i class="bi bi-arrow-counterclockwise me-1"></i>Rollback
                    </button>` : '';

                $list.append(`
                    <tr class="${rowClass}">
                        <td class="ps-4 text-center"><span class="status-dot bg-${statusColor}"></span></td>
                        <td class="font-monospace small fw-bold">${id}</td>
                        <td class="text-muted small">${timestamp}</td>
                        <td class="text-end pe-4">
                            ${rollbackBtn}
                            <button onclick="viewSql('${id}')" class="btn btn-link btn-sm text-primary p-0 me-3 shadow-none text-decoration-none small">
                                <i class="bi bi-code-slash me-1"></i>SQL
                            </button>
                            ${isApplied
                                ? '<span class="badge rounded-pill bg-success-subtle text-success border border-success-subtle">Applied</span>'
                                : '<span class="badge rounded-pill bg-warning-subtle text-warning border border-warning-subtle">Pending</span>'}
                        </td>
                    </tr>
                `);
            });

            // 4. Handle Snapshot Version casing
            const snapVer = response.snapshotVersion || response.SnapshotVersion || "Unknown";
            $('#migration-stats').html(`
                <span class="me-3 opacity-75">Snapshot: ${snapVer}</span>
                <span class="me-3">Total: ${migrations.length}</span>
                <span class="${pendingCount > 0 ? 'text-warning fw-bold' : ''}">Pending: ${pendingCount}</span>
            `);

            // Toggle Sync Button
            if (pendingCount > 0) $('#btn-sync-db').removeClass('d-none');
            else $('#btn-sync-db').addClass('d-none');
        }
    });
}

function viewSql(migrationId) {
    $('#sql-content').text('-- Loading SQL Script...');
    const modal = new bootstrap.Modal(document.getElementById('sqlModal'));
    modal.show();

    $.get(`/api/database/script/${migrationId}`, function (script) {
        $('#sql-content').text(script);
    });
}

function syncDatabase() {
    AppNotifications.confirm(
        "Apply Migrations?", 
        "This will execute schema changes on the live database. Ensure you have a backup!",
        "Yes, Sync Now"
    ).then((result) => {
        if (result.isConfirmed) {
            // Use your loading modal
            showLoading("Syncing Database", "Applying pending migrations... please do not close the window.");

            $.post('/api/database/apply')
                .done(function (res) {
                    hideLoading();
                    showToast(res.message, 'success');
                    loadMigrations(); // Refresh the list
                })
                .fail(function (err) {
                    hideLoading();
                    const errorMsg = err.responseJSON?.message || "Check server logs for details.";
                    showAlert("Sync Failed", errorMsg, 'error');
                });
        }
    });
}

function rollbackTo(migrationId) {
    const message = `
        <div class="text-center">
            <p>Reverting to: <b class="text-primary">${migrationId}</b></p>
            <p class="text-danger fw-bold">Warning: Data in newer columns will be lost.</p>
        </div>`;

    AppNotifications.confirmDestructive("Database Rollback", message, "ROLLBACK")
        .then((result) => {
            if (result.isConfirmed) {
                showLoading("Rolling Back", "Updating database schema...");

                $.post(`/api/database/rollback/${migrationId}`)
                    .done(function (res) {
                        hideLoading();
                        showToast(res.message, 'success');
                        loadMigrations();
                    })
                    .fail(function (err) {
                        hideLoading();
                        showAlert("Error", err.responseJSON?.message || "Rollback failed", 'error');
                    });
            }
        });
}