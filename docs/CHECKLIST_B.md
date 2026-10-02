# Person B completion checklist

Checks mean implementation and sandbox verification passed. Shared-world integration and teammate review are tracked separately. Unchecked tasks remain in scope. Optional work waits for required milestones.

## Phase milestones

- [x] Unity project, exact editor pin, shared contracts, Git/LFS and public repository.
- [x] Buoyancy sandbox: flat/sine ocean, debug console, nine EditMode and two PlayMode tests (foundation run).
- [ ] Prototype: game states, full player movement/interactions, sailing, anchor, day/night.
- [ ] Core loop: sail, dig, sell, save/load, die and respawn.
- [ ] Story slice: fishing, debt, quests, chapters, cannons, ship AI and repairs.
- [ ] Base defence: claim, build, farm, cook, gather and defend a raid.
- [ ] Titans and expansion: Maw, Ironjaw, progression, ship classes, 6–8 streamed islands and events.
- [ ] Release: balance, save migration/recovery, accessibility, measured stress tests and Windows build.

## Detailed master-document tasks


### 7.1 Core framework — `B-CORE`

- [ ] **B-CORE-01** `GameBootstrap`: first-loaded scene that registers all services, loads settings, then loads the menu or world.
- [x] **B-CORE-02** `Services` locator (Part 5.2) and `GameEvents` bus (Part 5.1).
- [x] **B-CORE-03** Game state machine: `Boot → MainMenu → Loading → Playing → Paused → Cutscene → Dead → Ending`.
- [ ] **B-CORE-04** Scene/world loading with Addressables; additive scenes for islands; loading progress events for A's loading screen.
- [ ] **B-CORE-05** Debug tools: in-game cheat console (teleport, give gold/items, set time/weather, spawn enemy/ship/Titan, kill all, toggle god mode), FPS and state overlay. Essential for fast testing by BOTH teammates.
- [ ] **B-CORE-06** Object pooling utility (reuse bullets, cannonballs, particles' parents) shared with A.

### 7.2 Time & weather logic — `B-TIM` / `B-WTH`

- [x] **B-TIM-01** `TimeOfDayService : ITimeOfDay` — configurable day length, pause during menus, set time via debug.
- [ ] **B-WTH-01** `WeatherService : IWeatherService` — state machine of weather types per region (with 30–90 s blends), weighted random by time of day, forced weather for story beats, lightning timer during storms.
- [ ] **B-WTH-02** Wind model: global wind direction that drifts slowly; gusts; region modifiers; storms increase speed. Used by sailing physics.

### 7.3 Player — `B-PLR`

- [ ] **B-PLR-01** Character controller (CharacterController or capsule + custom physics): walk/run/sprint/jump, swim (surface + dive), climb ladder/rope, step up, stand on a moving ship (**moving platform** — the player must move with the ship deck and not slide off).
- [ ] **B-PLR-02** Stats: health, stamina, hunger (slow drain), hit points regen rules, buffs/debuffs (well-fed, wet/cold optional, bleeding optional).
- [ ] **B-PLR-03** Interaction system: raycast/overlap for "Interact" (doors, chests, wheel, sails, merchants, beds, dig spots).
- [ ] **B-PLR-04** Reads `IPlayerInput` only; never reads the keyboard directly.
- [ ] **B-PLR-05** Equipment: equipped weapon/clothing slots affect stats; hotbar selection.

### 7.4 Ship systems — `B-SHP`

- [ ] **B-SHP-01** **Buoyancy:** the ship is a Rigidbody with 8–16 float points on the hull; each frame, query `IOceanSurface.GetHeights()` and apply upward forces proportional to submerged depth, damping, and torque so the ship tilts with waves. Tunable per `ShipDefinition` (mass, drag, centre of mass).
- [x] **B-SHP-02** **Sailing:** wind vector × sail angle × sail raise amount → forward thrust (use a realistic-feeling "point of sail" curve: running with the wind, reaching, beating upwind badly); rudder turning force scales with speed; anchor stops the ship; momentum and drift.
- [ ] **B-SHP-03** **Controls** (via player interaction with the wheel/sail ropes): steering, raise/lower each sail, rotate sail (boom angle), drop/raise anchor, brace for waves. Another player/NPC could man them (crew, later).
- [ ] **B-SHP-04** **Damage:** hull split into sections each with HP. Cannonballs create hit "holes" at impact positions that leak water; water level rises → ship gets heavier and slower → sinks if not bailed. Player actions: bail with bucket, repair with planks (consumes wood). Chain shot damages sails/masts. Fire spreads on wooden sections unless extinguished.
- [ ] **B-SHP-05** **Sinking & loot:** when sunk, spawn floating loot crates, despawn the ship after delay, event `OnShipSunk`; player respawns per rules in 7.14.
- [ ] **B-SHP-06** **Docking:** gentle collision with docks, mooring zone detection, lowering the plank, boarding transitions.
- [ ] **B-SHP-07** **Ship collisions:** ram damage by speed, island/reef collision damage, ship-to-ship bumping.
- [ ] **B-SHP-08** **Ship upgrades & stats:** implement upgrade effects (hull HP, sail area, rudder strength, cargo capacity, cannon slots, speed cap) from `ShipDefinition` tier tables. Cargo/storage system for ship chests.
- [ ] **B-SHP-09** Implements `IShipState` for each ship (player and AI ships).
- [ ] **B-SHP-10** Ship classes: rowboat (rowing physics), sloop, brigantine, galleon (later), warship (late). Data-driven.

### 7.5 Inventory, items & crafting — `B-INV`

- [ ] **B-INV-01** `InventoryService : IInventory` (slots, stack sizes, weight/capacity, gold). Hotbar. Separate storage for chests/ship hold.
- [ ] **B-INV-02** `ItemDefinition` ScriptableObjects (id, type, stack size, value, weight, stats, tier, tags). Categories: Weapon, Tool, Clothing, Food, Material, Treasure, Quest, Ammo, Seed, Placeable.
- [ ] **B-INV-03** Crafting: `RecipeDefinition` (inputs, outputs, station, time). Stations: workbench, stove/cooking pot, forge/anvil, shipwright table.
- [ ] **B-INV-04** Treasure items with value tiers (coins, gems, relics, legendary hoard pieces). Carrying treasure increases enemy pressure ("heat").
- [ ] **B-INV-05** Drops/loot tables for chests, enemies, barrels, Titans.

### 7.6 Combat — `B-CMB`

- [ ] **B-CMB-01** Damage system: `IDamageable` interface, damage types (slash, pierce, blunt, bullet, cannon, fire, bite/crush), armour/resistance, hit reactions, invulnerability frames during dodge, block/parry window.
- [ ] **B-CMB-02** Melee: combo chains, light/heavy attacks, hit detection via animation events from A (`AnimationEventRelay`) + overlap/spherecast; stamina cost; knockback.
- [ ] **B-CMB-03** Firearms: flintlock/musket/blunderbuss with reload times, spread, range, accuracy when aiming; hit-scan or projectile with drop; ammo consumption.
- [ ] **B-CMB-04** Cannons: aim (yaw/pitch), load (take cannonball from hold → insert → light fuse → fire), reload time, ballistic projectile with gravity, hit detection on ships/islands/Titans/water splash events. Ammo types: cannonball, chain shot (damages sails), fire shot.
- [ ] **B-CMB-05** Throwables: powder-keg bombs, torches. Area damage.
- [ ] **B-CMB-06** Boarding: grapple/plank boarding transitions, fight on enemy decks, capturing enemy ships (later).
- [ ] **B-CMB-07** Aggro/threat tuning and fairness rules (telegraphs: every enemy attack has a wind-up time that A can animate).

### 7.7 Enemy AI & raids — `B-AI`

- [ ] **B-AI-01** Land AI (pirates, guards, wildlife): NavMesh navigation (Unity's built-in path-finding; bake per island), state machine/behaviour tree: Idle/Patrol → Alert → Chase → Attack → Flank → Retreat → Call-for-help. Cover usage for gunners (basic).
- [ ] **B-AI-02** **Ship AI:** steering behaviours — sail to target using the same `ShipSailing` code as the player (wind matters to AI too), keep an ideal broadside distance, aim and fire cannons with prediction, ram, attempt to board, flee when damaged, call reinforcements.
- [ ] **B-AI-03** **Pirate encounter director:** spawns hostile ships around the player based on fame, treasure carried ("heat"), time of day, region danger level, with limits (max active ships, minimum distance spawns away from the player's view). Faction behaviour differences (Reavers = aggressive, Gulls = fast raiders).
- [ ] **B-AI-04** **Island raid system:** scheduler (probability grows with fame and days since last raid; guaranteed story raid in Chapter 5), approach from sea, landing parties, targets (storage, crops, cannons, you), defender behaviour (hired guards), raid win/lose rules (what is stolen if you lose), reward on win, event `OnRaidStarted/Ended`.
- [ ] **B-AI-05** Ambient life: seagulls, fish schools, crabs, boars on islands, merchant ships sailing routes (can be robbed later).
- [ ] **B-AI-06** NPC daily routines at Port Aurelia/Saltmere (simple schedule by time of day, sleep at night).

### 7.8 Titans — `B-TIT`

- [ ] **B-TIT-01** Boss framework: phases, health bar data, weak points, attack pattern selector, arena awareness, enrage timers.
- [ ] **B-TIT-02** The Maw: tentacle slams on deck, grabs ship and drags, ink cloud (visibility), tentacles must be cut; weak point = eyes after phase 2.
- [ ] **B-TIT-03** Ironjaw: circles ship, rams hull, breaches and bites; harpoon/cannon fish-hook mechanic.
- [ ] **B-TIT-04** Leviathan: multi-phase final sea boss with storm sync; spline-follow body logic with segments; lightning interplay.
- [ ] **B-TIT-05** Titan spawn rules (rare roaming events, story-forced encounters, "hoard" rewards).

### 7.9 World, streaming & treasure — `B-WLD`

- [ ] **B-WLD-01** World layout data: island positions, ids, danger levels, regions (shallows, open sea, storm belt).
- [ ] **B-WLD-02** **Streaming:** load island scenes when within ~700 m, unload beyond ~1000 m; keep ships/enemies simulated in a cheap mode when far; floating origin / world-shifting (**floating origin** = periodically moving everything back toward (0,0,0) so physics stays accurate far from the centre) if world size demands.
- [ ] **B-WLD-03** Treasure system: map items → X marker positions, dig spots (hold interact with shovel), loot tables by region danger, randomised each new game (seeded), fake/trap spots optional. Riddle maps and bottle messages (later).
- [ ] **B-WLD-04** Points of interest events: sunken wreck, merchant ship under attack, floating survivors, ghost ship at night, whirlpool.
- [ ] **B-WLD-05** Fast travel between docks you own (later), map discovery state saved.
- [ ] **B-WLD-06** Hazards: reef collision zones, whirlpool force fields, rogue wave events (coordinates with A's wave scale via weather).

### 7.10 Base building — `B-BLD`

- [ ] **B-BLD-01** Placement system: grid/free-snap, ghost preview (uses A's ghost material), validity checks (terrain slope, overlap, within claimed territory, cost), rotate, demolish with refund, snap-points from A's prefabs.
- [ ] **B-BLD-02** Territory claim: radius/area around Saltmere's flag grows with island level; only build inside.
- [ ] **B-BLD-03** Structure stats: HP, repair, resource cost, decay (optional), destruction by raiders.
- [ ] **B-BLD-04** Defence: placeable cannons (loadable by player/guards, auto-aim optional upgrade), walls/gates, watchtowers (early warning of raids), traps (later).
- [ ] **B-BLD-05** House: rooms/furniture placement; bed = respawn point; storage chests; crafting stations.
- [ ] **B-BLD-06** Island upgrade levels (hut → village → fort) with requirements.
- [ ] **B-BLD-07** Hired helpers (later): guards, farmers, cook; wages; simple orders.

### 7.11 Farming, cooking, fishing & survival — `B-FRM`

- [ ] **B-FRM-01** Farming: till soil, plant seeds, growth stages by game-time, watering (optional), weather effect, harvest yields, crop data in `CropDefinition`. 3–4 crops for slice (e.g. cabbage, potato, pumpkin, corn).
- [ ] **B-FRM-02** Cooking: recipes, cooking time, burn/quality (optional), meals give buffs (stamina regen, max health, wave-sickness resistance, speed on ship).
- [ ] **B-FRM-03** Fishing: mini-game (cast, bite timing, tension bar), fish types by area/time/weather, selling and cooking. **Needed in the prologue tutorial.**
- [ ] **B-FRM-04** Resource gathering: trees (wood), rocks (stone), ore (iron), plants (fiber/cloth), animals (leather/meat). Nodes respawn after N days.
- [ ] **B-FRM-05** Hunger & food spoilage (optional), water/drinks (optional).

### 7.12 Economy, merchants & debt — `B-ECO`

- [ ] **B-ECO-01** `MerchantDefinition`: stock, buy/sell price multipliers, restock timer, reputation discounts.
- [ ] **B-ECO-02** Dynamic prices: `price = basePrice × islandModifier × demandModifier × eventModifier × (1 − tradingSkillDiscount)`, with demand shifting when you sell lots of one thing.
- [ ] **B-ECO-03** Services: blacksmith (weapon/armour upgrade tiers), shipwright (ship purchase/upgrade/repair/paint), tailor (clothing), tavern (rumour, quests, later hire crew), black market (rare items, late).
- [ ] **B-ECO-04** **Debt system (prologue):** debt amount, interest rises on a schedule, deadline in days, collectors' visits as events (each calls `ShowDialogue`), scripted unwinnable outcome tuned so a skilled player reaches ~85–90% repaid but cannot finish → leads to Chapter 1 trigger. `OnDebtChanged` event for the HUD.
- [ ] **B-ECO-05** Currency: gold coins; treasure items convert to gold at merchants (rare relics sell for much more at specific buyers).

### 7.13 Story, quests & dialogue logic — `B-QST`

- [ ] **B-QST-01** Story state machine: chapters with flags; `OnChapterChanged`. Calls `ICutsceneService.Play(id, callback)` at the right moments and locks input during them.
- [ ] **B-QST-02** Quest system: `QuestDefinition` (id, title, steps, objectives types: reach location, kill, collect, deliver, dig, survive, build, cook), rewards, prerequisites; implements `IQuestService`.
- [ ] **B-QST-03** Dialogue data (branching, conditions, effects) feeding `IUiNotifier.ShowDialogue`; a simple text format (JSON/ScriptableObject) so writers can add lines without code.
- [ ] **B-QST-04** Side quests from NPCs and tavern; tutorial hints triggered by situations (first storm, first hole in ship, first raid...).
- [ ] **B-QST-05** Endings logic: choice flags → which epilogue cutscene id.
- [ ] **B-QST-06** Fame/reputation system: fame affects pirate frequency, merchant discounts, NPC dialogue.

### 7.14 Progression, saving & respawn — `B-PRG` / `B-SAV`

- [ ] **B-PRG-01** Skill tree: Sailing (faster sails, better turning), Combat (damage, stamina), Survival (health, hunger), Trading (better prices). XP from actions; skill points; data-driven.
- [ ] **B-PRG-02** Gear tiers (clothing, weapons, ships) with clear stat progression and unlock requirements.
- [ ] **B-SAV-01** Save system: JSON with version number and migration; saves: player, inventory, ships (position, damage, cargo), island buildings and crops, quest/chapter flags, world state (time, weather, discovered islands, treasures dug, raid timers), debt, fame, skills. Multiple slots, auto-save every N minutes and at safe moments, corruption protection (write to temp file then rename).
- [ ] **B-SAV-02** **Respawn:** on death → show death screen (A) → respawn at the latest chosen respawn point (home island bed, placed flag, owned dock; default Saltmere). Death penalty: drop a percentage (e.g. 30%) of carried treasure and gold at the death location in a retrievable bag for a limited time; keep gear. If the player's ship sank, they get a free rowboat at the respawn point.
- [ ] **B-SAV-03** New Game+ (later).

### 7.15 Balance, testing & tooling — `B-BAL` / `B-TST`

- [ ] **B-BAL-01** All numbers (damage, prices, costs, growth times) in ScriptableObjects or a CSV importer. A balance sheet in `docs/BALANCE.md` listing target progression pacing (e.g. first ship by ~1.5 hours of play).
- [ ] **B-TST-01** Unit tests (EditMode) for: price formula, sail thrust curve, inventory add/remove/stack, save/load round-trip, quest transitions, raid probability. Play Mode tests for buoyancy stability (ship does not explode or sink spontaneously over 5 minutes).
- [ ] **B-TST-02** Stress scenes: 20 AI ships; 50 enemies on an island; check CPU time budget (log it).
- [ ] **B-TST-03** Debug cheat console (see B-CORE-05) kept in release builds behind a flag.
- [ ] **B-NET-01** (Future, optional) Prepare for co-op: keep player/ship state in serialisable data classes; no direct `Input` calls in gameplay code; document what would need to sync. Do **not** implement networking in the first release.

## Integration and release evidence

- [ ] Run each milestone in Main_World with Person A dependencies.
- [ ] Teammate runs each phase; review and merge feature pull requests into dev.
- [ ] Manual moving-deck, sailing, repairs, sinking and respawn playtests.
- [ ] Save corruption recovery, slot isolation and version migration.
- [ ] Profile 20 ships and 50 enemies on representative hardware; record actual FPS.
- [ ] Windows release build and player feedback regression pass.

## Active work

Sailing and game states are sandbox-tested: 12 EditMode and 3 PlayMode tests passed. Eight-point hull buoyancy is verified; B-SHP-01 remains open until the later ShipDefinition data integration. Next: player movement, swimming, moving decks and wheel/sail interaction. Weather region weighting remains open.

- [x] ~~Single-sail test sloop: wind thrust, trim, raise/lower, rudder and anchor.~~
- [x] ~~Game-state transitions, input gating, pause and clock gating.~~
- [x] ~~Five-minute eight-point hull stability regression.~~
- [ ] Player locomotion, interactions and moving-deck sandbox.
- [ ] Main_World integration and teammate review of the prototype.
