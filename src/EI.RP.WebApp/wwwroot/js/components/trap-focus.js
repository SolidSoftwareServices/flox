
/***** DOM ready */

document.addEventListener('DOMContentLoaded', function (event) {

  var focusableElements =
    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])';

  var els = document.querySelectorAll('[data-trap-focus]');

  els.forEach(function(el) {
    var focusableEls = el.querySelectorAll(focusableElements);

    if (el.hasAttribute('data-trap-focus-ignore-hidden')) {
      focusableEls = Array.from(focusableEls).filter(function(element) {
        return element.offsetParent !== null;
      });
    }
    
    var firstFocusableEl = focusableEls[0];
    var lastFocusableEl = focusableEls[focusableEls.length - 1];
    
    el.addEventListener('keydown', function(e) {

      if (el.hasAttribute('data-trap-focus-active') && el.getAttribute('data-trap-focus-active') === 'false') {
        return;
      }

      var isTabPressed = e.key === 'Tab' || e.keyCode === 9;
    
      if (!isTabPressed) {
        return;
      }

      if (e.shiftKey) {
        if (document.activeElement === firstFocusableEl) {
          lastFocusableEl.focus();
          e.preventDefault();
        }
      } else { // if tab key is pressed
        if (document.activeElement === lastFocusableEl) {
          firstFocusableEl.focus();
          e.preventDefault();
        }
      }
    });
  });

}, false);
