using System.Collections.Generic;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.Ship
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ShipSailing : MonoBehaviour, IShipState
    {
        [SerializeField] private SailingSettings _settings;
        [SerializeField] private string _shipId = "b-test-sloop";
        private Rigidbody _body;
        private readonly float[] _sails = { 0 };
        private readonly float[] _angles = { 0 };
        private readonly float[] _sections = { 1, 1, 1, 1 };
        private IReadOnlyList<float> _sailView, _angleView, _sectionView;
        public string ShipId => _shipId;
        // Damage/flooding will replace these intact initial values in the damage milestone.
        public float HullHealth01 => 1;
        public float WaterLevel01 => 0;
        public IReadOnlyList<float> HullSectionHealth01 => _sectionView;
        public IReadOnlyList<float> SailRaise01 => _sailView;
        public IReadOnlyList<float> SailAngleDeg => _angleView;
        public float WheelAngle01 { get; private set; }
        public bool AnchorDown { get; private set; } = true;
        public float SpeedMetersPerSec => _body == null ? 0 : _body.linearVelocity.magnitude;
        public bool IsSinking => false;
        public Vector3 Velocity => _body == null ? Vector3.zero : _body.linearVelocity;
        public float LastThrust { get; private set; }
        public bool PlayerAtControls { get; set; }
        public bool ControlsSteering { get; set; } = true;
        public bool ControlsSails { get; set; } = true;
        public void Configure(SailingSettings settings) { _settings = settings; }
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _sailView = System.Array.AsReadOnly(_sails);
            _angleView = System.Array.AsReadOnly(_angles);
            _sectionView = System.Array.AsReadOnly(_sections);
        }
        public void SetAnchor(bool down) { AnchorDown = down; }
        public void SetSail(float raised, float angle)
        {
            _sails[0] = Mathf.Clamp01(raised);
            _angles[0] = Mathf.Clamp(angle, -(_settings ? _settings.MaximumSailAngle : 80), _settings ? _settings.MaximumSailAngle : 80);
        }
        public void ApplyControls(PlayerInputState controls, float dt)
        {
            if (_settings == null || dt <= 0) return;
            if (ControlsSteering) WheelAngle01 = Mathf.Clamp(controls.SteerAxis, -1, 1);
            if (ControlsSails)
                SetSail(_sails[0] + Mathf.Clamp(controls.SailAxis, -1, 1) * _settings.SailRaiseSpeed * dt,
                    _angles[0] + Mathf.Clamp(controls.SailTurnAxis, -1, 1) * _settings.SailTurnSpeed * dt);
        }
        private void FixedUpdate()
        {
            if (_settings == null) return;
            float dt = Time.fixedDeltaTime;
            if (PlayerAtControls && Services.TryGet<IPlayerInput>(out var input))
                ApplyControls(input.Enabled ? input.Current : default, dt);
            var forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            var right = Vector3.Cross(Vector3.up, forward);
            var velocity = _body.linearVelocity;
            float speed = Vector3.Dot(velocity, forward);
            if (AnchorDown)
            {
                LastThrust = 0;
                _body.AddForce(SailingMath.AnchorAcceleration(velocity, _settings.AnchorDeceleration, dt), ForceMode.Acceleration);
            }
            else
            {
                var wind = Services.TryGet<IWeatherService>(out var weather) ? weather.Current : default;
                LastThrust = SailingMath.Thrust(forward, wind.WindDirection, wind.WindSpeed, _sails[0], _angles[0], _settings.SailArea, _settings.ThrustCoefficient);
                // Smooth thrust taper and drag bound speed without teleporting the Rigidbody.
                float taper = Mathf.Clamp01(1 - Mathf.Max(0, speed) / _settings.MaximumSpeed);
                _body.AddForce(forward * LastThrust * taper);
                float authority = Mathf.Clamp(speed / 3, -1, 1);
                _body.AddTorque(Vector3.up * WheelAngle01 * authority * _settings.RudderAcceleration, ForceMode.Acceleration);
            }
            var drag = -forward * speed * _settings.ForwardDrag - right * Vector3.Dot(velocity, right) * _settings.SideDrag;
            _body.AddForce(drag);
            _body.AddTorque(-Vector3.up * _body.angularVelocity.y * _settings.TurnDamping, ForceMode.Acceleration);
        }
    }
}
