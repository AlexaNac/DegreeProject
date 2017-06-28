(function ($) {
    $(document).ready(function () {

        

        $('table').on('dblclick', 'tr', function (e) {
            e.preventDefault();
            var $p = $(e.delegateTarget);
            var $t = $(e.currentTarget);

            var type = $p.data('type');
            var id = $t.data('id');

            if (!type || !id) return;

            var path = '/__TYPE__/Details/__ID__';

            path = path.replace('__TYPE__', type)
                       .replace('__ID__', id);

            window.location.href = window.location.origin + path;
            });

        //var togglerActiveClass = 'togglerActive';
        //// Profile subscribe toggler
        //var $subscribeForm = $('.newsletterFields');
        //var $subscribeInput = $('#Subscribe');

        //$subscribeInput.on('change', function (e) {
        //    $subscribeForm.toggleClass(togglerActiveClass, this.checked)
        //}).trigger('change');

        //// Profile email toggler
        //// Simultate checkbox toggler for keeping the form visible after submitting with errors
        //var $emailForm = $('.sendOnEmail');
        //var $emailFormTogglerCheckbox = $('#SendEmail');

        //$('.js-emailToggler').on('click', function (e) {
        //    e.preventDefault();
        //    if (!$emailForm.length || !$emailFormTogglerCheckbox.length)
        //        return;

        //    var state = $emailFormTogglerCheckbox.prop('checked');
        //    $emailFormTogglerCheckbox.prop('checked', !state);

        //    $emailForm.toggleClass(togglerActiveClass, $emailFormTogglerCheckbox.checked);
        //});

    });
})(jQuery)