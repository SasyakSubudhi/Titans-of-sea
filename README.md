# Titans of the Sea — shared Unity project

Editor: **Unity 6.3 LTS 6000.3.25f1**, identical on Windows and Mac. Use this directory as the shared repository root. Commit Assets (including .meta files), Packages, ProjectSettings and docs; exclude Library, Temp, Logs, UserSettings and Builds.

Open `Assets/_Project/Scenes/B_Sandbox_Buoyancy.unity` and press Play. Try `ocean sine`, `time 20`, `speed 60`, `weather storm`, `services`, `pause` and `resume` in the development panel.

Person B's foundation and clock/weather compile in Unity. Twelve EditMode tests and seven physics/integration tests passed. See docs/VERIFICATION.md for evidence and limits. Sailing and game states are sandbox-tested. Player locomotion and ship interaction are tested in B_Sandbox_Player. Next: stats and inventory. Open B_Sandbox_Sailing to test the ship. See docs/CHECKLIST_B.md for full progress. Ownership follows docs/MASTER_PROMPT.md.

Public repository: https://github.com/SasyakSubudhi/Titans-of-sea. See docs/GITHUB_SETUP.md for teammate setup and branches.
