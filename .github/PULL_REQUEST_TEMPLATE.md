# Pull Request

## Description

<!-- Provide a clear and concise description of the changes. Why are they needed? -->

Fixes # (issue)

---

## Type of Change

<!-- Put an `x` in all the boxes that apply. -->

- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Refactoring / code cleanup
- [ ] Other (please describe):

---

## Standards Checklist

Please confirm the following before requesting review:

- [ ] **C# standards** followed (see [`docs/standards/CSharp.md`](../docs/standards/CSharp.md))
- [ ] **Razor standards** followed (see [`docs/standards/Razor.md`](../docs/standards/Razor.md))
- [ ] **SCSS standards** followed (see [`docs/standards/SCSS.md`](../docs/standards/SCSS.md))
- [ ] **JavaScript standards** followed (if applicable) (see [
  `docs/standards/JavaScript.md`](../docs/standards/JavaScript.md))
- [ ] **Accessibility standards** followed (see [`docs/standards/Accessibility.md`](../docs/standards/Accessibility.md))
- [ ] **Localization standards** followed (see [`docs/standards/Localization.md`](../docs/standards/Localization.md))
- [ ] No hardcoded strings (all user-facing text is localizable)
- [ ] All async methods end with `Async` and accept `CancellationToken`
- [ ] Disposal is handled for long-lived resources (JS interop, timers, subscriptions, etc.)

---

## Testing

<!-- Describe how you tested your changes and how reviewers can reproduce. -->

- [ ] Unit tests added/updated
- [ ] Component tests (bUnit) added/updated
- [ ] Accessibility verified (keyboard navigation, screen reader, contrast, focus)
- [ ] Localization verified (non-English, RTL if applicable)

---

## Additional Notes

<!-- Add any additional context, screenshots, or considerations for reviewers. -->
