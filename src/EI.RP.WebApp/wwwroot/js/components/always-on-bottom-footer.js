'use strict';
var alwaysOnBottomFooter = {
    bodyEl: document.querySelector('body.with-footer'),
    footerEl: document.querySelector('footer.always-on-bottom'),
    timer: undefined,
    delay: 250,
    UpdatedBodyPadding: function() {
        var that = this;
        clearTimeout(that.timer);
        that.timer = setTimeout(function() {
            that.bodyEl.style.paddingBottom = that.footerEl.clientHeight + 'px';
        }, that.delay);
    },
    BindEvents: function () {
        var that = this;
        window.addEventListener('resize', function() {
            that.UpdatedBodyPadding();
        });
    },
    Init: function () {
        if (this.bodyEl && this.footerEl) {
            this.UpdatedBodyPadding();
            this.BindEvents();
        }
    }
};
document.addEventListener('DOMContentLoaded', function (event) {
    alwaysOnBottomFooter.Init();
}, false);
