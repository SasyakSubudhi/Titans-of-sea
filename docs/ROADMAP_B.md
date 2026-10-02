# Person B — 36-week roadmap

Status: Phase 0 foundation plus time/weather compiled and tested in Unity. Interactive visuals and integration with A remain pending. Weeks are planning estimates, not completed work.

First practical demo: float and sail one placeholder ship while the clock and weather change. Validate buoyancy and steering before adding player interaction or treasure. Next playable loop: sail → dig → sell → save/load. Ownership remains exactly as MASTER_PROMPT Parts 3.4 and 7; A provides presentation.

| Weeks | Implementation | Completion gate |
|---|---|---|
| 1–2 | Contracts, services/events, bootstrap, mock water, buoyancy, console | Box and ship stable in sandbox; A connection verified |
| 3–6 | Game states, clock, movement/interactions, swimming/decks, sails/steering/anchor | Player sails placeholder ship; day/night advances |
| 7–12 | Items/inventory/storage, crafting, maps/digging, merchants, combat, save/respawn | Sail → dig → sell → reload → die/respawn |
| 13–18 | Fishing/debt/story/quests/dialogue, weather, cannons, ship AI, damage/flooding/repair | Prologue through Chapter 2 opening using A's cutscenes |
| 19–24 | Territory/building/defence, farming/cooking/resources, encounters/raids, upgrades | Build and supply Saltmere; defend a raid |
| 25–30 | Titans (Maw first), progression/services, streaming/world events, ships/quests | Defeat Maw; travel across 6–8 integrated islands |
| 31–36 | Balance, migration/recovery, accessibility, performance, release pipeline | Tested Windows release candidate |

Each milestone adds complete code, tuned logic data, B sandbox, tests, debug triggers and design notes. Integration into Main_World requires the teammate's available services and an agreed edit window. Networking and optional features wait until the required loop works.

Sailing, sail controls/anchor and game states are now implemented and sandbox-tested (12 EditMode / 7 PlayMode). Player movement, swimming, moving decks and wheel/sail interactions are now scene-tested. Next: stats and inventory. Detailed task evidence is tracked in CHECKLIST_B.md.

If behind schedule, cut optional co-op preparation, New Game+, black market, Leviathan, extra ship classes, diving/caves, multiple endings and hired workers in that order. Preserve sailing, treasure, combat, base defence, save/load and story.
