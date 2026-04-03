"use strict";

$(function () {
    var dataTableInstance = null;
    var editModal = new bootstrap.Modal(document.getElementById('modalEditUser'));

    var roleSelect = window.initTomSelect('#User_RoleId', {
        url:        '/Role/Inquire',
        valueField: 'Id',
        labelField: 'Name',
        params:     { isActive: true }
    });

    var $perms = $('#pagePermissions');
    var canUpdate = $perms.data('can-update') === true;
    var canDelete = $perms.data('can-delete') === true;

    initializeDataTable();

    $('#btnRefreshUsers').on('click', function () {
        if (dataTableInstance)
            dataTableInstance.ajax.reload(null, false);
    });

    function initializeDataTable() {
        if (dataTableInstance)
            return;

        dataTableInstance = window.initializeDataTable({
            tableSelector: '#tableUsers',
            ajaxUrl: '/User/Inquire',
            ajaxData: function (d) {
                d.roleId = null;
                d.isActive = null;
            },
            searchInputSelector: '#tableUsersSearch',
            columns: [
                {
                    data: 'FullName',
                    render: function (data, type, row) {
                        if (!data) return '';

                        return `
                            <span class="text-truncate d-inline-block" style="max-width: 150px;">${data}</span>
                            <i class="small"><br/>(${row.Username})</i>
                        `;
                    }
                },
                { data: 'Email' },
                { data: 'RoleName' },
                {
                    data: 'IsActive',
                    class: 'text-center',
                    render: function (data) {
                        if (data) {
                            return `<i class="bi bi-check-circle-fill text-success" style="font-size:1rem;"></i>`;
                        } else {
                            return `<i class="bi bi-x-circle-fill text-danger" style="font-size:1rem;"></i>`;
                        }
                    }
                },
                {
                    data: 'CreatedByDisplayName',
                    class: 'text-center',
                    render: function (data) {
                        if (!data) return 'N/A';

                        return data;
                    }
                },
                {
                    data: 'CreatedDate',
                    render: function (data) { return window.DataTableRenderers.formatDate(data, "YYYY-MM-DD HH:mm:ss"); }
                },
                {
                    data: 'ModifiedByDisplayName',
                    class: 'text-center',
                    render: function (data) {
                        if (!data) return 'N/A';

                        return data;
                    }
                },
                {
                    data: 'ModifiedDate',
                    render: function (data) { return window.DataTableRenderers.formatDate(data, "YYYY-MM-DD HH:mm:ss"); }
                },
                {
                    data: 'Id',
                    orderable: false,
                    class: 'text-center',
                    render: function (data, type, row) {
                        var editBtn = canUpdate ? `
                            <button class="btn-action btn-action-info edit-btn"
                                data-id="${data}" data-item-name="${row.FullName}" title="Edit">
                                <i class="bi bi-pencil"></i>
                            </button>` : '';
                        var unlockBtn = (!row.IsSuperAdmin && canUpdate && row.IsLockedOut) ? `
                            <button class="btn-action btn-action-success unlock-btn"
                                data-id="${data}" data-item-name="${row.FullName}" title="Unlock Account">
                                <i class="bi bi-unlock"></i>
                            </button>` : '';
                        var resetPasswordBtn = (!row.IsSuperAdmin && canUpdate) ? `
                            <button class="btn-action btn-action-warning reset-password-btn"
                                data-id="${data}" data-item-name="${row.FullName}" title="Reset Password">
                                <i class="bi bi-key"></i>
                            </button>` : '';
                        var deleteBtn = (!row.IsSuperAdmin && canDelete) ? `
                            <button class="btn-action btn-action-danger delete-btn"
                                data-id="${data}" data-item-name="${row.FullName}" title="Delete">
                                <i class="bi bi-trash"></i>
                            </button>` : '';

                        return `
                            <a class="btn-action btn-action-secondary"
                                href="/Permission/UserPermissions?userId=${data}" title="Manage Permissions">
                                <i class="bi bi-person-lock"></i>
                            </a>
                            ${editBtn}
                            ${unlockBtn}
                            ${resetPasswordBtn}
                            ${deleteBtn}
                        `;
                    }
                }
            ],
            language: {
                emptyIcon: 'bi bi-calendar-x',
                searchIcon: 'bi bi-search',
            },
            order: [[0, 'desc']],
            responsive: true,
            searchHighlight: true
        });
    }

    // ── Add ──────────────────────────────────────────────────

    $('#btnAddUser').on('click', function () {
        $('#modalEditUserLabel').text('Add User');
        $('#formEditUser')[0].reset();
        $('#User_Id').val(0);
        roleSelect.clear(true);
        $('#User_IsActive').prop('checked', true);
        editModal.show();
    });

    // ── Edit ─────────────────────────────────────────────────

    $(document).on('click', '.edit-btn', function () {
        var id = $(this).data('id');

        $.get('/User/GetById', { id: id }, function (user) {
            $('#modalEditUserLabel').text('Edit User');
            $('#User_Id').val(user.Id);
            $('#User_FirstName').val(user.FirstName);
            $('#User_LastName').val(user.LastName);
            $('#User_MiddleName').val(user.MiddleName ?? '');
            $('#User_Username').val(user.Username);
            $('#User_Email').val(user.Email);
            $('#User_IsActive').prop('checked', user.IsActive);
            roleSelect.setValue(user.RoleId, true);

            editModal.show();
        }).fail(function () {
            Swal.fire('Error', 'Failed to load user data.', 'error');
        });
    });

    $('#btnSaveUser').on('click', function () {
        var $form = $('#formEditUser');
        if (!$form[0].checkValidity()) {
            $form[0].reportValidity();
            return;
        }

        var isNew = parseInt($('#User_Id').val()) === 0;

        var model = {
            Id:         parseInt($('#User_Id').val()),
            FirstName:  $('#User_FirstName').val().trim(),
            LastName:   $('#User_LastName').val().trim(),
            MiddleName: $('#User_MiddleName').val().trim() || null,
            Username:   $('#User_Username').val().trim(),
            Email:      $('#User_Email').val().trim(),
            RoleId:     parseInt(roleSelect.getValue()),
            IsActive:   $('#User_IsActive').is(':checked')
        };

        var $btn = $(this);
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

        $.ajax({
            url: '/User/Save',
            method: 'POST',
            data: model,
            success: function () {
                editModal.hide();
                dataTableInstance.ajax.reload(null, false);
                Swal.fire({ icon: 'success', title: 'Saved', text: isNew ? 'User created successfully.' : 'User updated successfully.', timer: 1500, showConfirmButton: false });
            },
            error: function (xhr) {
                Swal.fire('Error', xhr.responseText || 'Failed to save user.', 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).text('Save Changes');
            }
        });
    });

    // ── Reset Password ───────────────────────────────────────

    $(document).on('click', '.reset-password-btn', function () {
        var id   = $(this).data('id');
        var name = $(this).data('item-name');

        Swal.fire({
            title: 'Reset Password',
            html: `Reset password for <strong>${name}</strong> to the system default?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Reset',
            cancelButtonText: 'Cancel',
            confirmButtonColor: 'var(--bs-warning)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/User/ResetPassword',
                method: 'POST',
                data: { id: id },
                success: function () {
                    Swal.fire({ icon: 'success', title: 'Password Reset', text: `Password for ${name} has been reset to default.`, timer: 1800, showConfirmButton: false });
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to reset password.', 'error');
                }
            });
        });
    });

    // ── Unlock ───────────────────────────────────────────────

    $(document).on('click', '.unlock-btn', function () {
        var id   = $(this).data('id');
        var name = $(this).data('item-name');

        Swal.fire({
            title: 'Unlock Account',
            html: `Unlock account for <strong>${name}</strong>?`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes, Unlock',
            cancelButtonText: 'Cancel',
            confirmButtonColor: 'var(--bs-success)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/User/UnlockUser',
                method: 'POST',
                data: { id: id },
                success: function () {
                    dataTableInstance.ajax.reload(null, false);
                    Swal.fire({ icon: 'success', title: 'Unlocked', text: `${name}'s account has been unlocked.`, timer: 1500, showConfirmButton: false });
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to unlock account.', 'error');
                }
            });
        });
    });

    // ── Delete ───────────────────────────────────────────────

    $(document).on('click', '.delete-btn', function () {
        var id   = $(this).data('id');
        var name = $(this).data('item-name');
        var currentUserId = parseInt($('#userPageMeta').data('current-user-id'), 10);

        if (id === currentUserId) {
            Swal.fire('Not Allowed', 'You cannot delete your own account.', 'warning');
            return;
        }

        Swal.fire({
            title: 'Delete User',
            html: `Are you sure you want to delete <strong>${name}</strong>?<br/>This action cannot be undone.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: 'var(--bs-danger)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/User/Delete',
                method: 'POST',
                data: { id: id },
                success: function () {
                    dataTableInstance.ajax.reload(null, false);
                    Swal.fire({ icon: 'success', title: 'Deleted', text: `${name} has been deleted.`, timer: 1500, showConfirmButton: false });
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to delete user.', 'error');
                }
            });
        });
    });
})
