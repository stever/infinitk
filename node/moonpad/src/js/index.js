import { Terminal } from 'xterm';
import * as fit from 'xterm/lib/addons/fit/fit';

Terminal.applyAddon(fit);

const xterm = new Terminal({
    fontFamily: 'PragmataPro Mono',
    fontSize: 16,
    cursorBlink: true
});

xterm.open(document.getElementById('terminal'));
xterm.fit();

// TODO: Re-fit on resize.

xterm.write('Hello from \x1B[1;3;31mxterm.js\x1B[0m $ ');
