
document.addEventListener("DOMContentLoaded", function (event) {
    var form = document.getElementById('paymentForm');
    if (form) {
        form.submit();
    }
});

window.addEventListener("message", function (event) {
    if (event.data) {

        var data = typeof event.data === 'object' && event.data !== null
            ? event.data
            : JSON.parse(event.data);

        if (data.iframe) {
            var iframe = document.querySelector(".elavon-container iframe");
            iframe.style.width = data.iframe.width;
            iframe.style.height = data.iframe.height;
        }

    }
});
