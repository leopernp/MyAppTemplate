"use strict";

$(function () {
    var $pageMeta  = $('#pageMeta');
    var userId     = parseInt($pageMeta.data('user-id'));
    var roleId     = parseInt($pageMeta.data('role-id'));

    var allModules      = [];
    var rolePermissions = {}; // keyed by ModuleId — role-level grants (read-only reference)
    var userPermissions = {}; // keyed by ModuleId — user override records

    initUserSelector();
    loadData();

    $('#btnRefresh').on('click', loadData);

    // ── User selector ─────────────────────────────────────────

    function initUserSelector() {
        var ts = window.initTomSelect('#userSelector', {
            url:        '/User/Inquire',
            valueField: 'Id',
            labelField: 'FullName',
            config: {
                onLoad: function () {
                    this.setValue(String(userId), true);
                }
            }
        });

        if (ts) {
            ts.on('change', function (value) {
                if (!value) return;
                window.location.href = '/Permission/UserPermissions?userId=' + value;
            });
        }
    }

    // ── Load ─────────────────────────────────────────────────

    function loadData() {
        $('#matrixBody').html(
            '<tr><td colspan="14" class="text-center py-4">' +
            '<span class="spinner-border spinner-border-sm me-2"></span>Loading...</td></tr>'
        );

        var modulesReq   = $.get('/Module/Inquire', { isActive: null });
        var rolePermsReq = $.get('/Permission/GetRolePermissions', { roleId: roleId });
        var userPermsReq = $.get('/Permission/GetUserPermissions', { userId: userId });

        $.when(modulesReq, rolePermsReq, userPermsReq)
            .done(function (mRes, rpRes, upRes) {
                allModules = mRes[0];

                rolePermissions = {};
                rpRes[0].forEach(function (p) { rolePermissions[p.ModuleId] = p; });

                userPermissions = {};
                upRes[0].forEach(function (p) { userPermissions[p.ModuleId] = p; });

                renderMatrix();
            })
            .fail(function () {
                $('#matrixBody').html(
                    '<tr><td colspan="14" class="text-center text-danger py-4">' +
                    '<i class="bi bi-exclamation-circle me-1"></i>Failed to load data.</td></tr>'
                );
            });
    }

    // ── Render ───────────────────────────────────────────────

    function renderMatrix() {
        var grouped = groupByType(allModules);
        var rows    = '';

        Object.keys(grouped).sort().forEach(function (typeName) {
            rows += '<tr class="perm-type-header"><td colspan="14"><i class="bi bi-collection me-1"></i>' +
                escHtml(typeName) + '</td></tr>';

            grouped[typeName].forEach(function (m) {
                rows += buildUserRow(m);
            });
        });

        if (!rows) {
            rows = '<tr><td colspan="14" class="text-center text-muted py-4">No modules found.</td></tr>';
        }

        $('#matrixBody').html(rows);
    }

    function buildUserRow(m) {
        var rolePerm   = rolePermissions[m.Id] || {};
        var userPerm   = userPermissions[m.Id] || {};
        var hasOverride = !!userPermissions[m.Id];

        var groupBadge = m.IsGroupNode
            ? ' <span class="badge bg-secondary-subtle text-secondary border border-secondary-subtle ms-1" title="No direct page link">Group</span>'
            : '';

        var rowClass = hasOverride ? ' perm-row-active' : '';

        var revokeBtn = hasOverride
            ? '<button class="btn-action btn-action-danger revoke-btn ms-1" data-module-id="' + m.Id + '" title="Remove Override"><i class="bi bi-person-dash"></i></button>'
            : '';

        // Effective = role OR user
        var effView   = (rolePerm.CanView   || false) || (userPerm.CanView   || false);
        var effCreate = (rolePerm.CanCreate || false) || (userPerm.CanCreate || false);
        var effUpdate = (rolePerm.CanUpdate || false) || (userPerm.CanUpdate || false);
        var effDelete = (rolePerm.CanDelete || false) || (userPerm.CanDelete || false);

        return '<tr data-module-id="' + m.Id + '" class="' + rowClass + '">' +
            '<td>' + escHtml(m.Name) + groupBadge + '</td>' +

            // Role grant (read-only badges)
            '<td class="col-perm text-center border-start">' + permBadge(rolePerm.CanView)   + '</td>' +
            '<td class="col-perm text-center">'               + permBadge(rolePerm.CanCreate) + '</td>' +
            '<td class="col-perm text-center">'               + permBadge(rolePerm.CanUpdate) + '</td>' +
            '<td class="col-perm text-center">'               + permBadge(rolePerm.CanDelete) + '</td>' +

            // User override (editable checkboxes)
            '<td class="col-perm text-center border-start">' + permCheckbox('CanView',   userPerm.CanView)   + '</td>' +
            '<td class="col-perm text-center">'              + permCheckbox('CanCreate', userPerm.CanCreate) + '</td>' +
            '<td class="col-perm text-center">'              + permCheckbox('CanUpdate', userPerm.CanUpdate) + '</td>' +
            '<td class="col-perm text-center">'              + permCheckbox('CanDelete', userPerm.CanDelete) + '</td>' +

            // Effective (computed)
            '<td class="col-perm text-center border-start eff-view">'   + permBadge(effView)   + '</td>' +
            '<td class="col-perm text-center eff-create">'              + permBadge(effCreate) + '</td>' +
            '<td class="col-perm text-center eff-update">'              + permBadge(effUpdate) + '</td>' +
            '<td class="col-perm text-center eff-delete">'              + permBadge(effDelete) + '</td>' +

            '<td class="col-actions">' +
                '<button class="btn-action btn-action-success save-row-btn" data-module-id="' + m.Id + '" title="Save Override"><i class="bi bi-floppy"></i></button>' +
                revokeBtn +
            '</td>' +
        '</tr>';
    }

    function permBadge(granted) {
        return granted
            ? '<i class="bi bi-check-circle-fill perm-badge-yes" title="Granted"></i>'
            : '<i class="bi bi-dash-circle    perm-badge-no"  title="Not granted"></i>';
    }

    function permCheckbox(permName, checked) {
        return '<input type="checkbox" class="form-check-input perm-check" data-perm="' + permName + '"' +
            (checked ? ' checked' : '') + '>';
    }

    // ── Live effective update ─────────────────────────────────

    $(document).on('change', '.perm-check', function () {
        var $row     = $(this).closest('tr');
        var moduleId = parseInt($row.data('module-id'));
        var rolePerm = rolePermissions[moduleId] || {};

        var checks = readChecks($row);

        $row.find('.eff-view').html(permBadge((rolePerm.CanView   || false) || checks.CanView));
        $row.find('.eff-create').html(permBadge((rolePerm.CanCreate || false) || checks.CanCreate));
        $row.find('.eff-update').html(permBadge((rolePerm.CanUpdate || false) || checks.CanUpdate));
        $row.find('.eff-delete').html(permBadge((rolePerm.CanDelete || false) || checks.CanDelete));
    });

    // ── Save Row ─────────────────────────────────────────────

    $(document).on('click', '.save-row-btn', function () {
        var $btn     = $(this);
        var moduleId = parseInt($btn.data('module-id'));
        saveRow(moduleId, $btn);
    });

    function saveRow(moduleId, $btn) {
        var $row        = $('tr[data-module-id="' + moduleId + '"]');
        var checks      = readChecks($row);
        var anyChecked  = checks.CanView || checks.CanCreate || checks.CanUpdate || checks.CanDelete;
        var existingPerm = userPermissions[moduleId];

        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

        if (!anyChecked) {
            if (!existingPerm) {
                $btn.prop('disabled', false).html('<i class="bi bi-floppy"></i>');
                return;
            }
            // Delete override
            $.ajax({
                url:    '/Permission/DeleteUserPermission',
                method: 'POST',
                data:   { userId: userId, moduleId: moduleId },
                success: function () {
                    delete userPermissions[moduleId];
                    $row.removeClass('perm-row-active');
                    $row.find('.revoke-btn').remove();
                    showQuickSuccess('Override removed.');
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to remove override.', 'error');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="bi bi-floppy"></i>');
                }
            });
        } else {
            // Save / upsert override
            $.ajax({
                url:    '/Permission/SaveUserPermission',
                method: 'POST',
                data: {
                    Id:        existingPerm ? (existingPerm.Id || 0) : 0,
                    UserId:    userId,
                    ModuleId:  moduleId,
                    CanView:   checks.CanView,
                    CanCreate: checks.CanCreate,
                    CanUpdate: checks.CanUpdate,
                    CanDelete: checks.CanDelete
                },
                success: function (result) {
                    userPermissions[moduleId] = result;
                    $row.addClass('perm-row-active');
                    if (!$row.find('.revoke-btn').length) {
                        $btn.after(
                            '<button class="btn-action btn-action-danger revoke-btn ms-1" data-module-id="' + moduleId + '" title="Remove Override">' +
                            '<i class="bi bi-person-dash"></i></button>'
                        );
                    }
                    showQuickSuccess('Override saved.');
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to save override.', 'error');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="bi bi-floppy"></i>');
                }
            });
        }
    }

    // ── Remove Override ──────────────────────────────────────

    $(document).on('click', '.revoke-btn', function () {
        var moduleId   = parseInt($(this).data('module-id'));
        var moduleName = $('tr[data-module-id="' + moduleId + '"]').find('td:first').text().trim();

        Swal.fire({
            title:             'Remove Override',
            html:              'Remove user override for <strong>' + escHtml(moduleName) + '</strong>?<br/>' +
                               'The user will fall back to their role\'s permissions.',
            icon:              'warning',
            showCancelButton:  true,
            confirmButtonText: 'Yes, Remove',
            cancelButtonText:  'Cancel',
            confirmButtonColor: 'var(--bs-danger)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            var $row = $('tr[data-module-id="' + moduleId + '"]');
            $row.find('.perm-check').prop('checked', false);
            // Trigger change to refresh effective badges before saving
            $row.find('.perm-check').first().trigger('change');
            saveRow(moduleId, $row.find('.save-row-btn'));
        });
    });

    // ── Helpers ──────────────────────────────────────────────

    function readChecks($row) {
        return {
            CanView:   $row.find('[data-perm="CanView"]').is(':checked'),
            CanCreate: $row.find('[data-perm="CanCreate"]').is(':checked'),
            CanUpdate: $row.find('[data-perm="CanUpdate"]').is(':checked'),
            CanDelete: $row.find('[data-perm="CanDelete"]').is(':checked')
        };
    }

    function groupByType(modules) {
        var grouped = {};
        modules.forEach(function (m) {
            var key = m.ModuleTypeName || 'Uncategorized';
            if (!grouped[key]) grouped[key] = [];
            grouped[key].push(m);
        });
        return grouped;
    }

    function showQuickSuccess(msg) {
        Swal.fire({ icon: 'success', title: msg, timer: 1200, showConfirmButton: false });
    }

    function escHtml(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
});
