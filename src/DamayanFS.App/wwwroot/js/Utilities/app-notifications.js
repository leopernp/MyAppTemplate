// Global Notification Helper
const AppNotifications = {
    // 1. Toast Notification (Top Right, non-blocking)
    toast: Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    }),

    // 2. Confirm Dialog (Returns a Promise)
    confirm: function (title, text, confirmButtonText = 'OK') {
        return Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showCancelButton: true,
            // Disable default styles to let our CSS/Bootstrap take over
            buttonsStyling: false, 
            customClass: {
                confirmButton: 'btn btn-primary px-4 me-2', // Indigo 600
                cancelButton: 'btn btn-outline-secondary px-4',
                popup: 'rounded-4 border-0' // Optional: slightly softer corners
            },
            confirmButtonText: confirmButtonText,
            cancelButtonText: 'Cancel',
            reverseButtons: true // Puts "Cancel" on the left, "OK" on the right (Standard UI)
        });
    },

    // 3. Loading Modal (Show)
    showLoading: function (message = 'Please wait...', text = 'We are processing your request...') {
        Swal.fire({
            title: message,
            text: text,
            allowOutsideClick: false,
            showConfirmButton: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    },

    // 4. Loading Modal (Hide)
    closeLoading: function () {
        Swal.close();
    },

    // 5. Highly Customizable Modal
    customModal: function (options) {
        /* Expected options object: 
           { title, html, icon, confirmText, cancelText, showCancel } 
        */
        return Swal.fire({
            title: options.title || 'Notification',
            html: options.html || '',
            icon: options.icon || 'info',
            showCancelButton: options.showCancel !== false,
            confirmButtonText: options.confirmText || 'OK',
            cancelButtonText: options.cancelText || 'Cancel',
            reverseButtons: true
        });
    },
    confirmDestructive: function (title, html, requiredText = 'ROLLBACK') {
        return Swal.fire({
            title: title,
            html: html,
            icon: 'warning',
            input: 'text',
            inputPlaceholder: `Type ${requiredText} to confirm`,
            showCancelButton: true,
            // Disable default SWAL styling
            buttonsStyling: false,
            customClass: {
                confirmButton: 'btn btn-danger px-4', // Using Bootstrap Danger (Red)
                cancelButton: 'btn btn-outline-secondary px-4 me-2',
                input: 'form-control mt-3 mx-auto w-75', // Matching your Bootstrap inputs
                popup: 'rounded-4'
            },
            confirmButtonText: 'Execute Action',
            cancelButtonText: 'Cancel',
            reverseButtons: true, // Cancel on left, Danger on right
            preConfirm: (value) => {
                if (value.toUpperCase() !== requiredText.toUpperCase()) {
                    Swal.showValidationMessage(`You must type "${requiredText}" to proceed`);
                    return false;
                }
                return true;
            }
        });
    },
};

// Global jQuery-style functions for easy access
function showToast(message, type = 'success') {
    AppNotifications.toast.fire({
        icon: type,
        title: message
    });
}

function showAlert(title, message, type = 'info') {
    Swal.fire(title, message, type);
}

function showLoading(msg, text) {
    AppNotifications.showLoading(msg, text); 
}

function hideLoading() { 
    AppNotifications.closeLoading(); 
}