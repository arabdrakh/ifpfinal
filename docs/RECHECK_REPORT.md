# MoodLog — second independent source audit

Review date: **2026-11-14**  
Source: `ifpfinal-rechecked-2026-11-14.zip` (the revised project supplied for this verification).  
Scope: existing Blazor/.NET solution and all 20 criteria in `Final Requirements CSh(3).docx`.

## Verification status — read this first

- **Completed:** source review of architecture, models, repositories, services, Razor pages, GitHub Actions, unit/integration/E2E test sources, documentation, CSS, and the 12-slide deck; static checks for 9 routes, XML project files, workflow YAML, CSS grammar, screenshots, slide structure, file structure, and ZIP integrity.
- **Executed on Windows:** .NET SDK 10.0.403; restore; Release build (0 warnings, 0 errors); unit/integration tests (55/55); Playwright E2E tests (5/5); Chromium installation; EF migration discovery; and `migrations has-pending-model-changes` (no pending changes). E2E used a temporary SQLite database.
- **Not independently verified:** team members' programming contributions, actual defense quality, live UX, performance/CPU/memory metrics, test coverage percentage, results of cross-browser/cross-device manual checks, and real suitability for public deployment.
- **Important:** A perfect grade is not verifiable from static code; each criterion is worth 5% and some require a working app, measured evidence, and a live defense.

## Additional issues found and changes made in this audit

1. Browser E2E could interact before Blazor server finished connecting. The original layout now signals interactive readiness in `MainLayout.razor`; E2E waits for it before interacting.
2. CSV E2E previously asserted only a success status, not the downloaded file. It now captures the download, reads its contents, and verifies 30 data rows plus header.
3. CRUD browser coverage now checks edits and deletions of **all four** trackers. Water goal regression explicitly changes the profile goal after a water entry already exists and checks results across pages.
4. Deletion `confirm()` calls previously happened outside exception handling and could produce unhandled JS interop errors on disconnect. They are now within existing page `try/catch` blocks.
5. Dashboard failures previously logged an exception but presented a misleading empty-looking dashboard. It now shows a readable error alert.
6. Mood tags had no individual length limit; the domain now rejects tags longer than 40 characters, and the UI describes this constraint.
7. Updated test runner packages to supported recent stable versions; GitHub Actions now also uploads unit/integration coverage reports (these packages and workflows still must be actually restored and run).
8. Added `scripts/VERIFY_WINDOWS.ps1` to run the build, automated tests, and browser installation in sequence and fail on a non-zero exit code.
9. Corrected the archive instructions: the source code in this ZIP may differ from the public repository's default branch.
10. Added the EF-generated `MoodLogDbContextModelSnapshot.cs` from the current runtime model. The EF tooling reports no pending model changes; no schema migration was added.
11. Made the E2E fixture launch the same build configuration as the test assembly and wait for the edit form to finish rendering before filling it, avoiding stale Debug binaries and Blazor event races.

## Rubric-to-evidence matrix

| # | Criterion (5% each) | Evidence in revised project | Remaining gate for 5/5 |
|---|---|---|---|
| 1 | Code structure & design | Core / Infrastructure / Web; repository abstractions; records | Full clean Release build; architectural review |
| 2 | Component reusability | `ChartPanel`, `EntryActions` with parameters and callbacks | Live component behavior |
| 3 | Cross-platform | Browser UI and three-OS GitHub Actions matrix | Passing Windows/macOS/Linux runs and device checks |
| 4 | State management | Scoped `WellnessState`, subscriptions and disposal | Verify page updates on live interactive circuit |
| 5 | UI design | Consistent styles, existing screenshots, contrast/focus improvements | Updated screenshots and independent UI evaluation |
| 6 | UX | CRUD, cancel/edit/delete, validation, feedback | E2E and keyboard/manual testing |
| 7 | Responsiveness | Responsive grids and media queries in `app.css` | 320/375/768/1024/1440 px and zoom checks |
| 8 | Core features | Mood, Water, Sleep, Activity, Profile, Analytics, CSV | Run all workflows and persistence tests |
| 9 | Advanced features | Deterministic insights, cross-page state, charts, CSV | Show non-trivial features and explain trade-offs |
| 10 | Error handling | Typed errors, UI try/catch, logger, disabled buttons | Failure-injection scenarios |
| 11 | Efficiency | Date-filtered EF queries and indexes; lightweight visualizations | Timing and load measurements |
| 12 | Scalability | Layered design, repositories, factory DbContexts | Document bounded single-user scope and load tests |
| 13 | Resource utilization | Limited data ranges, no network integrations, short DB contexts | CPU/RAM/network measurements |
| 14 | Testing coverage | xUnit model/service/SQLite tests; 5 Playwright scenarios; coverage CI collection | Actual passing runs and numeric coverage |
| 15 | Quality assurance | Manual QA checklist in `docs/QA_CHECKLIST.md` | Execute/check off each supported device/browser scenario |
| 16 | Documentation | README, architecture, defense guide, release notes, QA, this report | Verify setup instructions on target computer |
| 17 | Innovation | Private integrated habit tracking and transparent deterministic insights | Defend originality compared with existing apps |
| 18 | Creative FP/.NET | `Result`, `Option`, LINQ, records, Blazor events, EF Core | Demonstrate and explain semantics during defense |
| 19 | Professionalism | Documentation, CI, test structure, slide deck and graceful failures | Real passing build, updated screenshots, final demo |
| 20 | Presentation | Editable 12-slide deck, speaking notes and live demo plan | 15–20-minute two-person defense; each knows the code |

## Outstanding risks / limits

- **Cross-platform/manual QA:** The automated suite passed on Windows only. Run the CI matrix and complete `docs/QA_CHECKLIST.md` on supported browsers and devices before claiming cross-platform/manual verification.
- **Migrations:** The three schema migrations are manually authored; the conventional model snapshot is now generated by EF Core 10.0.14 from the runtime model and verified with `migrations has-pending-model-changes`. Continue to review generated migrations and back up real databases before upgrades.
- **Security and scale:** The app is intentionally single-user local software. It has no authentication or per-user isolation and should **not** be exposed to the public Internet as-is.
- **Screenshot provenance:** `screenshots/` and screenshot images in the deck predate the CRUD buttons. Retake these on a verified running build for a truly up-to-date submission.
- **Grading:** The professor chooses the final grade and may question either team member about any code. No code audit can guarantee 100/100.

## Recommended final verification sequence

On a Windows PC with the **.NET 10 SDK** installed, open PowerShell in the extracted project folder and run:

```powershell
powershell.exe -ExecutionPolicy Bypass -File .\scripts\VERIFY_WINDOWS.ps1
```

The supplied ZIP passed this script on Windows with .NET SDK 10.0.403: Release build, 55 unit/integration tests, and 5 browser tests. Still manually perform `docs/QA_CHECKLIST.md`, retake screenshots of the revised interface, and rehearse the deck with both contributors. Record the outcomes and, only after all gates pass, submit the ZIP or push the same checked revision to GitHub.
