
/***** File Download */

var fileDownload = {
    overlay: {
        isLoading: false,
        Show: function(timeoutInMilliseconds) {
            var t = this;
            if (!t.isLoading) {
                document.querySelector('body').classList.add('loading');
                t.isLoading = true;
                if (timeoutInMilliseconds) {
                    setTimeout(function() { t.Hide() }, timeoutInMilliseconds);
                }
            }
        },
        Hide: function() {
            document.querySelector('body').classList.remove('loading');
            this.isLoading = false;
        }
    },
    bind: {
        overlay: undefined,
        Bind: function(overlay) {
            var anchors = document.querySelectorAll('a[data-file-download-promise]');
            anchors.forEach(function(a) {
                a.addEventListener('click', function (e) {
                    e.preventDefault();
                    overlay.Show();
                    $.fileDownload(this.href)
                        .done(function() {
                            overlay.Hide();
                        })
                        .fail(function() {
                            overlay.Hide();
                        });
                    return false;
                }, false);
            });
        },
        Init: function(overlay) {
            this.Bind(overlay);
        }
    },
    Init: function () {
        this.bind.Init(this.overlay);
    }
};


/***** DOM ready */

document.addEventListener('DOMContentLoaded', function (event) {
    fileDownload.Init();
}, false);
