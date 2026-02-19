// Wal-O-Mat JS interop helpers for Blazor

window.walOMatInterop = {
    /**
     * Selects all text in the given element.
     * Used by ResultsPhase to select the share URL input on click.
     * @param {HTMLElement} element - The DOM element reference from Blazor
     */
    selectAllText: function (element) {
        if (element && typeof element.select === 'function') {
            element.select();
        }
    },

    /**
     * Copies the given text to the clipboard with fallback for older browsers
     * or non-HTTPS contexts where the Clipboard API may not be available.
     * @param {string} text - The text to copy
     * @returns {boolean} true if copy succeeded, false otherwise
     */
    copyToClipboard: async function (text) {
        try {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                await navigator.clipboard.writeText(text);
                return true;
            }
            // Fallback: create a temporary textarea and use execCommand
            var textarea = document.createElement('textarea');
            textarea.value = text;
            textarea.style.position = 'fixed';
            textarea.style.opacity = '0';
            document.body.appendChild(textarea);
            textarea.select();
            var success = document.execCommand('copy');
            document.body.removeChild(textarea);
            return success;
        } catch (err) {
            console.warn('Copy to clipboard failed:', err);
            return false;
        }
    }
};

