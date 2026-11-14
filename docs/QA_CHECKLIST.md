# MoodLog release verification checklist

**Important:** This is a manual test plan, not a claim that these checks have been executed. Check each box only after running the revised project on a machine with .NET 10 and Playwright installed.

## Build / automation

Windows shortcut: `powershell.exe -ExecutionPolicy Bypass -File .\scripts\VERIFY_WINDOWS.ps1` from the extracted project root. It stops if a step fails.



- [ ] `dotnet restore MoodLog.slnx`
- [ ] `dotnet build MoodLog.slnx -c Release`
- [ ] `dotnet test tests/MoodLog.Tests/MoodLog.Tests.csproj -c Release --collect:"XPlat Code Coverage"`
- [ ] `dotnet build tests/MoodLog.E2E/MoodLog.E2E.csproj` and install Playwright Chromium with the generated script
- [ ] `dotnet test tests/MoodLog.E2E/MoodLog.E2E.csproj --no-build`
- [ ] GitHub Actions checks pass on Windows, macOS, and Linux; browser job passes on Ubuntu
- [ ] No uncommitted personal database, logs, credentials, or test outputs in the submission

## Functional scenarios

- [ ] Initial start creates an empty SQLite database and all pages open
- [ ] Restart preserves mood/water/sleep/activity/profile records
- [ ] Mood: create, edit, cancel edit, delete (cancel and confirm), validation of level and 501-character note
- [ ] Water: enter amount, reject zero and 5001, change profile goal, verify Dashboard/Water/Analytics/CSV show same goal, edit amount, delete
- [ ] Sleep: overnight calculation, reject durations above 16h, edit, delete
- [ ] Activity: reject invalid duration and steps, edit, delete
- [ ] Profile: update name and each goal, verify dashboard after navigation, reject out-of-range goals
- [ ] Analytics: 7/30 days, empty days, chart scale, averages, insights, CSV download opens correctly
- [ ] DB unavailable: friendly failure message and no duplicate save
- [ ] Changing selected date does not accidentally edit an entry from the previous date
- [ ] Empty histories, duplicate tags, multiple entries on same day, very long names/notes

## Devices, browsers, and accessibility

- [ ] Chrome, Firefox, Edge, Safari (where available)
- [ ] Windows, Linux, macOS (where available)
- [ ] Responsive viewport widths 320, 375, 768, 1024, and 1440 pixels
- [ ] Screen zoom 200% without lost controls or clipped text
- [ ] Keyboard-only Tab/Shift+Tab, Enter/Space, accessible form labels and focus indicator
- [ ] Deletion confirmation works with mouse and keyboard
- [ ] Confirm contrast of all text, validation errors and navigation states
- [ ] Test actual touchscreen device and portrait/landscape rotation where available
- [ ] Browser console/network: no uncaught exceptions or failing requests during normal flows

## Defense rehearsal (15–20 minutes)

- [ ] Show technology comparison and architecture diagram
- [ ] Explain `Result`, `BindAsync`, immutable records and validation
- [ ] Demonstrate CRUD, analytics, insights, CSV and profile goal consistency
- [ ] Explain the database tables, EF migration approach and local single-user privacy limits
- [ ] Show tests and CI results **actually recorded**
- [ ] Both team members can explain any area of source code; practice questions in `docs/DEFENSE.md`

## Evidence for the final grade

- [ ] Record the real unit/integration/E2E pass counts and coverage percentage in your submission notes
- [ ] Retake screenshots after the new CRUD actions are visible
- [ ] Save proof of passing GitHub Actions runs on the submitted commit
- [ ] Record any manual performance/device findings rather than assuming they pass
