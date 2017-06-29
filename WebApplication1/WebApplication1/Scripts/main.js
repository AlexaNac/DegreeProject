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
        $('.addNew').on('click', function (e) {
            e.preventDefault();
            var $p = $(e.delegateTarget);
            var $t = $(e.currentTarget);

            var type = $p.data('type');
            if (type == 'Account')
                var path = '/__TYPE__/Register';
            else
                var path = '/__TYPE__/New';
            path = path.replace('__TYPE__', type)
                   
            window.location.href = window.location.origin + path;
        });
        $('.edit').on('click', function (e) {
            e.preventDefault();
            var $p = $(e.delegateTarget);
            var $t = $(e.currentTarget);

            var type = $p.data('type');
            var id = $t.data('id');
            var path = '/__TYPE__/Edit/__ID__';

            path = path.replace('__TYPE__', type)
                       .replace('__ID__', id);

            window.location.href = window.location.origin + path;
        });
    });
})(jQuery)