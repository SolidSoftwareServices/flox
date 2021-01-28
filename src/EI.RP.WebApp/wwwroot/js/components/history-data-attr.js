/***** DOM ready */
document.addEventListener('DOMContentLoaded', function (event) {
    var els = document.querySelectorAll('[data-history]');
    els.forEach(function(el) {
        el.addEventListener('click',
            function(e) {
                e.preventDefault();
                var ee = e.target;
                if (ee.dataset.historyBack !== undefined) {
                    window.history.back();
                } else if (ee.dataset.historyForward !== undefined) {
                    window.history.forward();
                } else if (ee.dataset.historyGo !== undefined) {
                    window.history.go(e.dataset.historyGo);
                }
            });
    });
}, false);
