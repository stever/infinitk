jQuery(function($, undefined) {
    var isContinued = false;

    var config = {
        height: '100vh',
        width: '100vw',
        prompt: '> ',
        greetings: '',
        name: 'shell',
        onBlur: function() {
            return false;
        }
    };

    var handler = function(command, term) {
        term.pause();
        $.ajax({
            type: 'POST',
            url: '/api/method/luaRepl',
            dataType: 'json',
            data: JSON.stringify({Input: command})
        }).done((data) => {
            var output = data['Output'];
            if (output == null) isContinued = true;
            isContinued = output == null;
            terminal.set_prompt(isContinued ? '  ' : '> ');
            term.echo(output == null ? "" : output).resume();
        });
    };

    var terminal = $('#term').terminal(handler, config);
});
