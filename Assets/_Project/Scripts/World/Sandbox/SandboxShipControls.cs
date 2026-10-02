using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.Ship;
using UnityEngine;

namespace TitansOfTheSea.World.Sandbox
{
    // Labelled test stand-in for A's input reader. No hardware input in gameplay.
    public sealed class SandboxShipControls : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private ShipSailing _ship;
        private float _steer, _raise, _turn;
        public bool Enabled { get; set; } = true;
        public PlayerInputState Current => Enabled ? new PlayerInputState
        { SteerAxis = _steer, SailAxis = _raise, SailTurnAxis = _turn, HotbarSlot = -1 } : default;
        public void Configure(ShipSailing ship) { _ship = ship; }
        private void OnEnable() { Services.RegisterFallback<IPlayerInput>(this); }
        private void OnDisable() { Services.Unregister<IPlayerInput>(this); if (_ship != null) _ship.PlayerAtControls = false; }
        private void OnGUI()
        {
            if (!(Application.isEditor || Debug.isDebugBuild) || _ship == null) return;
            GUILayout.BeginArea(new Rect(12, 290, 380, 350), GUI.skin.box);
            GUILayout.Label("B SAILING TEST CONTROLS — INPUT STAND-IN");
            _ship.PlayerAtControls = GUILayout.Toggle(_ship.PlayerAtControls, "Man wheel / sail controls");
            GUILayout.Label("Steer: " + _steer.ToString("F2"));
            _steer = GUILayout.HorizontalSlider(_steer, -1, 1);
            GUILayout.Label("Raise / lower sail: " + _raise.ToString("F2"));
            _raise = GUILayout.HorizontalSlider(_raise, -1, 1);
            GUILayout.Label("Rotate sail: " + _turn.ToString("F2"));
            _turn = GUILayout.HorizontalSlider(_turn, -1, 1);
            if (GUILayout.Button("Release controls")) _steer = _raise = _turn = 0;
            if (GUILayout.Button(_ship.AnchorDown ? "Raise anchor" : "Drop anchor")) _ship.SetAnchor(!_ship.AnchorDown);
            GUILayout.Label($"Sail {_ship.SailRaise01[0]:P0} | Boom {_ship.SailAngleDeg[0]:F0}° | Speed {_ship.SpeedMetersPerSec:F1} m/s | Thrust {_ship.LastThrust:F0} N");
            GUILayout.EndArea();
        }
    }
}
