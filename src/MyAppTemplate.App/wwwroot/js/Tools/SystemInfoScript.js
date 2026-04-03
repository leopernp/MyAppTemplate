const SystemMonitor = {
    connection: new signalR.HubConnectionBuilder()
        .withUrl("/systemHub")
        .withAutomaticReconnect()
        .build(),

    init: function() {
        this.connection.on("ReceiveSystemUpdate", (data) => {
            this.updateUI(data);
        });

        // Add these listeners for better UX
        this.connection.onreconnecting((error) => {
            showToast("Connection lost. Reconnecting...", "warning");
        });

        this.connection.onreconnected((connectionId) => {
            showToast("Connection restored!", "success");
        });

        // Handle the Toggle Switch
        $('#liveModeToggle').on('change', function() {
            if (this.checked) {
                SystemMonitor.start();
            } else {
                SystemMonitor.stop();
            }
        });
    },

    start: function() {
        if (this.connection.state === "Disconnected") {
            this.connection.start()
                .then(() => {
                    showToast("Live monitoring enabled", "success");
                    $('.form-check-label').removeClass('text-muted').addClass('text-primary');
                })
                .catch(err => {
                    showAlert("Connection Failed", "Could not start live stream.", "error");
                    $('#liveModeToggle').prop('checked', false);
                });
        }
    },

    stop: function() {
        if (this.connection.state !== "Disconnected") {
            this.connection.stop().then(() => {
                showToast("Live monitoring paused", "info");
                $('.form-check-label').addClass('text-muted').removeClass('text-primary');
            });
        }
    },

    updateUI: function(data) {
        console.log("Raw Data from Hub:", data);

        // Update cards with a small "pulse" effect so user sees the change
        $('#info-uptime').text(data.Uptime).addClass('text-primary');
        $('#info-memory').text(data.MemoryUsage);

        const $dbStatus = $('#info-db');
        if (data.DbStatus === "Healthy") {
            $dbStatus.text("Connected").addClass('text-success').removeClass('text-danger');
        } else {
            $dbStatus.text("Offline").addClass('text-danger').removeClass('text-success');
        }

        // Update Table IDs
        $('#env-val').text(data.Environment);
        $('#os-val').text(data.OS);
        $('#frame-val').text(data.Framework);
        $('#cpu-val').text(data.CpuCount + " Cores");
        $('#time-val').text(data.ServerTime);

        // Remove highlight after 500ms
        setTimeout(() => $('#info-uptime').removeClass('text-primary'), 500);
    }
};

// Initialize on load
$(document).ready(() => {
    SystemMonitor.init();
    manualRefresh(); // Initial static load
});

function manualRefresh() {
    // If live mode is off, do a one-time AJAX call
    if (!$('#liveModeToggle').is(':checked')) {
        refreshSystemInfo(); // Your existing AJAX function
    } else {
        showToast("Live mode is already active", "info");
    }
}


function refreshSystemInfo() {
    showLoading("Fetching system data...");
    
    $.ajax({
        url: '/api/systeminfo',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            SystemMonitor.updateUI(data); // Reuse the UI logic from your object
            hideLoading();
            showToast("System info updated", "success");
        },
        error: function () {
            hideLoading();
            showToast("Failed to fetch system info", "error");
        }
    });
}