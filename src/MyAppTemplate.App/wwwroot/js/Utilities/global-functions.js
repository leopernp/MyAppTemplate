(function() {
  /**
   * Attaches the translation utility to the global window object.
   */
  window.translateToLocal = function(utcDateTime, format = 'YYYY-MM-DD HH:mm:ss') {
    if (!utcDateTime) return 'Invalid Date';

    // moment.utc() parses the date as UTC
    // .local() converts it to the user's system timezone
    return moment.utc(utcDateTime).local().format(format);
  };
})();