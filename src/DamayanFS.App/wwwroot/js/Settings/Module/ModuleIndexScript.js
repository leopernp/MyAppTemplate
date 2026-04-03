"use strict";

$(function () {
    var dataTableInstance = null;
    var editModal = new bootstrap.Modal(document.getElementById('modalEditModule'));
    var iconPickerModal = new bootstrap.Modal(document.getElementById('modalIconPicker'));
    var moduleTypeTomSelect = null;
    var parentModuleTomSelect = null;

    var $perms = $('#pagePermissions');
    var canUpdate = $perms.data('can-update') === true;
    var canDelete = $perms.data('can-delete') === true;

    // Common Bootstrap Icons (verified to exist)
    var bootstrapIcons = [
        'bi bi-house', 'bi bi-file-text', 'bi bi-gear', 'bi bi-person', 'bi bi-people',
        'bi bi-lock', 'bi bi-unlock', 'bi bi-check', 'bi bi-x', 'bi bi-plus',
        'bi bi-dash', 'bi bi-trash', 'bi bi-pencil', 'bi bi-eye', 'bi bi-eye-slash',
        'bi bi-search', 'bi bi-funnel', 'bi bi-download', 'bi bi-upload', 'bi bi-link-45deg',
        'bi bi-diagram-3', 'bi bi-boxes', 'bi bi-bookmark', 'bi bi-star', 'bi bi-star-fill',
        'bi bi-heart', 'bi bi-bell', 'bi bi-calendar', 'bi bi-clock', 'bi bi-sliders',
        'bi bi-bar-chart', 'bi bi-pie-chart', 'bi bi-graph-up', 'bi bi-graph-down', 'bi bi-database',
        'bi bi-server', 'bi bi-shield-lock', 'bi bi-key', 'bi bi-check-circle', 'bi bi-folder',
        'bi bi-archive', 'bi bi-clock-history', 'bi bi-info-circle', 'bi bi-question-circle',
        'bi bi-exclamation-circle', 'bi bi-circle-fill', 'bi bi-wrench', 'bi bi-tools', 'bi bi-briefcase',
        'bi bi-credit-card', 'bi bi-wallet', 'bi bi-gift', 'bi bi-flag', 'bi bi-play',
        'bi bi-pause', 'bi bi-stop', 'bi bi-arrow-right', 'bi bi-arrow-left', 'bi bi-arrow-up',
        'bi bi-arrow-down', 'bi bi-arrow-repeat', 'bi bi-arrow-clockwise', 'bi bi-share', 'bi bi-lightning-fill',
        'bi bi-power', 'bi bi-list', 'bi bi-menu-button', 'bi bi-grid', 'bi bi-card-list',
        'bi bi-speedometer', 'bi bi-zoom-in', 'bi bi-zoom-out', 'bi bi-telephone', 'bi bi-chat',
        'bi bi-envelope', 'bi bi-globe', 'bi bi-map', 'bi bi-rss', 'bi bi-award', 'bi bi-ribbon'
    ];

    initializeDataTable();

    $('#btnRefreshModules').on('click', function () {
        if (dataTableInstance)
            dataTableInstance.ajax.reload(null, false);
    });

    function initializeDataTable() {
        if (dataTableInstance)
            return;

        dataTableInstance = window.initializeDataTable({
            tableSelector: '#tableModules',
            ajaxUrl: '/Module/Inquire',
            ajaxData: function (d) {
                d.isActive = null;
            },
            searchInputSelector: '#tableModulesSearch',
            columns: [
                { data: 'Name' },
                {
                    data: 'ModuleTypeName',
                    class: 'text-center',
                    render: function (data) {
                        if (!data) return '<span class="text-muted">—</span>';
                        return data;
                    }
                },
                {
                    data: 'ParentModuleName',
                    class: 'text-center',
                    render: function (data) {
                        if (!data) return '<span class="text-muted">—</span>';
                        return `<span class="text-truncate d-inline-block" style="max-width: 150px;" title="${data}">${data}</span>`;
                    }
                },
                {
                    data: 'Controller',
                    class: 'text-center',
                    render: function (data) {
                        if (!data) return '<span class="text-muted">—</span>';
                        return data;
                    }
                },
                {
                    data: 'Action',
                    class: 'text-center',
                    render: function (data) {
                        if (!data) return '<span class="text-muted">—</span>';
                        return data;
                    }
                },
                {
                    data: 'DisplayOrder',
                    class: 'text-center'
                },
                {
                    data: 'IsActive',
                    class: 'text-center',
                    render: function (data) {
                        return data
                            ? `<i class="bi bi-check-circle-fill text-success" style="font-size:1rem;"></i>`
                            : `<i class="bi bi-x-circle-fill text-danger" style="font-size:1rem;"></i>`;
                    }
                },
                {
                    data: 'InMaintenance',
                    class: 'text-center',
                    render: function (data) {
                        return data
                            ? `<i class="bi bi-cone-striped text-warning" style="font-size:1rem;" title="In Maintenance"></i>`
                            : `<span class="text-muted">—</span>`;
                    }
                },
                {
                    data: 'CreatedByDisplayName',
                    class: 'text-center',
                    render: function (data) { return data ?? 'N/A'; }
                },
                {
                    data: 'CreatedDate',
                    render: function (data) { return window.DataTableRenderers.formatDate(data, "YYYY-MM-DD HH:mm:ss"); }
                },
                {
                    data: 'ModifiedByDisplayName',
                    class: 'text-center',
                    render: function (data) { return data ?? 'N/A'; }
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
                        return editBtn + deleteBtn || '<span class="text-muted">—</span>';
                    }
                }
            ],
            language: {
                emptyIcon: 'bi bi-diagram-3-x',
                searchIcon: 'bi bi-search',
            },
            order: [[5, 'asc']],
            responsive: true,
            searchHighlight: true
        });
    }

    // ── Icon Picker ───────────────────────────────────────────

    function populateIconPicker() {
        var $grid = $('#iconPickerGrid');
        $grid.empty();

        bootstrapIcons.forEach(function (iconClass) {
            var $btn = $('<button>')
                .attr('type', 'button')
                .attr('data-icon', iconClass)
                .addClass('btn btn-outline-secondary p-2')
                .html('<i class="' + iconClass + ' fs-5"></i>')
                .on('click', function (e) {
                    e.preventDefault();
                    selectIcon(iconClass);
                });
            $grid.append($btn);
        });
    }

    function selectIcon(iconClass) {
        $('#Module_Icon').val(iconClass);
        $('#iconPreviewEl').attr('class', iconClass);
        iconPickerModal.hide();
    }

    $('#btnIconPicker').on('click', function () {
        populateIconPicker();
        iconPickerModal.show();
        // Adjust backdrop z-index to be below the modal
        setTimeout(function () {
            var backdrop = document.querySelector('.modal-backdrop:last-of-type');
            if (backdrop) {
                backdrop.style.zIndex = '1060';
            }
        }, 10);
    });

    $('#iconSearchInput').on('input', function () {
        var searchTerm = $(this).val().toLowerCase().trim();
        $('#iconPickerGrid button').each(function () {
            var iconClass = $(this).data('icon');
            var matches = iconClass.toLowerCase().includes(searchTerm) || searchTerm === '';
            $(this).toggleClass('d-none', !matches);
        });
    });

    // ── Icon input preview update ──────────────────────────────

    $('#Module_Icon').on('input', function () {
        var val = $(this).val().trim();
        $('#iconPreviewEl').attr('class', val || 'bi bi-diagram-3');
    });

    // ── TomSelect Initialization ──────────────────────────────

    function initializeTomSelects() {
        // Destroy existing instances to avoid conflicts
        window.destroyTomSelect('#Module_ModuleTypeId');
        window.destroyTomSelect('#Module_ParentModuleId');

        // Initialize ModuleType select
        moduleTypeTomSelect = window.initTomSelect('#Module_ModuleTypeId', {
            url: '/ModuleType/Inquire',
            valueField: 'Id',
            labelField: 'Name',
            config: {
                closeAfterSelect: true
            }
        });

        // Initialize ParentModule select with excludeId and moduleTypeId parameters
        initializeParentModuleSelect();
    }

    function initializeParentModuleSelect() {
        window.destroyTomSelect('#Module_ParentModuleId');

        var moduleTypeId = parseInt(moduleTypeTomSelect?.getValue() || 0);
        var $parentSelect = $('#Module_ParentModuleId');

        if (!moduleTypeId || moduleTypeId === 0) {
            // Disable and clear parent module select if no module type selected
            $parentSelect.prop('disabled', true);
            $parentSelect.val('');
            return;
        }

        // Enable and initialize parent module select with filtered results
        $parentSelect.prop('disabled', false);

        parentModuleTomSelect = window.initTomSelect('#Module_ParentModuleId', {
            url: '/Module/GetAvailableParentModules',
            valueField: 'Id',
            labelField: 'Name',
            params: {
                excludeId: parseInt($('#Module_Id').val()) || 0,
                moduleTypeId: moduleTypeId
            },
            config: {
                closeAfterSelect: true
            }
        });
    }

    // Reinitialize parent modules when module type changes
    $(document).on('change', '#Module_ModuleTypeId', function () {
        initializeParentModuleSelect();
    });

    // ── Add ──────────────────────────────────────────────────

    $('#btnAddModule').on('click', function () {
        $('#modalEditModuleLabel').text('Add Module');
        $('#formEditModule')[0].reset();
        $('#Module_Id').val(0);
        $('#Module_IsActive').prop('checked', true);
        $('#Module_InMaintenance').prop('checked', false);
        $('#Module_DisplayOrder').val(0);
        $('#iconPreviewEl').attr('class', 'bi bi-diagram-3');

        initializeTomSelects();
        editModal.show();
    });

    // ── Edit ─────────────────────────────────────────────────

    $(document).on('click', '.edit-btn', function () {
        var id = $(this).data('id');

        $.get('/Module/GetById', { id: id }, function (m) {
            $('#modalEditModuleLabel').text('Edit Module');
            $('#Module_Id').val(m.Id);
            $('#Module_Name').val(m.Name);
            $('#Module_Description').val(m.Description ?? '');
            $('#Module_Controller').val(m.Controller ?? '');
            $('#Module_Action').val(m.Action ?? '');
            $('#Module_Icon').val(m.Icon ?? '');
            $('#Module_DisplayOrder').val(m.DisplayOrder);
            $('#Module_IsActive').prop('checked', m.IsActive);
            $('#Module_InMaintenance').prop('checked', m.InMaintenance);
            $('#iconPreviewEl').attr('class', m.Icon || 'bi bi-diagram-3');

            initializeTomSelects();

            // Set module type value once its options are loaded
            moduleTypeTomSelect.on('load', function onModuleTypeLoaded() {
                moduleTypeTomSelect.off('load', onModuleTypeLoaded);
                moduleTypeTomSelect.setValue(m.ModuleTypeId);

                // Reinitialize parent module select now that module type is set
                initializeParentModuleSelect();

                if (m.ParentModuleId && parentModuleTomSelect) {
                    // Set parent module value once its options are loaded
                    parentModuleTomSelect.on('load', function onParentLoaded() {
                        parentModuleTomSelect.off('load', onParentLoaded);
                        parentModuleTomSelect.setValue(m.ParentModuleId);
                    });
                }
            });

            editModal.show();
        }).fail(function () {
            Swal.fire('Error', 'Failed to load module data.', 'error');
        });
    });

    // ── Save ─────────────────────────────────────────────────

    $('#btnSaveModule').on('click', function () {
        var $form = $('#formEditModule');
        if (!$form[0].checkValidity()) {
            $form[0].reportValidity();
            return;
        }

        var isNew = parseInt($('#Module_Id').val()) === 0;

        var model = {
            Id:              parseInt($('#Module_Id').val()),
            Name:            $('#Module_Name').val().trim(),
            Description:     $('#Module_Description').val().trim() || null,
            Icon:            $('#Module_Icon').val().trim() || null,
            DisplayOrder:    parseInt($('#Module_DisplayOrder').val()) || 0,
            IsActive:        $('#Module_IsActive').is(':checked'),
            InMaintenance:   $('#Module_InMaintenance').is(':checked'),
            Controller:      $('#Module_Controller').val().trim() || null,
            Action:          $('#Module_Action').val().trim() || null,
            ModuleTypeId:    parseInt(moduleTypeTomSelect?.getValue() || 0) || 0,
            ParentModuleId:  parseInt(parentModuleTomSelect?.getValue() || 0) || null
        };

        if (model.ModuleTypeId === 0) {
            Swal.fire('Validation', 'Please select a Module Type.', 'warning');
            return;
        }

        var $btn = $(this);
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

        $.ajax({
            url: '/Module/Save',
            method: 'POST',
            data: model,
            success: function () {
                editModal.hide();
                dataTableInstance.ajax.reload(null, false);
                Swal.fire({ icon: 'success', title: 'Saved', text: isNew ? 'Module created successfully.' : 'Module updated successfully.', timer: 1500, showConfirmButton: false });
            },
            error: function (xhr) {
                Swal.fire('Error', xhr.responseText || 'Failed to save module.', 'error');
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
            title: 'Delete Module',
            html: `Are you sure you want to delete <strong>${name}</strong>?<br/>This action cannot be undone.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: 'var(--bs-danger)',
        }).then(function (result) {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/Module/Delete',
                method: 'POST',
                data: { id: id },
                success: function () {
                    dataTableInstance.ajax.reload(null, false);
                    Swal.fire({ icon: 'success', title: 'Deleted', text: `${name} has been deleted.`, timer: 1500, showConfirmButton: false });
                },
                error: function (xhr) {
                    Swal.fire('Error', xhr.responseText || 'Failed to delete module.', 'error');
                }
            });
        });
    });
});
