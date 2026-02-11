// Wal-O-Mat JS interop helpers for Blazor

/**
 * Selects all text in the given element.
 * Used by ResultsPhase to select the share URL input on click.
 * @param {HTMLElement} element - The DOM element reference from Blazor
 */
window.walOMatInterop = {
    selectAllText: function (element) {
        if (element && typeof element.select === 'function') {
            element.select();
        }
    }
};

