(function () {
    'use strict';
    window.tableUtils = {
        debounce: function (callback, delay) {
            var timer;
            return function () {
                var args = arguments, receiver = this;
                clearTimeout(timer);
                timer = setTimeout(function () { callback.apply(receiver, args); }, delay || 200);
            };
        },
        icons: function (root) {
            if (!window.lucide || !root) return;
            root.querySelectorAll('[data-lucide]').forEach(function (element) {
                var name = element.getAttribute('data-lucide').replace(/(^|-)([a-z])/g, function (_, dash, letter) { return letter.toUpperCase(); });
                var definition = lucide.icons[name];
                if (!definition) return;
                var icon = lucide.createElement(definition);
                Array.from(element.attributes).forEach(function (attribute) { icon.setAttribute(attribute.name, attribute.value); });
                icon.removeAttribute('data-lucide');
                element.replaceWith(icon);
            });
        }
    };
})();
