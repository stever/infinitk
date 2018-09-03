jQuery(function ($, undefined) {
    $('#term').terminal('/api/method/json-rpc', {
        height: '100vh',
        width: '100vw',
        prompt: '> ',
        greetings: ''
    });
});
