# Sailing sandbox test procedure

Use Unity 6.3 LTS 6000.3.25f1. Open `Assets/_Project/Scenes/B_Sandbox_Sailing.unity` and press Play. The IMGUI controls are B-only test apparatus; Person A supplies the production input reader and presentation.

1. Let the hull settle. Raise the anchor with the sandbox button.
2. Enable Man wheel / sail controls. Move the raise slider toward +1 until the sail is fully raised, then release controls.
3. Check the weather wind direction in the Weather service inspector. Trim the boom using the rotate slider. Sailing directly into wind produces no thrust; a broad reach is most efficient.
4. Once moving, move the steer slider. Rudder authority grows with speed. Release controls to return the rudder to centre.
5. Drop the anchor. Horizontal movement should stop while buoyancy continues.
6. Use `ocean sine`, `weather storm`, `time 20`, and `services` to exercise connected services.

The fixed overview camera and distance markers are labelled stand-ins. A ship may leave the camera view; its Rigidbody continues simulating. Production follow cameras remain Person A's work.

Automated coverage: point of sail, windless/upwind cases, sail reefing/trim, anchor non-reversal, and a real generated-scene PlayMode test for travel, turning, braking and flotation. Runtime values allocate no new sampling/control arrays each physics tick. Shared interfaces are unchanged.

Current limitations: one sail for the test sloop; interaction points and additional ship classes come in following tasks. Hull damage/flooding values are explicitly intact initial values until the damage milestone. Anchor is an arcade horizontal brake, without seabed chain simulation.
