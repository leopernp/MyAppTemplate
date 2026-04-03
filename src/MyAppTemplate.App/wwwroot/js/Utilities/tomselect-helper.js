'use strict';

/**
 * Global TomSelect initialization utility.
 *
 * Usage:
 *   // Static options already in the <select>
 *   window.initTomSelect('#mySelect');
 *
 *   // Remote AJAX options
 *   window.initTomSelect('#mySelect', {
 *       url: '/Role/Inquire',
 *       valueField: 'Id',
 *       labelField: 'Name',
 *       params: { isActive: true }   // optional extra query params
 *   });
 *
 *   // Destroy when no longer needed
 *   window.destroyTomSelect('#mySelect');
 */

(function (window) {

    /**
     * Initialize a TomSelect instance on a <select> element.
     *
     * @param {string|HTMLElement} selector  CSS selector or DOM element
     * @param {Object}             [options] Override / extend defaults
     * @param {string}             [options.url]        Remote URL (enables AJAX mode)
     * @param {string}             [options.valueField] Property to use as option value  (default: 'Id')
     * @param {string}             [options.labelField] Property to use as option label  (default: 'Name')
     * @param {string}             [options.searchField] Property to search on           (default: same as labelField)
     * @param {Object}             [options.params]     Extra query-string params for AJAX requests
     * @returns {TomSelect}
     */
    window.initTomSelect = function (selector, options) {
        options = options || {};

        var el = typeof selector === 'string' ? document.querySelector(selector) : selector;
        if (!el) {
            console.warn('initTomSelect: element not found —', selector);
            return null;
        }

        // Destroy any existing instance so re-init is safe
        if (el.tomselect) {
            el.tomselect.destroy();
        }

        var valueField  = options.valueField  || 'Id';
        var labelField  = options.labelField  || 'Name';
        var searchField = options.searchField || labelField;

        var baseConfig = {
            allowEmptyOption: true,
            placeholder:      el.dataset.placeholder || 'Select…',
            maxOptions:       200,
        };

        if (options.url) {
            // ── AJAX / remote mode ──────────────────────────────
            Object.assign(baseConfig, {
                valueField:  valueField,
                labelField:  labelField,
                searchField: searchField,
                preload:     true,
                load: function (query, callback) {
                    var params = Object.assign({}, options.params || {});
                    if (query) params.search = query;

                    fetch(options.url + '?' + new URLSearchParams(params))
                        .then(function (r) { return r.json(); })
                        .then(function (data) { callback(data); })
                        .catch(function ()   { callback();      });
                },
                // Render each option using labelField
                render: {
                    option: function (item, escape) {
                        return '<div>' + escape(item[labelField]) + '</div>';
                    },
                    item: function (item, escape) {
                        return '<div>' + escape(item[labelField]) + '</div>';
                    }
                }
            });
        }

        // Allow caller to override anything
        var finalConfig = Object.assign(baseConfig, options.config || {});

        return new TomSelect(el, finalConfig);
    };

    /**
     * Destroy a TomSelect instance and restore the original <select>.
     * Safe to call even if no instance exists.
     *
     * @param {string|HTMLElement} selector
     */
    window.destroyTomSelect = function (selector) {
        var el = typeof selector === 'string' ? document.querySelector(selector) : selector;
        if (el && el.tomselect) {
            el.tomselect.destroy();
        }
    };

})(window);
