'use strict;'

var subNavigation = {
    helpers: {
        IsSmallScreen: function () {
            return window.innerWidth <= 991;
        },
        RemoveInnerElements: function (el) {
            while (el.lastElementChild) {
                el.removeChild(el.lastElementChild);
            }
        }
    },
    el: {
        wrapper: undefined,
        nav: undefined,
        activeItem: undefined,
        current: undefined,
        overlays: undefined
    },
    offset: {
        wrapper: {
            top: undefined
        }
    },
    ResolveElements: function () {
        this.el.wrapper = document.querySelector('.portal-header__nav');

        if (this.el.wrapper) {
            this.el.nav = this.el.wrapper.querySelector('ul:not(.portal-header__nav__current)');
            this.el.activeItem = this.el.nav.querySelector('.portal-header__nav__item.active');
            this.el.current = this.el.wrapper.querySelector('.portal-header__nav__current');
            this.el.overlays = document.querySelectorAll('.sub-navigation-overlay');
        }
    },
    ResolveActive: function () {
        if (this.el.wrapper && this.el.wrapper.hasAttribute('data-resolve-active') && this.el.activeItem && this.el.current) {
            var clone = this.el.activeItem.cloneNode(true);
            clone.classList.remove('active');
            this.helpers.RemoveInnerElements(this.el.current);
            this.el.current.appendChild(clone);
        }
    },
    BindCurrentClick: function () {
        var that = this;
        this.el.current.addEventListener('click', function (evt) {
            if (!evt.target.matches('a')) return;
            evt.preventDefault();
            that.Toggle();
        }, false);
    },
    BindNavClick: function () {
        var that = this;
        this.el.nav.addEventListener('click', function (evt) {
            if (!evt.target.matches('a')) return;
            that.Close();
        }, false);
    },
    BindOverlayClick: function () {
        var that = this;
        if (this.el.overlays && this.el.overlays.length) {
            this.el.overlays.forEach(function(item) {
                item.addEventListener('click', function (evt) {
                    evt.preventDefault();
                    that.Close();
                }, false);
            });
        }
    },
    BindStickyNav: function() {
        var that = this;
        that.offset.wrapper.top = that.el.wrapper.offsetTop;
        window.addEventListener('scroll', function(e) {
            if (window.pageYOffset >= that.offset.wrapper.top) {
                that.el.wrapper.classList.add('portal-header__nav--sticky');
            }
            else {
                that.el.wrapper.classList.remove('portal-header__nav--sticky');
            }
        });        
    },
    Open: function () {
        if (this.el.wrapper) this.el.wrapper.classList.add('drop');
        if (this.el.overlays && this.el.overlays.length) {
            this.el.overlays.forEach(function(item) { 
                item.classList.add('show');
            });
        }
    },
    Close: function () {
        if (this.el.wrapper) this.el.wrapper.classList.remove('drop');
        if (this.el.overlays && this.el.overlays.length) {
            this.el.overlays.forEach(function(item) { 
                item.classList.remove('show');
            });
        }
    },
    Toggle: function () {
        if (this.el.wrapper) this.el.wrapper.classList.toggle('drop');
        if (this.el.overlays && this.el.overlays.length) {
            this.el.overlays.forEach(function(item) { 
                item.classList.toggle('show');
            });
        }
    },
    Resolve: function () {
        this.ResolveElements();
        this.ResolveActive();
    },
    Bind: function () {
        this.BindCurrentClick();
        this.BindNavClick();
        this.BindOverlayClick();
        this.BindStickyNav();
    },
    Init: function () {
        this.Resolve();
        if (this.el.wrapper) {
            this.Bind();
        }
    }
};


/***** DOM ready */

document.addEventListener('DOMContentLoaded', function (event) {
    subNavigation.Init();
}, false);
