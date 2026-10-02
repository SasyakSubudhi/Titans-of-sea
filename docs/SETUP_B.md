# First-time Unity setup

**Completed on this machine:** the actual project is `TitansUnity` inside this workspace. Compilation, sandbox generation, nine logic tests and two physics/integration tests passed in Unity. Open that project and `Assets/_Project/Scenes/B_Sandbox_Buoyancy.unity`, then press Play. The setup steps below are retained for a fresh installation; teammates should clone the actual shared project.

1. Install Unity Hub and **Unity 6.3 LTS (6000.3.25f1)**, with Windows Build Support on Windows. Both teammates must use this exact editor version, including on Mac. This version was selected during setup; package versions will be recorded when Unity creates the project.
2. In Hub, choose New Project → Universal 3D (URP). Create `TitansUnity` inside this workspace. This separate child directory avoids overwriting the source kit.
3. Close the editor. From PowerShell in this source kit folder, run `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Import-PersonB.ps1`. This copies the kit into `TitansUnity` after checking its version. To preview the import, add `-WhatIf`. To target another project folder, add `-ProjectPath 'C:\path\to\project'`. The script stops before copying if existing destination files differ. On Mac, copy this kit's `Assets/_Project` and `docs` folders into the matching locations in the new project. Reopen Unity afterward.
4. In Window → Package Management → Package Manager, install Unity Test Framework. The first sandbox uses engine APIs only. For later milestones install Input System, Cinemachine, Timeline, Addressables, Burst, Collections and Mathematics through the editor's compatible package selection. URP supplies Shader Graph; add VFX Graph when A needs it. Use the editor's TextMeshPro resources when A starts UI.
5. Commit the generated `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json`, and `Packages/packages-lock.json`. These pin the real editor/package versions for the team.
6. Edit → Project Settings → Editor: set Asset Serialization to Force Text and Version Control to Visible Meta Files. Initialize Git in the actual Unity project if not already initialized; configure Git LFS for art/audio before adding large assets. Do not track Library, Temp, Logs, Obj, UserSettings or Builds.
7. Edit → Project Settings → Tags and Layers: keep built-in Default and reserved layers. Add Water, Terrain, Ship, Player, Enemy, Projectile, Interactable, Buildable, Treasure, Titan and Trigger to free user layer slots. Keep built-in Player tag; add Ship, Cannon, Dig_Spot, Merchant and Raider. Agree on numeric layer slots with A before integration.
8. Use Titans of the Sea → Person B → Create Buoyancy Sandbox. This creates `B_Sandbox_Buoyancy.unity`, two `B_` prefabs and four settings assets (ship, box, clock and weather). It prompts before replacing the sandbox and preserves existing settings assets.
9. Press Play. A large hull and small box should settle near the Y=0 markers. This is a physics test, with no ocean art. Use the top-left development panel: `ocean sine`, `ocean flat`, `reset`, `services`, `help`.
10. Select each hull and inspect Ship Buoyancy. Sampling points appear in Scene view with Gizmos enabled. Edit the matching BuoyancySettings asset to tune mass, support depth, lift, damping and centre of mass. Keep hull roots at unit scale.
11. Window → General → Test Runner: run EditMode tests, then PlayMode tests. The stability test covers five simulated minutes at 10× time: 150 game seconds flat water, then 150 game seconds sine water. It restores the previous time scale afterward. Run in an isolated scene and leave the editor active. This is a stability check, not a frame-rate benchmark.
12. Commit the working sandbox, settings, prefabs and all generated `.meta` files on `b/foundation-buoyancy`. Suggested commit: `feat(b): add core contracts and buoyancy sandbox`. Teammate review and Main_World validation precede merge.

## Common problems

- Pink placeholders: confirm the project uses Universal 3D/URP and assign a URP-compatible material to the sandbox geometry. B does not author production materials.
- Missing custom menu: check Console compilation errors and confirm the complete Assets tree was copied, including assembly definitions.
- Hull falls forever: check an enabled FlatOcean or real IOceanSurface provider, nonempty settings points, gravity enabled and non-kinematic Rigidbody.
- Violent rocking: restore default settings, unit root scale and a 0.02-second fixed timestep. No test collider should intersect a hull at spawn.
- Console buttons disappear in a release build: intentional. The panel is restricted to editor/development builds.
- Real ocean is present: it wins over the mock. Removing its provider restores the enabled fallback; A must unregister its provider on disable.

## Integration boundary

Only the B sandbox builder creates test cameras, light, markers and debug panel. These are not A's production presentation. Do not place mock/test components in Main_World. A registers its real IOceanSurface with Services.Register and B follows service changes. Missing water means no buoyancy support; startup sequencing must provide the real ocean before gameplay begins.
