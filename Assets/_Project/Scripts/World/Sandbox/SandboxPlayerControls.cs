using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.Player;
using UnityEngine;
namespace TitansOfTheSea.World.Sandbox
{
    public sealed class SandboxPlayerControls : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private PlayerMotor _player;
        [SerializeField] private Transform _ship;
        private float _moveX, _moveY, _yaw, _steer, _sail, _trim;
        private bool _sprint, _dive;
        private int _jump = -10, _interact = -10;
        public bool Enabled { get; set; } = true;
        public PlayerInputState Current => Enabled ? new PlayerInputState
        {
            Move = new Vector2(_moveX, _moveY), Look = new Vector2(_yaw, 0), Sprint = _sprint,
            AimHeld = _dive, JumpPressed = Time.frameCount == _jump + 1, InteractPressed = Time.frameCount == _interact + 1,
            SteerAxis = _steer, SailAxis = _sail, SailTurnAxis = _trim, HotbarSlot = -1
        } : default;
        public void Configure(PlayerMotor player, Transform ship) { _player = player; _ship = ship; }
        private void OnEnable() { Services.RegisterFallback<IPlayerInput>(this); }
        private void OnDisable() { Services.Unregister<IPlayerInput>(this); }
        private float Slider(string label, float value)
        { GUILayout.Label(label + ": " + value.ToString("F1")); return GUILayout.HorizontalSlider(value, -1, 1); }
        private void OnGUI()
        {
            if (!(Application.isEditor || Debug.isDebugBuild) || _player == null) return;
            GUILayout.BeginArea(new Rect(600, 12, 370, 670), GUI.skin.box);
            GUILayout.Label("B PLAYER TEST INPUT — BUTTONS / SLIDERS");
            _moveY = Slider("Forward / backward", _moveY); _moveX = Slider("Strafe", _moveX); _yaw = Slider("Turn player", _yaw);
            _sprint = GUILayout.Toggle(_sprint, "Sprint"); _dive = GUILayout.Toggle(_dive, "Dive while swimming");
            if (GUILayout.Button("Jump / swim up")) _jump = Time.frameCount;
            if (GUILayout.Button("Interact / leave station")) _interact = Time.frameCount;
            _steer = Slider("Wheel", _steer); _sail = Slider("Raise / lower sail", _sail); _trim = Slider("Sail trim", _trim);
            if (GUILayout.Button("Release sliders")) _moveX = _moveY = _yaw = _steer = _sail = _trim = 0;
            if (GUILayout.Button("TEST: return to deck")) _player.Teleport(_ship.TransformPoint(new Vector3(0, .85f, -1)));
            if (GUILayout.Button("TEST: move to shore")) _player.Teleport(new Vector3(8, .6f, 0));
            if (GUILayout.Button("TEST: move to water")) _player.Teleport(new Vector3(-5, -.8f, 0));
            var actor = _player.GetComponent<PlayerInteractor>();
            GUILayout.Label(actor.Active?.Prompt ?? actor.Focused?.Prompt ?? "Look toward an interaction marker.");
            GUILayout.Label($"Grounded: {_player.Grounded} | Swimming: {_player.Swimming} | Climbing: {_player.Climbing}");
            GUILayout.EndArea();
        }
    }
}
