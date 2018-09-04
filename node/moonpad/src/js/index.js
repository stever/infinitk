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

xterm.write('\u001b[33m$ \u001b[0m');

let input = '';

xterm.on('key', (key, ev) => {
    const printable = !ev.altKey && !ev.altGraphKey && !ev.ctrlKey && !ev.metaKey;

    // // Ignore arrow keys
    // if (ev.code === 'ArrowUp' || ev.code === 'ArrowDown' || ev.code === 'ArrowLeft' || ev.code === 'ArrowRight') {
    //     return;
    // }

    if (ev.keyCode === 13) { // RETURN
        xterm.write('\r\n');
        // TODO: process this.input
        input = '';

    } else if (ev.keyCode === 8) { // DELETE
        // TODO: if (xterm.buffer.x > 2)
        if (xterm.buffer || xterm.buffer.x > 2) {
            xterm.write('\b \b');
            input = input.slice(0, -1);
        }

    } else if (printable) {
        console.log(`Printable: ${key}`);
        input += key;
        xterm.write(key);
    }
});
