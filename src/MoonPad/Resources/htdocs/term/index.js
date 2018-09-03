jQuery(function($, undefined) {
    var isContinued = false;

    var config = {
        height: '100vh',
        width: '100vw',
        prompt: '> ',
        greetings: '',
        prompt: function(callback) {
            callback(isContinued ? '>> ' : '> ');
        },
        name: 'shell',
        onBlur: function() {
            return false;
        }
    };

    var handler = function(command, term) {
        $.ajax({
            type: 'POST',
            url: '/api/method/luaRepl',
            dataType: 'json',
            data: JSON.stringify({Input: command})
        }).done((data) => {
            term.echo(data['Output']);
        });
    };

    $('#term').terminal(handler, config);
});
