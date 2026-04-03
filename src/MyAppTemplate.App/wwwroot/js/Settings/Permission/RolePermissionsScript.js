"use strict";

$(function () {
    var $pageMeta     = $('#pageMeta');
    var roleId        = parseInt($pageMeta.data('role-id'));

    var allModules      = [];
    var rolePermissions = {}; // keyed by ModuleId

    initRoleSelector();
    loadData();

    $('#btnRefresh').on('click', loadData);

    // ── Role selector ─────────────────────────────────────────

    function initRoleSelector() {
        var ts = window.initTomSelect('#roleSelector', {
            url:        '/Role/Inquire',
            valueField: 'Id',
            labelField: 'Name',
            config: {
                onLoad: function () {
                    this.setValue(String(roleId), true);
                }
            }
        });

        if (ts) {
            ts.on('change', function (value) {
                if (!value) return;
                window.location.href = '/Permission/RolePermissions?roleId=' + value;
            });
        }
    }

    // ── Load ─────────────────────────────────────────────────

    function loadData() {
        $('#matrixBody').html(
            '<tr><td colspan="6" class="text-center py-4">' +
            '<span class="spinner-border spinner-border-sm me-2"></span>Loading...</td></tr>'
        );

        var modulesReq = $.get('/Module/Inquire', { isActive: null });
        var permsReq   = $.get('/Permission/GetRolePermissions', { roleId: roleId });

        $.when(modulesReq, permsReq)
            .done(function (mRes, pRes) {
                allModules = mRes[0];

                rolePermissions = {};
                pRes[0].forEach(function (p) {
                    rolePermissions[p.ModuleId] = p;
                });

                renderMatrix();
            })
            .fail(function () {
                $('#matrixBody').html(
                    '<tr><td colspan="6" class="text-center text-danger py-4">' +
                    '<i class="bi bi-exclamation-circle me-1"></i>Failed to load data.</td></tr>'
                );
            });
    }

    // ── Render ───────────────────────────────────────────────

    function renderMatrix() {
        var grouped = groupByType(allModules);
        var rows    = '';

        Object.keys(grouped).sort().forEach(function (typeName) {
            rows += '<tr class="perm-type-header"><td colspan="6"><i class="bi bi-collection me-1"></i>' +
                escHtml(typeName) + '</td></tr>';

            grouped[typeName].forEach(function (m) {
                rows += buildRoleRow(m);
            });
        });

        if (!rows) {
            rows = '<tr><td colspan="6" class="text-center text-muted py-4">No modules found.</td></tr>';
        }

        $('#matrixBody').html(rows);
    }

    function buildRoleRow(m) {
        var perm       = rolePermissions[m.Id] || {};
        var hasRecord  = !!rolePermissions[m.Id];
        var groupBadge = m.IsGroupNode
            ? ' <span class="badge bg-secondary-subtle text-secondary border border-secondary-subtle ms-1" title="No direct page link">Group</span>'
            : '';

        var rowClass = hasRecord ? ' perm-row-active' : '';

        var revokeBtn = hasRecord
            ? '<button class="btn-action btn-action-danger revoke-btn ms-1" data-module-id="' + m.Id + '" title="Revoke All"><i class="bi bi-shield-x"></i></button>'
            : '';

        return '<tr data-module-id="' + m.Id + '" class="' + rowClass + '">' +
            '<td>' + escHtml(m.Name) + groupBadge + '</td>' +
            permCheckCell('CanView',   perm.CanView) +
            permCheckCell('CanCreate', perm.CanCreate) +
            permCheckCell('CanUpdate', perm.CanUpdate) +
            permCheckCell('CanDelete', perm.CanDelete) +
            '<td class="col-actions">' +
                '<button class="btn-action btn-action-success save-row-btn" data-module-id="' + m.Id + '" title="Save"><i class="bi bi-floppy"></i></button>' +
                revokeBtn +
            '</td>' +
        '</tr>';
    }

    function permCheckCell(permName, checked) {
        return '<td class="col-perm text-center">' +
            '<input type="checkbox" class="form-check-input perm-check" data-perm="' + permName + '"' +
            (checked ? ' checked' : '') + '>' +
        '</td>';
    }

    // ── Save Row ─────────────────────────────────────────────

    $(document).on('click', '.save-row-btn', function () {
        var $btn     = $(this);
        var moduleId = parseInt($btn.data('module-id'));
        saveRow(moduleId, $btn);
    });

    function saveRow(moduleId, $btn) {
        var $row = $('tr[data-module-id="' + moduleId + '"]');

        var checks = readChecks($row);
        var anyChecked  = checks.CanView || checks.CanCreate || checks.CanUpdate || checks.CanDelete;
        var existingPerm = rolePermissions[moduleId];

        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

        if (!anyChecked) {
            if (!existingPerm) {
                $btn.prop('disabled', false).html('<i class="bi bi-floppy"></i>');
                return;
            }
            // Delete
            $.ajax({
                url:    '/Permission/DeleteRolePermission',
                method: 'POST',
                data:   { roleId: roleId, moduleId: moduleId },
                success: function () {
                    delete rolePermissions[moduleId];
                    $row.removeClass('perm-row-active');
                    $row.find('.revoke-btn').remove();
                    showQuickSuccess('Permission revoked.');
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to revoke permission.', 'error');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="bi bi-floppy"></i>');
                }
            });
        } else {
            // Save / upsert
            $.ajax({
                url:    '/Permission/SaveRolePermission',
                method: 'POST',
                data: {
                    Id:        existingPerm ? (existingPerm.Id || 0) : 0,
                    RoleId:    roleId,
                    ModuleId:  moduleId,
                    CanView:   checks.CanView,
                    CanCreate: checks.CanCreate,
                    CanUpdate: checks.CanUpdate,
                    CanDelete: checks.CanDelete
                },
                success: function (result) {
                    rolePermissions[moduleId] = result;
                    $row.addClass('perm-row-active');
                    if (!$row.find('.revoke-btn').length) {
                        $btn.after(
                            '<button class="btn-action btn-action-danger revoke-btn ms-1" data-module-id="' + moduleId + '" title="Revoke All">' +
                            '<i class="bi bi-shield-x"></i></button>'
                        );
                    }
                    showQuickSuccess('Permission saved.');
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to save permission.', 'error');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="bi bi-floppy"></i>');
                }
            });
        }
    }

    // ── Revoke All ───────────────────────────────────────────

    $(document).on('click', '.revoke-btn', function () {
        var moduleId   = parseInt($(this).data('module-id'));
        var moduleName = $('tr[data-module-id="' + moduleId + '"]').find('td:first').text().trim();

        Swal.fire({
            title:             'Revoke Permission',
            html:              'Revoke all permissions for <strong>' + escHtml(moduleName) + '</strong>?',
            icon:              'warning',
            showCancelButton:  true,
            confirmButtonText: 'Yes, Revoke',
            cancelButtonText:  'Cancel',
            confirmButtonColor: 'var(--bs-danger)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            var $row = $('tr[data-module-id="' + moduleId + '"]');
            $row.find('.perm-check').prop('checked', false);
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
