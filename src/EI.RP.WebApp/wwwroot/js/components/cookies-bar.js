'use strict';
var cookieBar = {
    el: document.querySelector('[data-cookie-bar]'),
    hideEl: document.querySelector('[data-cookie-bar-hide]'),
    IsHidden: function () {
        return Cookies.get('.AspNet.Consent') === 'yes';
    },
    Hide: function () {
        this.el.classList.add('d-none');
    },
    Show: function () {
        this.el.classList.remove('d-none');
    },
    SetStartupVisibility: function () {
        if (!this.IsHidden()) {
            this.Show();
        } else {
            this.Hide();
        }
    },
    BindEvents: function () {
        var that = this;
        that.hideEl.addEventListener('click',
            function (e) {
                var cookieContent = that.hideEl.getAttribute('data-cookie-bar-hide-string');
                document.cookie = cookieContent;
                that.Hide();
            });
    },
    Init: function () {
        this.SetStartupVisibility();
        this.BindEvents();
    }
};
document.addEventListener('DOMContentLoaded', function (event) {
    cookieBar.Init();
}, false);
