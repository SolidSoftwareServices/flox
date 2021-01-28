// Checking if the elment is on screen
$.fn.isOnScreen = function() {
  var win = $(window);
  var viewport = {
      top: win.scrollTop(),
      left: win.scrollLeft()
  };
  viewport.right = viewport.left + win.width();
  viewport.bottom = viewport.top + win.height();
  var bounds = this.offset();
  if(bounds) {
      bounds.right = bounds.left + this.outerWidth() ;
      bounds.bottom = bounds.top + this.outerHeight() + 100;
      return (!(viewport.right < bounds.left || viewport.left > bounds.right || viewport.bottom < bounds.top || viewport.top > bounds.bottom));
  } else {
      return true;
  }
};

function ordinal_suffix_of(i) {
  var j = i % 10,
      k = i % 100;
  if (j == 1 && k != 11) {
      return i + "st";
  }
  if (j == 2 && k != 12) {
      return i + "nd";
  }
  if (j == 3 && k != 13) {
      return i + "rd";
  }
  return i + "th";
}

$(document).ready(function() {

  objectFitImages();

  var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
  var isIOSChrome = window.navigator.userAgent.match("CriOS");
  if(isChrome || isIOSChrome) {
    $('body').addClass('chrome');
  }
  var isFirefox = navigator.userAgent.toLowerCase().indexOf('firefox') > -1;
  if(isFirefox) {
    $('body').addClass('firefox');
  }
  if (document.documentMode || /Edge/.test(navigator.userAgent)) {
    $('body').addClass('ie');
  }

  var vh = window.innerHeight * 0.01;
  document.documentElement.style.setProperty('--vh', vh + 'px');

  window.addEventListener('resize', function() {
    var vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty('--vh', vh + 'px');
  });

  // options
  $("[role=options]").find("[role=button]").click(function() {
    $(this).parent().find("[role=button]").removeClass("active").attr('aria-pressed', 'false');
    $(this).addClass("active").attr('aria-pressed', 'true');
  });

  // portal menu --------------------------------------------

  if($('.portal-menu').length) {
    $('.portal-menu__profile__title').on('click', function(e) {
      if(window.innerWidth > 991) {
        e.stopPropagation();
        e.preventDefault();

        $('.portal-menu__profile').toggleClass('active');
      }
    });

    $(document).click(function() {
      if(window.innerWidth > 991) {
        $('.portal-menu__profile').removeClass('active');
      }
    });
    $('.portal-menu__toggle').on('click', function(e) {
      $('.portal-menu__nav').toggleClass('active');
      $(this).toggleClass('close');
      });
      $(".portal-menu__profile__drop").keydown(function (e) {
          if (e.keyCode == 27) {
              if (window.innerWidth > 991) {
                  $('.portal-menu__profile').removeClass('active');
              }
          }
      });
  }

  // copy img to bg img
  $(".img-as-bg").each(function() {
    var img = $(this).find("img");
    img.css('visibility', 'hidden');
    $(this).css('background-image', 'url(' + img.attr("src") + ')');
  });

  // form payment -------------------------------------------

  if($('.choose-card').length) {
    $('.form-check input').on('change', function() {
      if($(this).prop('checked')) {
        $(this).parents('.form-check').addClass('checked');
      } else {
        $(this).parents('.form-check').removeClass('checked');
      }

      $('.form-check input[type="radio"]').each(function() {
        if(!$(this).prop('checked')) {
          $(this).parents('.form-check').removeClass('checked');
        }
      });
    });
  }

  // cookies --------------------------------------------------
  if($('.cookies').length) {
    $('.cookies__close').on('click', function(e) {
      e.preventDefault();

      $('.cookies').remove();
    });
  }

  // close icon ----------------------------------------------
  $(".has-close input")
    .on("input propertychange", function() {
      var $this = $(this);
      var visible = Boolean($this.val());
      $this.siblings(".close").toggleClass("d-none", !visible);
    })
    .trigger("propertychange");

  $(".close").click(function() {
    $(this)
      .siblings("input")
      .val("")
      .trigger("propertychange")
      .focus();
  });

  // button loading ------------------------------------------
  $(".btn-loading").on("click", function() {
    var $this = $(this);
    var loadingText = $this.data("loading-text");

    $this.data("original-text", $(this).html());
    $this.html(loadingText);

    setTimeout(function() {
      $this.html($this.data("original-text"));
    }, 2000);
  });

  // tooltip -------------------------------------------------

  var is_touch_device = ("ontouchstart" in window) || window.DocumentTouch && document instanceof DocumentTouch;
  $('[data-toggle="tooltip"]').tooltip({
      trigger: is_touch_device ? "click" : "hover"
  });


  // popover -------------------------------------------------
  $('[data-toggle="popover"]').popover({
    trigger: "focus"
  });

  // alert
  $('.alert').alert();

  // file input ----------------------------------------------
  $(".custom-file-input").on("change", function() {
    var fileName = $(this).val();
    $(this)
      .next()
      .next(".file-name")
      .html(fileName);
  });

  $(".date-wrapper .form-control").each(function () {
    var l = $(this).attr('placeholder').length;
    if(window.innerWidth <= 1024 && l == 2) {
      l = 3;
    }
    $(this).attr('size', l);
  });



  // video -------------------------------------------------
  if (document.getElementById("video-banner")) {
    var playBtn = document.getElementById('watchVideo');
    var videoModal = document.getElementById('videoModal');
    var closeBtn = document.getElementById('closeVideo');
    var seekBar = document.querySelectorAll('.timeline .bg')[0];
    var play_pause_btn = document.getElementById('playpause');
    var fullScreenBtn = document.getElementById('fullscreen');
    var videoDuration = 0;
    var timeCount = 0;
    var completed = 0;
    var videoInterval;
    playBtn.addEventListener('click', function (e) {
        e.preventDefault();
        var bodyTag = document.getElementById('video-banner');
        bodyTag.classList.toggle('video-player-active');
        //videoBg.pause();
        //videoModal.currentTime = 0;
        play_pause_btn.classList.remove('paused');
        videoDuration = videoModal.duration;
        videoModal.play();
    });
    closeBtn.addEventListener('click', function (e) {
        e.preventDefault();
        var bodyTag = document.getElementById('video-banner');
        bodyTag.classList.toggle('video-player-active');
        videoModal.pause();
        clearInterval(videoInterval);
    });
    seekBar.addEventListener('click', function (event) {
        var xpos = event.layerX;
        var percent = xpos / seekBar.offsetWidth;
        var cTime = Math.ceil((percent) * videoDuration);
        videoModal.currentTime = cTime;
        timeCount = cTime * 1000;
        videoModal.play();
        document.querySelectorAll('.timeline .control')[0].style.left = (percent * 100) + '%';
    });
    fullScreenBtn.addEventListener('click', function (e) {
        e.preventDefault();
        goFullscreen('videoModal');
    });
    play_pause_btn.addEventListener('click', function (e) {
        e.preventDefault();
        if (videoModal.paused) {
            videoModal.play();
        } else {
            clearInterval(videoInterval);
            videoModal.pause();
        }
        this.classList.toggle('paused');
    });
    videoModal.onplay = function () {
        videoInterval = setInterval(function () {
            timeCount += 50;
            completed = ((timeCount / 1000) / videoDuration) * 100;
            if (completed > 100) {
                completed = 100;
            }
            document.querySelectorAll('.timeline .control')[0].style.left = completed + '%';
        }, 50);
    };
    videoModal.onended = function () {
        timeCount = 0;
        completed = 0;
        play_pause_btn.classList.toggle('paused');
        clearInterval(videoInterval);
    };
  }

  function goFullscreen(id) {
    var element = document.getElementById(id);
    if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.webkitRequestFullScreen) {
        element.webkitRequestFullScreen();
    }
  }

  // carousel -------------------------------------------------
  if ($(".carousel").length) {
    $(".carousel").each(function() {
      var total = $(this).find(".carousel-item").length;

      $(this)
        .find(".carousel-nbs .total")
        .text(("0" + parseInt(total)).slice(-2));

      $(this).on("slide.bs.carousel", function() {
        $(this)
          .find(".carousel-caption")
          .removeClass("active");
      });

      $(this).on("slid.bs.carousel", function() {
        var index = $(this)
          .find(".carousel-item.active")
          .index();
        $(this)
          .find(".carousel-nbs .current")
          .text(("0" + parseInt(index + 1)).slice(-2));

        $(this)
          .find(".carousel-caption")
          .eq(index)
          .addClass("active");

        if (
          $(this)
            .find(".carousel-item")
            .first()
            .hasClass("active")
        ) {
          $(this)
            .children(".carousel-control-prev")
            .addClass("disabled");
          $(this)
            .children(".carousel-control-next")
            .removeClass("disabled");
        } else if (
          $(this)
            .find(".carousel-item")
            .last()
            .hasClass("active")
        ) {
          $(this)
            .children(".carousel-control-prev")
            .removeClass("disabled");
          $(this)
            .children(".carousel-control-next")
            .addClass("disabled");
        } else {
          $(this)
            .children("a")
            .removeClass("disabled");
        }
      });
    });
  }




  if($('.homepage').length || $('.help').length) {
    $('.homepage__head').addClass('animate');

    // $('.block-feature, .block-image').each(function() {
    //   if($(this).isOnScreen()) {
    //     $(this).addClass('animate');
    //   }
    // });

    if(window.innerWidth > 768) {
      $('.plans__box').hover(function() {
        if(!$(this).hasClass('active')) $('.plans__box.active').addClass('fade--out');
      }, function() {
        $('.plans__box.active').removeClass('fade--out');
      });
    }

    // $(document).on('scroll', function() {
    //   $('.block-feature, .block-image').each(function() {
    //     if($(this).isOnScreen()) {
    //       $(this).addClass('animate');
    //     }
    //   });
    // });
  }

  if($('.price-plans').length) {

    if($('.smart-meter').length) {
      if(window.innerWidth > 768) {
        $('.plans__box').hover(function() {
          $('.plans__box').not($(this)).addClass('fade--out');

        }, function() {
          $('.plans__box').removeClass('fade--out');
        });
      }
    }

    if($('.good-signal').length) {

      var collapsed = false;
      var h = $('.plan-details').first().height();
      $('.plan-details').eq(1).height(h);

      $('.plan-details a').on('click', function(e) {
        if(collapsed) {
          $('.plan-details a').text('Hide details');
          $('.collapse').collapse('show');
          collapsed = false;

          setTimeout(function() {
            var h = $('.plan-details').first().height();
            $('.plan-details').eq(1).height(h);
          }, 300);
        } else {
          $('.plan-details a').text('Show details');
          $('.plan-details').css('height', '');
          $('.collapse').collapse('hide');
          collapsed = true;
        }
      });
    }

    var myElement = document.getElementById('content');
    // create a simple instance
    // by default, it only adds horizontal recognizers
    var mc = new Hammer(myElement);
    // listen to events...
    mc.on("swipeleft swiperight", function (ev) {
        //get target element
        var angle = Math.abs(ev.angle);
        if (angle >= 90 && angle < 150)
            return;

        if (angle > 30 && angle < 90)
            return;
        var obj = (ev.target ? ev.target : ev.srcEvent);
        //if target is carousel then proceed further
        var activeTabId = $(obj).parents('.price-plans').attr('data-active-id');
        if (activeTabId !== undefined) {
            var nextId = '';
            if (ev.type === "swipeleft") {
                if(activeTabId === '#box1'){
                    nextId = '#box2';
                }else if(activeTabId === '#box2'){
                  if($('#box3').length) {
                    nextId = '#box3';
                  } else {
                    nextId = '#box1';
                  }
                }else if(activeTabId === '#box3') {
                    nextId = '#box1';
                }
            }
            if (ev.type === "swiperight") {
                if(activeTabId === '#box1'){
                  if($('#box3').length) {
                    nextId = '#box3';
                  } else {
                    nextId = '#box2';
                  }
                }else if(activeTabId === '#box2'){
                    nextId = '#box1';
                }else if(activeTabId === '#box3') {
                    nextId = '#box2';
                }
            }
            if(nextId !== ''){
                $('.price-plans').attr('data-active-id', nextId);
                $('.tab-links a').removeClass('active');
                $('a[href="' + nextId+'"]').addClass('active');
            }
        }
    });


    var planLinks = document.querySelectorAll('.price-plans .tab-links a');
    for (var c = 0; c < planLinks.length; c++) {
        planLinks[c].addEventListener('click', function (event) {
            event.preventDefault();
            var thisHref = this.getAttribute('href');
            document.querySelectorAll('.price-plans')[0].setAttribute('data-active-id', thisHref);
            document.querySelectorAll('.price-plans .tab-links a.active')[0].classList.remove('active');
            this.classList.add('active');
            // document.querySelectorAll('.customer-tabs .nav-tabs li a[href="#new-customer"]')[0].parentNode.classList.add('active');
        });
    }
  }

  if($('.toggle-password-visibility').length) {
    $('.toggle-password-visibility').on('click', function(e) {
      e.preventDefault();

      var x = $(this).prev('input')[0];
      if (x.type === "password") {
        x.type = "text";
        $(this).text('hide');
      } else {
        x.type = "password";
        $(this).text('show');
      }
    });
  }

  // account validation
  function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
  }


  $('input[type="email"][required]').on('keyup blur', function() {
    var v = $(this).val();

    if(v == '' || validateEmail(v) == false) {
      $(this).addClass('is-invalid');
      $(this).parent().find('.alert').removeClass('d-none');
    } else {
      $(this).removeClass('is-invalid');
      $(this).parent().find('.alert').addClass('d-none');
    }
  });

  $('input[type="text"][required], input[type="phone"][required], input[type="password"][required], textarea[required]').on('keyup blur', function() {
    var v = $(this).val();

    if(v == '') {
      $(this).addClass('is-invalid');
      $(this).parent().find('.alert').removeClass('d-none');
    } else {
      $(this).removeClass('is-invalid');
      $(this).parent().find('.alert').addClass('d-none');
    }
  });


  // authorise
  if($('.page-authorise').length || $('.smart-meter--small_header').length) {
    $('.form-check').on('click', function(e) {
      if($(e.target).hasClass('form-check')) {
        $(e.target).find('input').trigger('click');
      }
    });

    $('.form-check input[type="radio"]').each(function() {
      if(!$(this).prop('checked')) {
        $(this).parents('.form-check').removeClass('checked');
      }
    });

    $('.form-radio').removeClass('fade-out');
    $('.form-radio').not('.checked').addClass('fade-out');

    if($('.billing').length) {
      var t = $(this).data('text');
      $('.btn-page-submit').text(t);
    }
  }

  if($('.moving-form').length) {
    $('input[type=radio][name=radioName]').change(function() {
      if($(this).val() == 'yes') {
        $('.form-toggle').removeClass('d-none');
      } else {
        $('.form-toggle').addClass('d-none');
      }
    });
  }

  if($('.page-loading').length) {
    var count = 0;
    var counting = setInterval(function(){
      if(count < 101) {
        $('.progress-status span').text(count);
        $('.progress-bar').width(count + '%');
        $('.progress-bar').prop('aria-valuenow', count);

        if(count > 95) {
          $('.progress-bar').addClass('loaded');
        }
        count++;
      } else {
        clearInterval(counting);
      }
    }, 20);
  }


  if($('.terms').length) {
    $('.terms__menu a').on('click', function(e) {
      e.preventDefault();

      var href = $(this).attr('href');
      $("html, body").stop().animate( { scrollTop: $(href).offset().top - 150 }, 300);

    });
  }


});

