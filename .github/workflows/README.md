# CI/CD Pipeline — MassMediaEdit

> Everything in this folder is the automated pipeline. Workflows live here, scripts live in `scripts/`.

## Files

| File                            | Trigger                             | Purpose                                 |
|---------------------------------|-------------------------------------|-----------------------------------------|
| `ci.yml`                        | push + PR + `workflow_call`         | Build + tests + coverage (windows)      |
| `release.yml`                   | tag push `v*`                       | GitHub Release + NuGet push             |
| `nightly.yml`                   | CI success on `master`              | `nightly-YYYY-MM-DD` prerelease + GFS   |
| `_build.yml`                    | `workflow_call` (internal)          | WinForms exe + NuGet build block        |
| `scripts/version.pl`            | invoked by workflows                | `X.Y.Z.BUILD` (csproj scan)             |
| `scripts/update-changelog.mjs`  | invoked by workflows                | Bucketise commits into CHANGELOG.md     |
| `scripts/prune-nightlies.mjs`   | invoked by workflows                | 7+4+3 GFS retention of nightlies        |

## How it works

```
push/PR ──► ci.yml ─────────────────┐
                                    │ on success on master
                                    ▼
  tag v* ──► release.yml ──► _build.yml ──► GH Release + nuget.org
                                    ▲
                                    │
                 workflow_run ──► nightly.yml ──► prerelease + GFS prune
```

## Why

- **No cron triggers.** The old `NewBuild.yml` cut a release on every push to master; that pattern is replaced with proper tag-gated releases plus nightlies for mainline.
- **Tag-gated NuGet push.** Old `NuGet.yml` pushed on any release event; now only real tag releases (`v*`) push to nuget.org.
- **Release calls CI** via `workflow_call` — tests and releases stay in lockstep.
- **Nightly builds the SHA CI validated**, not branch tip.
- **3-generation (GFS) retention**: 7 daily + 4 weekly + 3 monthly.

## Release artifacts

| Artifact                                    | Produced by          | Destination         |
|---------------------------------------------|----------------------|---------------------|
| `MassMediaEdit-win-x64-<version>.zip`       | release + nightly    | GitHub Release      |
| `Hawkynt.NfoFileFormat.<version>.nupkg`     | release only         | GitHub + nuget.org  |
