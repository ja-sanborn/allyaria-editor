"use strict";

// Global interop helpers for Allyaria.Editor

// Returns the element's innerHTML, used to capture contenteditable changes.
window.Allyaria_Editor_getInnerHtml = function (element) {
    try {
        if (!element) return "";
        return element.innerHTML ?? "";
    } catch {
        return "";
    }
};

// Accepts a space-separated list of element IDs and returns a space-separated list
// of IDs that exist and have non-empty trimmed textContent. Returns "" if none.
window.Allyaria_Editor_sanitizeLabelledBy = function (ids) {
    try {
        if (!ids) return "";
        const tokens = ids.split(/\s+/).filter(t => !!t);
        const valid = [];
        for (const id of tokens) {
            const el = document.getElementById(id);
            if (el) {
                const text = (el.textContent || "").trim();
                if (text.length > 0) valid.push(id);
            }
        }
        return valid.join(" ");
    } catch {
        return "";
    }
};

/**
 * Detects the system theme preference.
 * Returns one of: "hc" (forced/high-contrast), "dark", "light".
 */
window.Allyaria_Editor_detectSystemTheme = function () {
    try {
        const supportsMatch = typeof window.matchMedia === "function";
        if (supportsMatch) {
            // Windows/macOS forced colors High Contrast
            if (window.matchMedia("(forced-colors: active)").matches) {
                return "hc";
            }
            // Dark mode
            if (window.matchMedia("(prefers-color-scheme: dark)").matches) {
                return "dark";
            }
        }
        return "light";
    } catch {
        return "light";
    }
};
