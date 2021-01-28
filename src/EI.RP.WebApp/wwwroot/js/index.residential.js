'use strict;'

var IsLargeResolution = function() {
  return window.innerWidth > 991;
};

var index = {
  identifyBrowser: {
    Run: function() {
      var body = document.body;
      // Is Chrome or Chrome iOS
      if((/Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor)) || 
        window.navigator.userAgent.match('CriOS')) {
        body.classList.add('chrome');
      }
      // Is Firefox
      else if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
        body.classList.add('firefox');
      }
      // Is IE/legacy Edge
      else if (document.documentMode || /Edge/.test(navigator.userAgent)) {
        body.classList.add('ie');
      }
    },
    Init: function() {
      this.Run();
    }
  },
  setVh: {
    Run: function() {
      var vh = window.innerHeight * 0.01;
      document.documentElement.style.setProperty('--vh', vh + 'px');
    },
    Bind: function() {
      var that = this;
      window.addEventListener('resize', function() {
        that.Run();
      });
    },
    Init: function() {
      this.Run();
      this.Bind();
    }
  },
  objectFit: {
    Run: function() {
      objectFitImages();
    },
    Init: function() {
      this.Run();
    }
  },
  bs: {
    Tooltip: function() {
      $('[data-toggle="tooltip"]').tooltip();
    },
    Popover: function() {
      $('[data-toggle="popover"]').popover({
        trigger: 'focus'
      });
    },
    Init: function() {
      this.Tooltip();
      this.Popover();
    }
  },
  portalMenu: {
    Bind: function() {
      var menu = document.querySelector('.portal-menu');
      if (menu !== null) {
        var profileTrigger = document.querySelector('a.portal-menu__profile__title');
        var profileMenu = document.querySelector('.portal-menu__profile');
        var menuToggle = document.querySelector('.portal-menu__toggle');
        var menuNav = document.querySelector('.portal-menu__nav');
        if (profileMenu !== null && profileTrigger !== null) {
          profileTrigger.addEventListener('click', function(e) {
            if (IsLargeResolution()) {
              e.stopPropagation();
              e.preventDefault();
              profileMenu.classList.toggle('active');
              var expanded = profileTrigger.getAttribute('aria-expanded') === 'true';
              profileTrigger.setAttribute('aria-expanded', !expanded);
            }
          });
          document.addEventListener('click', function(e) {
            if (IsLargeResolution() && !profileMenu.contains(e.target)) {
              profileMenu.classList.remove('active');
              profileTrigger.setAttribute('aria-expanded', false);
            }
          });
        }
        if (menuToggle !== null && menuNav !== null) {
          menuToggle.addEventListener('click', function(e) {
            menuNav.classList.toggle('active');
            e.currentTarget.classList.toggle('close');
            var expanded = e.currentTarget.getAttribute('aria-expanded') === 'true';
            e.currentTarget.setAttribute('aria-expanded', !expanded);
            var trap = e.currentTarget.closest('[data-trap-focus]');
            if (trap !== null) {
              var active = trap.getAttribute('data-trap-focus-active') === 'true';
              trap.setAttribute('data-trap-focus-active', !active);
            }
          });
        }
      }
    },
    Init: function() {
      this.Bind();
    }
  },
  togglePassword: {
    Bind: function() {
      if ($('.toggle-password-visibility').length) {
        $('.toggle-password-visibility').on('click', function(e) {
          e.preventDefault();
          var x = $(this).prev('input')[0];
          if (x.type === 'password') { 
            x.type = 'text';
            $(this).text('hide');
            $(this).attr('aria-label', 'Hide Password');
          } else {
            x.type = 'password';
            $(this).text('show');
            $(this).attr('aria-label', 'Show Password');
          }
        });
      }
    },
    Init: function() {
      this.Bind();
    }
  },
  termsScrollTo: {
    Bind: function() {
      if($('.terms').length) {
        $('.terms__menu a').on('click', function(e) {
          e.preventDefault();
          var href = $(this).attr('href');
          $('html, body').stop().animate( { scrollTop: $(href).offset().top - 150 }, 300, function() { 
            window.location.hash = href;
          });
        });
      }
    },
    Init: function() {
      this.Bind();
    }
  },
  Init: function() {
    this.identifyBrowser.Init();
    this.setVh.Init();
    this.objectFit.Init();
    this.bs.Init();
    this.portalMenu.Init();
    this.togglePassword.Init();
    this.termsScrollTo.Init();
  }
};

document.addEventListener('DOMContentLoaded', function (event) {
  index.Init();
}, false);
