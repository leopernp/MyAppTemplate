'use strict';

/**
 * Global DataTable initialization utility
 * Provides a standardized way to initialize DataTables across the application
 */
(function (window, $) {
    /**
     * Initialize a DataTable with common configurations
     * @param {Object} config - Configuration object
     * @param {string} config.tableSelector - jQuery selector for the table element
     * @param {string} config.ajaxUrl - URL for AJAX data source
     * @param {Function} [config.ajaxData] - Function to modify AJAX request data
     * @param {Array} config.columns - DataTable column definitions
     * @param {Object} [config.language] - Custom language/message configurations
     * @param {string} [config.language.emptyIcon] - Icon class for empty table
     * @param {string} [config.language.emptyTitle] - Title for empty table
     * @param {string} [config.language.emptyMessage] - Message for empty table
     * @param {Function} [config.drawCallback] - Custom draw callback function
     * @param {Array} [config.order] - Initial ordering (default: [[0, "asc"]])
     * @param {boolean} [config.responsive] - Enable responsive mode (default: true)
     * @param {boolean} [config.autoWidth] - Enable auto width (default: true)
     * @param {boolean} [config.processing] - Show processing indicator (default: true)
     * @param {boolean} [config.select] - Enable row selection (default: true)
     * @param {boolean} [config.scrollX] - Enable horizontal scrolling (default: false)
     * @param {string} [config.searchInputSelector] - jQuery selector for external search input
     * @param {string} [config.dataSrc] - AJAX data source property (default: "")
     * @param {Array} [config.columnDefs] - Additional column definitions
     * @param {boolean} [config.searchHighlight] - Enable search term highlighting (default: true)
     * @param {boolean} [config.showExportButtons] - Show export buttons (default: false)
     * @returns {Object} DataTable instance
     */
    window.initializeDataTable = function (config) {
        if (!config.tableSelector || !config.ajaxUrl || !config.columns) {
            console.error('Missing required parameters: tableSelector, ajaxUrl, and columns are required');
            return null;
        }

        const defaultLanguage = {
            processing: "<div class='text-center'><span class='spinner-border spinner-border-sm'></span> Loading...</div>",
            emptyTable: buildEmptyMessage(
                config.language?.emptyIcon || 'bi bi-inbox',
                config.language?.emptyTitle || 'No Records Found',
                config.language?.emptyMessage || 'There are no records to display.'
            ),
            zeroRecords: buildEmptyMessage(
                config.language?.searchIcon || 'bi bi-search',
                config.language?.searchTitle || 'No Matching Results',
                config.language?.searchMessage || 'Try adjusting your search criteria.'
            ),
            paginate: {
                previous: "<i class='bi bi-chevron-left'></i>",
                next: "<i class='bi bi-chevron-right'></i>"
            }
        };

        const defaultDrawCallback = function () {
            $(".dataTables_paginate > .pagination").addClass("pagination-rounded");
            $(".dt-paging > .pagination").addClass("pagination-rounded");
        };

        const dataTableConfig = {
            ajax: {
                url: config.ajaxUrl,
                data: config.ajaxData || function (d) { },
                dataSrc: config.dataSrc !== undefined ? config.dataSrc : ""
            },
            columns: config.columns.map(function (col) {
                if (col.class && !col.className) {
                    col = Object.assign({}, col, { className: col.class });
                    delete col.class;
                }
                return col;
            }),
            language: $.extend(true, {}, defaultLanguage, config.language || {}),
            drawCallback: function () {
                // Call user-defined draw callback if provided
                if (config.drawCallback) {
                    config.drawCallback();
                } else {
                    defaultDrawCallback();
                }
                // Adjust columns after draw to ensure proper sizing
                setTimeout(function () {
                    if (dataTableInstance) {
                        dataTableInstance.columns.adjust();
                    }
                }, 50);
            },
            responsive: config.responsive !== undefined ? config.responsive : true,
            autoWidth: config.autoWidth !== undefined ? config.autoWidth : false,
            order: config.order || [[0, "asc"]],
            processing: config.processing !== undefined ? config.processing : true,
            select: config.select !== undefined ? config.select : true,
            scrollX: config.scrollX || false,
            columnDefs: config.columnDefs || [],
            searchHighlight: config.searchHighlight || true
        };

        const dataTableInstance = $(config.tableSelector).DataTable(dataTableConfig);

        // Adjust columns after initial load
        setTimeout(function () {
            if (dataTableInstance) {
                dataTableInstance.columns.adjust();
            }
        }, 100);

        $(`${config.tableSelector}_filter, ${config.tableSelector}_length, .dt-search, .dt-length`).hide();

        if (!config.showExportButtons) {
            $('.dt-buttons').hide();
        }

        if (config.searchInputSelector) {
            $(config.searchInputSelector).on('input', function () {
                dataTableInstance.search(this.value).draw();
            });
        }

        return dataTableInstance;
    };

    function buildEmptyMessage(iconClass, title, message) {
        return `<div class='text-center py-4'>
                    <i class='${iconClass} fs-1 text-muted mb-3'></i>
                    <h5 class='text-muted'>${title}</h5>
                    <p class='text-muted'>${message}</p>
                </div>`;
    }

    /**
     * Common render functions for DataTable columns
     */
    window.DataTableRenderers = {
        statusBadge: function (data, activeText = 'Active', inactiveText = 'Inactive') {
            if (data === true) {
                return `<span class="badge-status badge-success">${activeText}</span>`;
            } else {
                return `<span class="badge-status badge-danger">${inactiveText}</span>`;
            }
        },

        formatDate: function (data, format = 'MM/DD/YYYY') {
            // 1. Check if data exists and isn't just whitespace
            if (data && data.trim() !== "") {
                const m = moment.utc(data).local();

                // 2. The Blocker: Check if Moment can actually parse the input
                if (m.isValid()) {
                    return m.format(format);
                }
            }
    
            // 3. Return empty string for null, empty, or invalid dates
            return "";
        },

        formatTime: function (data, format = 'HH:mm') {
            if (data && data.trim() !== "") {
                return moment.utc(data).local().format(format);
            }
            return "";
        },

        renderDateTime: function (data, format = 'MM/DD/YYYY HH:mm') {
            if (data && data.trim() !== "") {
                return `<>`
            }
            return "";
        },

        renderActionButtons: function(dataId, dataName) { 
            return `
                    <div class="btn-group" role="group">
                        <button class="btn-action btn-action-info view-btn" data-id="${dataId}" data-item-name="${dataName}"
                            title="View Details">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn-action btn-action-info edit-btn" data-id="${dataId}" data-item-name="${dataName}"
                            title="Edit">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn-action btn-action-danger delete-btn" data-id="${dataId}" data-item-name="${dataName}"
                            title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                `;
        }
    };

})(window, jQuery);