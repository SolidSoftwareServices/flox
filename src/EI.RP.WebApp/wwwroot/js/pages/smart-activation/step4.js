'use strict;'
/***** DOM ready */
document.addEventListener('DOMContentLoaded', function (event) {

    var container = document.querySelector('[data-day-of-month-container]');
    var triggers = document.querySelectorAll('[data-day-of-month-visible]');

    function ResolveDayOfMonthContainerVisibility(el, container) {
        if (el.checked) {
            if (el.dataset.dayOfMonthVisible.toLowerCase() === 'true') {
                container.classList.remove('d-none');
            }
            else {
                container.classList.add('d-none');
            }
        }
    }

    function Bind() {
        triggers.forEach(function(el) {
            el.addEventListener('change',
                function(e) {
                    ResolveDayOfMonthContainerVisibility(e.target, container);
                });
        });
    }

    triggers.forEach(function(el) {
        ResolveDayOfMonthContainerVisibility(el, container);
    });

    Bind();

}, false);
