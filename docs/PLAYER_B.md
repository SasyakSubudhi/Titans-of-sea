# Player sandbox

Open B_Sandbox_Player with Unity 6000.3.25f1. Press Play. The right panel is a B-only input stand-in; production devices/camera/UI remain Person A's responsibility.

- Forward/strafe/turn sliders move the capsule; Sprint increases speed. Jump is a one-frame pulse. Release sliders stops sustained test input.
- Return to deck resets the test player near the wheel. Face the tall wheel marker and press Interact; movement locks while aboard the station. Use Wheel to steer. Press Interact again to leave.
- Face the sail-rope marker to raise/lower or trim the sail, and the anchor marker to toggle anchor state. Occupied controls are exclusive.
- Move to shore tests walking and a .2m step. The ladder marker starts a climb; forward climbs and Jump exits. The ladder has a test landing platform; configure an exit marker and clear landing lip in a finished level.
- Move to water tests surface buoyancy and swimming. Dive descends; release Dive to return toward the surface. Jump gives an upward impulse.
- `state paused` / `state playing` exercises global pause/input gating. `ocean sine` exercises a rocking deck.

Ground checks ignore the Player layer. Set player roots to Player; provide solid deck/terrain colliders and unit-scale physics roots. Interaction markers are triggers and must remain within the eye ray's 2.5m reach. The visual eye is an interaction reference, not a production camera.

Motor yaw uses Look.x as a normalized turn-rate axis (120 degrees/sec at full input). A's reader can map its look delta/rate accordingly. Movement remains character-relative; cameras may rotate the character through this axis.

Moving-deck support stores the grounded Rigidbody's local anchor and previous transform. Each motor tick carries the capsule by the deck's position/rotation delta before applying independent motion. Jumping detaches. No parenting changes move the physics body. Wave support and surface swimming query IOceanSurface.

AutomaticSimulation can be disabled for deterministic integration checks. Gameplay reads IPlayerInput; no direct device input is used. IInteractable and the motor are B-local APIs; Part 5 contracts remain unchanged.

Compilation and scene generation passed. Player integration tests cover walking, stepping, swimming/diving, climbing/landing, translated/rotated decks, live input replacement, actual sailing in sine waves, and station cleanup. Stats, equipment, stamina coupling, full docking and story interactions remain subsequent tasks.

CharacterController.minMoveDistance is set to zero so high render frame rates do not discard small movement steps. During cutscenes the motor continues deck transport/gravity with zero gameplay input; global pause still stops simulation.
