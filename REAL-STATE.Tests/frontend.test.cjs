'use strict';
const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const scripts = path.resolve(__dirname, '../REAL-STATE/wwwroot/js');
const lucide = require('../REAL-STATE/wwwroot/lib/lucide/lucide.min.js');
function utilities() {
    const context = { window: { lucide }, lucide, clearTimeout, setTimeout };
    vm.createContext(context);
    vm.runInContext(fs.readFileSync(path.join(scripts, 'table-utils.js'), 'utf8'), context);
    return context.window.tableUtils;
}

test('application JavaScript parses successfully', () => {
    for (const file of fs.readdirSync(scripts).filter(file => file.endsWith('.js')))
        new vm.Script(fs.readFileSync(path.join(scripts, file), 'utf8'), { filename: file });
});

test('scoped icons use the pinned Lucide API and preserve accessibility attributes', () => {
    const makeNode = name => ({ name, attributes: {}, children: [],
        setAttribute(key, value) { this.attributes[key] = value; },
        removeAttribute(key) { delete this.attributes[key]; },
        appendChild(child) { this.children.push(child); }
    });
    const originalDocument = global.document;
    global.document = { createElementNS: (_, name) => makeNode(name) };
    try {
        let rendered;
        const icon = {
            attributes: [{ name: 'class', value: 'size-4' }, { name: 'aria-hidden', value: 'true' }, { name: 'data-lucide', value: 'check-circle' }],
            getAttribute: () => 'check-circle', replaceWith: node => { rendered = node; }
        };
        utilities().icons({ querySelectorAll: () => [icon] });
        assert.equal(rendered.name, 'svg');
        assert.equal(rendered.attributes.class, 'size-4');
        assert.equal(rendered.attributes['aria-hidden'], 'true');
        assert.equal(rendered.attributes['data-lucide'], undefined);
    } finally { global.document = originalDocument; }
});

test('debounced searches run once using the final input and original receiver', async () => {
    const calls = [], receiver = { search: true };
    const callback = utilities().debounce(function (value) { calls.push({ receiver: this, value }); }, 10);
    callback.call(receiver, 'a'); callback.call(receiver, 'ab'); callback.call(receiver, 'abc');
    await new Promise(resolve => setTimeout(resolve, 50));
    assert.deepEqual(calls, [{ receiver, value: 'abc' }]);
});
