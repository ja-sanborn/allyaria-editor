"use strict";

// Component-scoped interop for AllyariaToolbar

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
