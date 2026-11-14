# Assessment rubric: requirement-to-evidence map

This map points the reviewer to concrete evidence for each of the 20 criteria (5% each). Evidence in source code is **not** evidence of a successful run: verify the application and CI on a machine with .NET 10, and show actual test results during the defense.

| # | Requirement | Evidence / places to inspect | Verification still needed |
|---|---|---|---|
| 1 | Code structure and design | Core / Infrastructure / Web separation; interfaces; `docs/ARCHITECTURE.md` | Build and maintainability review |
| 2 | Component reusability | `Shared/ChartPanel.razor`, `Shared/EntryActions.razor` (parameters and callbacks) | Review component interaction |
| 3 | Cross-platform | .NET 10 ASP.NET Core, responsive browser UI; three-OS CI matrix | Pass matrix on all 3 OS and mobile browsers |
| 4 | State management | Scoped `WellnessState` with change notifications; Dashboard and Analytics subscribe/dispose | Verify interactive refresh |
| 5 | UI design | Original theme, forms, metrics and screenshots | Review current running UI |
| 6 | UX | CRUD, cancel edit, confirmed delete, validation and status messages | Browser and keyboard checks |
| 7 | Responsiveness | `wwwroot/app.css` responsive layouts, flexible action rows and breakpoints | 320/375/768/1024/1440 px manual review |
| 8 | Core functionality | All 4 tracker services, Profile, Analytics, CSV and data integrity | Complete feature checklist |
| 9 | Advanced features | Smart Insights, state-driven refresh, CRUD persistence and report export | Demonstrate functionality |
| 10 | Error handling | Domain `Result`, annotations, try/catch with `ILogger`, pending button state | Simulate DB error |
| 11 | Efficiency | EF `AsNoTracking`, date-filtered queries, small CSS charts | Profile real loading times |
| 12 | Scalability | Thin services / repository abstractions / replaceable DB adapter | State local single-user scope honestly |
| 13 | Resource utilization | Short-lived factory contexts, bounded charts, no cloud calls | Measure CPU/RAM/network |
| 14 | Testing coverage | Unit, SQLite integration, and Playwright E2E under `tests/` | Run test suite, collect coverage |
| 15 | Quality assurance | `docs/QA_CHECKLIST.md` with browser/device/error scenarios | Perform and record manual tests |
| 16 | Documentation | README, ARCHITECTURE, DEFENSE, QA, RELEASE_NOTES | Check instructions on another computer |
| 17 | Innovation | Integrated local private wellness tracker, deterministic non-clinical insights | Explain benefits/differentiation |
| 18 | Creative FP/.NET usage | `Result`, `Option`, immutable records, `BindAsync`, LINQ, EF Core, Blazor events | Explain FP boundaries at defense |
| 19 | Professionalism | Structured app, migration compatibility, tests, CI, slides, docs | Verify build, screenshots, demo |
| 20 | Presentation | `presentation/MoodLog_Final_Presentation.pptx`, `docs/DEFENSE.md` | Rehearse 15–20 min with both people |

## Requirements from assignment

The project can have at most two team members. Both must participate in programming. Defense is mandatory and any member can be questioned on any part of the code. The presentation needs names, topic, aim, goals, plan, existing technology comparison, architecture, practical relevance, and methodology/mockups. The slide deck includes these sections; do not misrepresent contribution or unexecuted verification as completed.

## Important limitations that cannot be solved by source edits alone

- A **100/100 grade cannot be guaranteed**; some categories depend on instructors' subjective assessment and live defense.
- Full cross-platform, accessibility, performance and high-coverage claims must be supported by measured results; the development environment used to prepare this archive has no .NET SDK.
- The original software is a local single-user application. Authentication and Internet deployment are not part of its safe scope.
