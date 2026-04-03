$(document).ready(function () {
    loadCacheEntries();
});

function loadCacheEntries() {
    const $list = $('#cache-list').html('<tr><td colspan="3" class="text-center p-5 text-muted">Scanning Memory Cache...</td></tr>');
    const $stats = $('#cache-stats');

    $.ajax({
        url: '/api/cachemanagement',
        type: 'GET',
        success: function (response) {
            $list.empty();
            
            // Handle property casing safely (keys vs Keys)
            const keys = response.keys || response.Keys || [];
            const count = response.count ?? response.Count ?? 0;
            const type = response.type || response.Type || "In-Memory";

            if (keys.length === 0) {
                $list.html('<tr><td colspan="3" class="text-center p-5 text-muted">Cache is currently empty.</td></tr>');
            } else {
                $.each(keys, function (i, key) {
                    $list.append(`
                        <tr>
                            <td class="ps-4">
                                <code class="text-primary fw-bold">${key}</code>
                            </td>
                            <td>
                                <span class="badge rounded-pill bg-info-subtle text-info border border-info-subtle small">${type}</span>
                            </td>
                            <td class="text-end pe-4">
                                <button onclick="inspectCache('${key}')" class="btn btn-link btn-sm text-primary p-0 me-3 shadow-none text-decoration-none small">
                                    <i class="bi bi-eye me-1"></i>Inspect
                                </button>
                                <button onclick="removeCacheEntry('${key}')" class="btn btn-link btn-sm text-danger p-0 shadow-none text-decoration-none small">
                                    <i class="bi bi-trash3 me-1"></i>Remove
                                </button>
                            </td>
                        </tr>
                    `);
                });
            }

            // Update stats in the terminal header
            $stats.html(`
                <span class="me-3 opacity-75">Storage: ${type}</span>
                <span class="fw-bold">Total Keys: ${count}</span>
            `);
        },
        error: function (err) {
            $list.html('<tr><td colspan="3" class="text-center p-5 text-danger">Failed to fetch cache registry.</td></tr>');
            console.error("Cache Load Error:", err);
        }
    });
}

function removeCacheEntry(cacheKey) {
    AppNotifications.confirmDestructive(
        "Remove Cache Key?",
        `Are you sure you want to evict <b class="text-danger">${cacheKey}</b>? Any dependent features will need to re-fetch this data.`,
        "EVict"
    ).then((result) => {
        if (result.isConfirmed) {
            showLoading("Evicting Key", "Removing entry from memory...");

            $.ajax({
                url: `/api/cachemanagement/${encodeURIComponent(cacheKey)}`,
                type: 'DELETE'
            })
            .done(function () {
                hideLoading();
                showToast("Entry removed", 'success');
                loadCacheEntries();
            })
            .fail(function (err) {
                hideLoading();
                const errorMsg = err.responseJSON?.message || "Eviction failed.";
                showAlert("Error", errorMsg, 'error');
            });
        }
    });
}

function confirmFlushAll() {
    const message = `
        <div class="text-center">
            <p class="text-danger fw-bold">CRITICAL ACTION</p>
            <p>This will wipe the <b>entire system cache</b>. This may cause a temporary performance dip while data is repopulated.</p>
        </div>`;

    AppNotifications.confirmDestructive("Flush System Cache", message, "FLUSH ALL")
        .then((result) => {
            if (result.isConfirmed) {
                showLoading("Flushing Cache", "Wiping all memory entries...");

                $.ajax({
                    url: '/api/cachemanagement',
                    type: 'DELETE'
                })
                .done(function (res) {
                    hideLoading();
                    showToast(res.message || "Cache cleared", 'success');
                    loadCacheEntries();
                })
                .fail(function (err) {
                    hideLoading();
                    showAlert("Flush Failed", "Could not clear the cache registry.", 'error');
                });
            }
        });
}

function inspectCache(cacheKey) {
    // Reusing your existing Modal logic from the Migrations page
    $('#sql-content').text('-- Fetching cache value...');
    $('.modal-header .uppercase').text(`Cache Inspector: ${cacheKey}`);
    
    const modal = new bootstrap.Modal(document.getElementById('inspectModal'));
    modal.show();

    $.get(`/api/cachemanagement/${encodeURIComponent(cacheKey)}`, function (response) {
        // Pretty-print the JSON value if it's an object
        const value = response.value || response.Value;
        const formatted = typeof value === 'object' 
            ? JSON.stringify(value, null, 2) 
            : value;
            
        $('#sql-content').text(formatted);
    }).fail(function() {
        $('#sql-content').text('Error: Could not retrieve value. It may have expired.');
    });
}