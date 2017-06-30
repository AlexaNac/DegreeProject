(function ($) {
    $(document).ready(function () {
        $('table.click').on('dblclick', 'tr', function (e) {
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
        $('.report').on('click', function (e) {
            e.preventDefault();
            var $p = $(e.delegateTarget);
            var $t = $(e.currentTarget);

            var id = $t.data('id');
            var path = '/Chart/CreatePdf/__ID__';

            path = path.replace('__ID__', id);

            window.location.href = window.location.origin + path;
        });
        $('.index').on('click', function(e){
            e.preventDefault();
            var $p = $(e.delegateTarget);
            var $t = $(e.currentTarget);

            var type = $p.data('type');
            var sort = $p.data('sort');
            if (!type) return;

            if (sort)
                var path = '/__TYPE__/Index' + sort;
            else var path = '/__TYPE__/Index';

            path = path.replace('__TYPE__', type)

            window.location.href = window.location.origin + path;
        });
        $('.account').on('click', function(e){
            e.preventDefault();
            var path = '/Manage/Index';
            window.location.href = window.location.origin + path;
        });
        $('.prioritizeButton').on('click', function (e) {
            e.preventDefault();
            debugger;
            var val = $('#prioritizeInput').val();
            var path = '/Task/TaskPrioritization/?period=' + val;
            window.location.href = window.location.origin + path;
        });
    });
})(jQuery)