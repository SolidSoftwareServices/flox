'use strict;'

var validations = {
    describedBy: {
        Run: function() {
            var errors = document.querySelectorAll('[data-valmsg-for]');
            errors.forEach(function(errorEl) {
                var fieldName = errorEl.getAttribute('data-valmsg-for');
                var fieldEl = document.querySelector('[name="' + fieldName + '"]');
                var errorId = errorEl.getAttribute('id');
                if (!errorId) {
                    errorId = fieldName + '-error';
                    errorEl.setAttribute('id', errorId);
                }
                fieldEl.setAttribute('aria-describedby', errorId);
            });
        },
        Init: function() {
            this.Run();
        }
    },
    focusOnErrors: {
        Run: function() {
            var firstError = document.querySelector('.field-validation-error');
            if (firstError) {
                var msgFor = firstError.dataset.valmsgFor;
                if (msgFor) {
                    document.querySelector('[name="' + msgFor + '"]').focus();
                }
            }
        },
        Init: function() {
            this.Run();
        }
    },
    Init: function() {
        this.describedBy.Init();
        this.focusOnErrors.Init();
    }
};


/***** DOM ready */

document.addEventListener('DOMContentLoaded', function (event) {
    validations.Init();
}, false);
