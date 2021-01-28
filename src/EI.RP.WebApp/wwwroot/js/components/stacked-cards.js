'use strict';

document.addEventListener('DOMContentLoaded', function (event) {

    var diff = {
        x: 10,
        y: 10,
        scale: 0.015,
        zIndex: 1,
        opacity: 0.2
    };

    var t = {
        timer: undefined,
        delay: 150
    };

    var stacked = document.querySelectorAll('[data-stacked-cards]');

    var updateHeight = function() {
        stacked.forEach(function(el) {
            var maxHeight = 0;
            var cards = el.querySelectorAll('[data-stacked-card]');

            el.style.height = null;
            cards.forEach(function(e) {
                e.style.height = null;
            });

            cards.forEach(function(e) {
                if (maxHeight < e.clientHeight) {
                    maxHeight = e.clientHeight;
                }
            });

            el.style.height = maxHeight + 'px';
            cards.forEach(function(e) {
                e.style.height = maxHeight + 'px';
            });
        });
    }

    var updateHeightDebounced = function() {
        clearTimeout(t.timer);
        t.timer = setTimeout(function() {
            updateHeight();
        }, t.delay);
    }

    stacked.forEach(function(el) {
        var cards = el.querySelectorAll('[data-stacked-card]');
        var nextEl = el.querySelector('[data-stacked-cards-next]');

        if (cards.length === 0) {
            el.remove();
        }

        var indexes = [];

        el.style.width = 'calc(100% - ' + (cards.length - 1) * diff.x + 'px)';
        el.style.marginBottom = 'calc(4rem + ' + (cards.length - 1) * diff.y + 'px)';

        for(var i = 0; i < cards.length; i++) {
            indexes.push(i);
        }

		var updateCards = function(arr) {
			for(var i = 0; i < cards.length; i++) {
                var m = arr[i];
                var card = cards[i];

                card.dataset.slide = arr[i];
                card.style.transform = 'translate3d(' + diff.x * m + 'px, ' + diff.y * m + 'px, 0)';
                card.style.zIndex = cards.length - diff.zIndex * (m + 1);
                card.style.opacity = m === 0 ? 1 : (1 - (1 / cards.length) * m);

                if (card.style.zIndex === 0) {
                    card.style.zIndex = null;
                }
			}
        };

        nextEl.addEventListener('click', function () {
			indexes.unshift(indexes.pop());
			updateCards(indexes);
            var focusOn = cards[indexes[0]].querySelector('[data-stacked-card-focus]');
            if (focusOn) {
                focusOn.focus();
            }
        });

        var images = el.querySelectorAll('[data-stacked-card] img');
        images.forEach(function(e) {
            e.addEventListener("load", function() {
                updateHeightDebounced();
            });
        });

        if (cards.length > 1) {
            nextEl.classList.remove('d-none');
        }

        updateCards(indexes);
    });

    updateHeight();

    window.addEventListener('resize', function() {
        updateHeightDebounced();
    });

}, false);

$(function() {
    if ($('[data-dismiss-notification]').length) {
        $('[data-dismiss-notification]').on('click', function(e) {
            e.preventDefault();
            var el = $(this);
            if (el.data('dismiss-notification') !== undefined) {
                loadingOverlay.overlay.Show();
                var endpoint = el.data('dismiss-notification-endpoint');
                $.get(endpoint, function() {
                    location.reload(true);
                }).fail(function() {
                    loadingOverlay.overlay.Hide();
                });
            }
        });
    }
})