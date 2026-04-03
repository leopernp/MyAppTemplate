"use strict";

$(function () {
    var dataTableInstance = null;
    var editModal = new bootstrap.Modal(document.getElementById('modalEditRole'));

    var $perms = $('#pagePermissions');
    var canUpdate = $perms.data('can-update') === true;
    var canDelete = $perms.data('can-delete') === true;

    initializeDataTable();

    $('#btnRefreshRoles').on('click', function () {
        if (dataTableInstance)
            dataTableInstance.ajax.reload(null, false);
    });

    function initializeDataTable() {
        if (dataTableInstance)
            return;

        dataTableInstance = window.initializeDataTable({
            tableSelector: '#tableRoles',
            ajaxUrl: '/Role/Inquire',
            ajaxData: function (d) {
                d.isActive = null;
            },
            searchInputSelector: '#tableRolesSearch',
            columns: [
                { data: 'Name' },
                {
                    data: 'Description',
                    render: function (data) {
                        if (!data) return '<span class="text-muted">—</span>';
                        return `<span class="text-truncate d-inline-block" style="max-width: 200px;" title="${data}">${data}</span>`;
                    }
                },
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
                                data-id="${data}" data-item-name="${row.Name}" title="Edit">
                                <i class="bi bi-pencil"></i>
                            </button>` : '';
                        var deleteBtn = canDelete ? `
                            <button class="btn-action btn-action-danger delete-btn"
                                data-id="${data}" data-item-name="${row.Name}" title="Delete">
                                <i class="bi bi-trash"></i>
                            </button>` : '';
                        return `
                            <a class="btn-action btn-action-secondary"
                                href="/Permission/RolePermissions?roleId=${data}" title="Manage Permissions">
                                <i class="bi bi-shield-lock"></i>
                            </a>
                            ${editBtn}
                            ${deleteBtn}
                        `;
                    }
                }
            ],
            language: {
                emptyIcon: 'bi bi-shield-x',
                searchIcon: 'bi bi-search',
            },
            order: [[0, 'asc']],
            responsive: true,
            searchHighlight: true
        });
    }

    // ── Add ──────────────────────────────────────────────────

    $('#btnAddRole').on('click', function () {
        $('#modalEditRoleLabel').text('Add Role');
        $('#formEditRole')[0].reset();
        $('#Role_Id').val(0);
        $('#Role_IsActive').prop('checked', true);
        editModal.show();
    });

    // ── Edit ─────────────────────────────────────────────────

    $(document).on('click', '.edit-btn', function () {
        var id = $(this).data('id');

        $.get('/Role/GetById', { id: id }, function (role) {
            $('#modalEditRoleLabel').text('Edit Role');
            $('#Role_Id').val(role.Id);
            $('#Role_Name').val(role.Name);
            $('#Role_Description').val(role.Description ?? '');
            $('#Role_IsActive').prop('checked', role.IsActive);

            editModal.show();
        }).fail(function () {
            Swal.fire('Error', 'Failed to load role data.', 'error');
        });
    });

    $('#btnSaveRole').on('click', function () {
        var $form = $('#formEditRole');
        if (!$form[0].checkValidity()) {
            $form[0].reportValidity();
            return;
        }

        var isNew = parseInt($('#Role_Id').val()) === 0;

        var model = {
            Id:          parseInt($('#Role_Id').val()),
            Name:        $('#Role_Name').val().trim(),
            Description: $('#Role_Description').val().trim() || null,
            IsActive:    $('#Role_IsActive').is(':checked')
        };

        var $btn = $(this);
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

        $.ajax({
            url: '/Role/Save',
            method: 'POST',
            data: model,
            success: function () {
                editModal.hide();
                dataTableInstance.ajax.reload(null, false);
                Swal.fire({ icon: 'success', title: 'Saved', text: isNew ? 'Role created successfully.' : 'Role updated successfully.', timer: 1500, showConfirmButton: false });
            },
            error: function (xhr) {
                Swal.fire('Error', xhr.responseText || 'Failed to save role.', 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).text('Save Changes');
            }
        });
    });

    // ── Delete ───────────────────────────────────────────────

    $(document).on('click', '.delete-btn', function () {
        var id   = $(this).data('id');
        var name = $(this).data('item-name');

        Swal.fire({
            title: 'Delete Role',
            html: `Are you sure you want to delete <strong>${name}</strong>?<br/>This action cannot be undone.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: 'var(--bs-danger)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/Role/Delete',
                method: 'POST',
                data: { id: id },
                success: function () {
                    dataTableInstance.ajax.reload(null, false);
                    Swal.fire({ icon: 'success', title: 'Deleted', text: `${name} has been deleted.`, timer: 1500, showConfirmButton: false });
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to delete role.', 'error');
                }
            });
        });
    });
})
