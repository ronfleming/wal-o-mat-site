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

// Thin wrapper around Application Insights for custom events.
// Safe to call before the SDK loads (no-ops until window.appInsights is set).
// Referrer is captured once per page load, stripped of query string per GDPR choice.
window.walOMatAnalytics = (function () {
    var sessionReferrer = null;

    function initReferrer() {
        try {
            if (!document.referrer) {
                sessionReferrer = '(direct)';
                return;
            }
            var u = new URL(document.referrer);
            if (u.hostname === location.hostname) {
                sessionReferrer = '(internal)';
                return;
            }
            sessionReferrer = u.origin + u.pathname;
        } catch (err) {
            sessionReferrer = '(unknown)';
        }
    }

    function trackEvent(name, props) {
        try {
            if (!window.appInsights || typeof window.appInsights.trackEvent !== 'function') return;
            var merged = Object.assign({
                referrer: sessionReferrer,
                path: location.pathname,
                lang: document.documentElement.lang || 'de'
            }, props || {});
            window.appInsights.trackEvent({ name: name }, merged);
        } catch (err) {
            /* never let analytics break the app */
        }
    }

    function getReferrer() {
        return sessionReferrer;
    }

    return { initReferrer: initReferrer, trackEvent: trackEvent, getReferrer: getReferrer };
})();

