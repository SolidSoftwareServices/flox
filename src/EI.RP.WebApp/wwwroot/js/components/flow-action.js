
/***** Flow Action */

var flowAction = {
    Bind: function(overlay) {
        var anchors = document.querySelectorAll('a[data-trigger-event]');
        anchors.forEach(function(a) {
            a.addEventListener('click', function (e) {
                e.preventDefault();

                var el = e.currentTarget;
                var eventFieldName = el.dataset.eventFieldName;
                var triggerEvent = el.dataset.triggerEvent;
                var form = el.closest('form');

                if (!form.hasAttribute('data-ignore-loading-overlay')) {
                    loadingOverlay.overlay.Show();
                }

                form.querySelector('[name="' + eventFieldName + '"]').value = triggerEvent;
                form.submit();

            }, false);
        });
    },
    Init: function () {
        this.Bind();
    }
};


/***** DOM ready */

document.addEventListener('DOMContentLoaded', function (event) {
    flowAction.Init();
}, false);
