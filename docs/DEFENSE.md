# MoodLog defense guide (15–20 minutes)

## Suggested timing and speaking order

| Part | Duration | Presenter |
| --- | --- | --- |
| Introduction, goal and practical relevance | 2 min | Student A |
| Technologies and alternatives | 2 min | Student B |
| Architecture, FP patterns and state | 4 min | Student A |
| Live demonstration: CRUD, profile, charts, CSV | 5 min | Student B |
| SQLite, tests, risks and conclusions | 3 min | Both |
| Questions | 4 min | Both |

The timing and speaker split are suggestions; review and agree contributions honestly before submitting. Your instructor can ask **either member about any part of the code**.

## Demo script

1. Start with a clean local profile. Open Dashboard and explain the four cards.
2. Log a mood with tags and a note. Edit it, then show the updated history.
3. Add 250 ml of water, change your water goal in Profile and revisit the Water page. Explain that all analytics now use the current profile goal.
4. Add sleep across midnight and a walking activity with steps.
5. Open Analytics, switch from seven to 30 days, explain how average values ignore days without logs, and export CSV.
6. Show source folders for core validation, repositories, database, UI and tests.
7. Run the unit tests and browser tests **if they pass on the presentation machine**; do not claim they passed without running them.

## Common questions and concise answers

**What is functional programming here?** — Domain creation validates inputs and returns `Result<T,DomainError>` rather than throwing for routine invalid data. Models are records; analytics transforms immutable datasets with LINQ. `Map`, `Bind`, `BindAsync`, `Match` and `Option` are implemented in Core.

**Is this pure FP?** — No. IO, persistence and Blazor UI are deliberately imperative at the boundaries; pure validation and data transformation are placed in Core. This is a hybrid functional style.

**Why is the repository an interface?** — Application services depend on storage contracts, not EF Core. SQL storage can be changed and unit-tested using in-memory fakes.

**Why Blazor Server?** — Reuse C# on UI and server with interactive components; the local scenario tolerates needing a connected server. The tradeoff is a persistent interactive connection.

**Why SQLite?** — It stores data in one local file with minimal setup and EF Core migrations. It is not intended for high-concurrency multi-user production.

**What is a scoped state container?** — `WellnessState` exposes a change notification within one Blazor circuit. Dashboard and Analytics subscribe and reload after successful mutations.

**Why not use static global state?** — Static state could be shared between unrelated users/circuits and cause incorrect data mixing.

**How is data validated?** — Data annotations check the form, while immutable model `Create` methods enforce the actual domain rules. The service returns success or failure as data.

**What happens when an entry is edited?** — The service loads the existing ID, revalidates changed fields, preserves its original timestamp, updates through EF Core and notifies the UI. Missing records return `NOT_FOUND`.

**What happens if storage fails?** — A database exception is logged server-side and the UI shows a friendly error. Cancellation/duplication is controlled with a temporary disabled save button.

**What is Smart Insights?** — Deterministic calculations over up to two weeks of logs: water-goal streak, weekly sleep and activity comparisons and frequency of mood check-ins. No ML model or health claims.

**What does cross-platform mean for this system?** — The .NET web app can run on supported desktop OSs; clients use a responsive browser. Real cross-browser/device testing still must be demonstrated.

**Why can goals change for older reports?** — A single current water goal from Profile is the reference for summaries. Historical volumes remain unchanged; prior goal values remain in stored legacy log records for compatibility.

**What would be needed for public cloud deployment?** — Authentication, authorization, tenant data isolation, secure HTTPS, managed secrets, backups, privacy policy and likely PostgreSQL.

## Final reminder

The rubric lists 20 independent criteria (5% each). Slides alone cannot guarantee 100: the demonstration, real test results and each person's answers are also graded.
