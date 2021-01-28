$(function() {
    $(document).on('click',
        'a[data-dev-tools-toggle]',
        function(e) {
            e.preventDefault();
            var target = $(this).data('dev-tools-toggle-target');
            $(target).toggleClass('open');
        });
})
