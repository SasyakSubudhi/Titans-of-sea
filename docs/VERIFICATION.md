# Verification — 2026-10-02

- Unity 6.3 LTS **6000.3.25f1** installed. Actual URP project created at `TitansUnity`; scripts compiled and sandbox generation succeeded. Resolved packages include URP 17.3.0 and Test Framework 1.6.0, recorded in Unity-generated package files.
- **Unity EditMode: 9 passed, 0 failed. Unity PlayMode: 2 passed, 0 failed.** Latest PlayMode run took approximately 30.6 seconds and covered five simulated minutes of flat/sine-water buoyancy at 10× time plus the actual generated scene's services, midnight, storm and pause integration.
- Generated B_Sandbox_Buoyancy, two B prefabs and four settings assets; shared layers/tags and Force Text serialization verified.
- Source audit verifies interface coverage, event count, valid assembly JSON/reference graph and complete source delimiters. See tools/audit_foundation.py.
- EditMode test sources cover provider ownership/replacement, stale-session reset, buoyancy force equilibrium/damping and scalar/batch ocean consistency.
- PlayMode source checks five minutes of flat/sine-water hull stability, bounded speed/height, upright orientation and flat-water equilibrium.
- Headless Unity checks establish logic/physics behaviour, not visual quality or frame rate. Interactive graphics, Main_World/Person A integration, Windows build and hardware performance remain unverified.
- Import helper verified with an isolated temporary fixture: dry-run writes nothing; first import succeeds; repeat import skips identical files; differing destination files stop the import and preserve existing content. PowerShell syntax check passed. This fixture does not establish Unity compilation.
- Actual TimeOfDayClock.cs compiled and ran under the installed .NET 9 SDK: nine clock scenarios passed (midnight rollover, new-day event, multiple days, dawn/restore, dusk, zero advance, invalid input, wrapped daylight and a full day of small timesteps). Run via `dotnet run --project tools/ClockChecks/ClockChecks.csproj`.
- Unity time/weather checks passed for transition interruption, normalized wind, seeded schedules and lightning gating. An instant-weather issue was fixed and its new regression test passed; PlayMode was rerun afterward.
- Raw test XML and logs are in `.cache/unity/` at the source kit root: EditMode.xml and PlayMode.xml. No sailing, player movement, treasure or saving implementation is claimed yet.

API references consulted: [Unity Rigidbody](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/rigidbody), [Unity startup callbacks](https://docs.unity3d.com/ja/current/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html), [scene test guidance](https://docs.unity.com/en-us/engine/6000.5/manual/scripting/test-framework-introduction/overview/general-introduction/scene-based-tests).
