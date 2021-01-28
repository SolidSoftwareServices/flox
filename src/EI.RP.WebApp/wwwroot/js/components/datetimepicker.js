
'use strict';

// Tempus Dominus event notification fix
jQuery.fn.datetimepicker.Constructor.prototype._notifyEvent = function _notifyEvent(e) {
    if (e.type === jQuery.fn.datetimepicker.Constructor.Event.CHANGE && 
        (e.date && 
         e.date.isSame(e.oldDate) || 
         !e.date && 
         !e.oldDate))
    {
        return;
    }
    this._element.trigger(e);
};

$(function() {
    if ($('.datetimepicker-input').length) {
        $('.datetimepicker-input').each(function() {
            var $this = $(this);
            $this.datetimepicker();
            $this.on('show.datetimepicker', function(e) {
                $(e.target).next('.bootstrap-datetimepicker-widget').addClass('ei-datepicker show days');
            });
            // handle min/max date validation
            $this.on('change.datetimepicker', function (e) {
                if (e.date && e.date.isValid()) {
                    var input = $(e.target);
                    var errorId = input.data("error-message");
                    var errorDiv = $(errorId);
                    if (errorDiv.length) {
                        input.removeClass("is-invalid");
                        input.attr("aria-invalid", false);
                        errorDiv.find("span").html('');
                        errorDiv.addClass("d-none").attr("aria-hidden", true);
                    }
                }
            });
            // handle min/max date validation
            $this.on('error.datetimepicker', function (e) {
                var input = $(e.target);
                var errorId = input.data("error-message");
                var errorDiv = $(errorId);
                if (errorDiv.length) {
                    var message = 'Invalid date entered. ';
                    var minDate = input.data('date-min-date');
                    var maxDate = input.data('date-max-date');
                    if (minDate && moment(e.date, 'DD/MM/YYYY').isBefore(minDate)) {
                        message += 'The minimum date is ' + moment(minDate).format("DD/MM/YYYY") + ".";
                    }
                    if (maxDate && moment(e.date, 'DD/MM/YYYY').isAfter(maxDate)) {
                        message += 'The maximum date is ' + moment(maxDate).format("DD/MM/YYYY") + ".";
                    }
                    input.addClass("is-invalid");
                    input.attr("aria-invalid", true);
                    errorDiv.find("span").html(message);
                    errorDiv.removeClass("d-none").attr("aria-hidden", false);
                }
            });
        });
    }
});
