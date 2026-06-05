# Agent guide — MassMediaEdit

Working agreement for **all** coding agents and human contributors working in
this repository. These rules are not optional. The full house spec lives in
the `Hawkynt/project-template` repo (`STANDARD.md`); this file is the
per-repo distillation.

## What this is

A **WinForms batch media editor**: bulk metadata editing, renaming and
organisation. Solution `MassMediaEdit.sln`; the `NfoFileFormat` library
(+ its tests) ships as a NuGet package — a hybrid repo (app + package).

## Commits

- **Group changes semantically/logically** — one concern per commit.
- **Every subject line starts with a prefix**: `+` added · `-` removed ·
  `*` changed · `#` bug fixed · `!` critical todo.
- Never start a subject with "fix"/"bugfix"/"changed"/"modified".
- **No AI traces anywhere**: no `Co-Authored-By` AI lines, no "Generated
  with" footers, no agent mentions in messages, comments, or authorship.

## The loop (always, in this order)

1. **Before committing**: `dotnet build MassMediaEdit.sln -c Release` and
   both test projects until green. UI changes update the `screenshots/`
   captures referenced by the README.
2. **Commit** (rules above) and **push**.
3. **Wait for CI**; on `main` a green CI triggers the nightly (prerelease +
   GFS prune, same-day replace). Fix and loop until everything is green.

Stable releases are **manual** (`gh workflow run release.yml`) — never cut
one unless explicitly asked.

## Code conventions

- Latest C# features; metadata writes are batch operations over user
  files — dry-run/preview paths stay accurate, and nothing touches a file
  the preview didn't list.
- `NfoFileFormat` versions per its own folder (NuGet reuse via
  `--skip-duplicate`); keep package-relevant changes inside that folder.
- Beware filename-case sensitivity on the linux CI leg (pack metadata,
  resource names).

## README & repo conventions

- Standard frame: title → badges → one-line `>` blockquote; fixed emoji
  mapping for the standard sections (`## ✨ Features`, `## 🖼️ Screenshots`,
  `## 📦 Installation`, `## 🚀 Usage`, `## ❤️ Support`, `## 📜 License`);
  `## 🆘 Getting Help` stays distinct from the funding section.
- License is LGPL-3.0-or-later; the `## ❤️ Support` section and
  `.github/FUNDING.yml` stay intact.
