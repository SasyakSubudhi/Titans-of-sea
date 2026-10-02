using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings _settings;
        private CharacterController _controller;
        private Transform _platform;
        private Matrix4x4 _platformPrevious;
        private Vector3 _platformAnchor, _ladderBottom, _ladderTop;
        private Vector3? _ladderLanding;
        private float _vertical;
        public bool AutomaticSimulation { get; set; } = true;
        public bool MovementLocked { get; set; }
        public bool Grounded { get; private set; }
        public bool Swimming { get; private set; }
        public bool Climbing { get; private set; }
        public Transform Platform => _platform;
        public Vector3 Velocity { get; private set; }
        public void Configure(PlayerMovementSettings settings)
        {
            _settings = settings;
            if (_controller == null) _controller = GetComponent<CharacterController>();
            if (_controller != null) ConfigureController();
        }
        private void Awake() { _controller = GetComponent<CharacterController>(); ConfigureController(); }
        private void ConfigureController()
        {
            _controller.height = 1.8f; _controller.radius = .32f;
            _controller.center = new Vector3(0, .9f, 0);
            _controller.skinWidth = .03f;
            _controller.minMoveDistance = 0; // Preserve motion even at very high render frame rates.
            if (_settings != null) { _controller.stepOffset = _settings.StepHeight; _controller.slopeLimit = _settings.SlopeLimit; }
        }
        private void Update()
        {
            if (!AutomaticSimulation || Time.deltaTime <= 0) return;
            var controls = Services.TryGet<IPlayerInput>(out var input) && input.Enabled ? input.Current : default;
            if (Services.TryGet<GameSession>(out var session) && !session.AcceptsPlayerInput) controls = default;
            Simulate(controls, Time.deltaTime);
        }
        public void Teleport(Vector3 position)
        {
            GetComponent<PlayerInteractor>()?.ReleaseInteraction();
            bool active = _controller.enabled;
            _controller.enabled = false; transform.position = position; _controller.enabled = active;
            _platform = null; _vertical = 0; Climbing = Swimming = false;
        }
        public bool BeginClimb(Vector3 bottom, Vector3 top, Vector3? landing = null)
        {
            if (Vector3.Distance(bottom, top) < .5f || Vector3.Distance(transform.position, bottom) > 2.5f) return false;
            _ladderBottom = bottom; _ladderTop = top; Climbing = true; Swimming = false; _platform = null; _vertical = 0;
            _ladderLanding = landing;
            return true;
        }
        public void Simulate(PlayerInputState input, float dt)
        {
            if (_settings == null || dt <= 0 || !_controller.enabled) return;
            Vector3 before = transform.position;
            TransportWithDeck();
            if (!MovementLocked) transform.Rotate(Vector3.up, input.Look.x * _settings.YawSpeed * dt, Space.World);
            if (Climbing)
            {
                Vector3 direction = (_ladderTop - _ladderBottom).normalized;
                Vector3 along = _ladderBottom + direction * Mathf.Clamp(Vector3.Dot(transform.position - _ladderBottom, direction), 0, Vector3.Distance(_ladderBottom, _ladderTop));
                _controller.Move(along - transform.position + direction * input.Move.y * _settings.ClimbSpeed * dt);
                if (input.JumpPressed) Climbing = false;
                else if (Vector3.Dot(transform.position - _ladderTop, direction) >= -.05f)
                {
                    // Clear the landing's lip before moving sideways onto its surface.
                    _controller.Move(_ladderTop + Vector3.up * .1f - transform.position);
                    if (_ladderLanding.HasValue) _controller.Move(_ladderLanding.Value - transform.position);
                    Climbing = false; _vertical = 0;
                }
                Velocity = (transform.position - before) / dt;
                return;
            }
            Grounded = ProbeGround(out var hit);
            float water = Services.TryGet<IOceanSurface>(out var ocean) ? ocean.GetHeight(transform.position) : float.NegativeInfinity;
            Swimming = !Grounded && water > transform.position.y + .5f;
            Vector2 move = MovementLocked ? Vector2.zero : Vector2.ClampMagnitude(input.Move, 1);
            var directionXZ = transform.right * move.x + transform.forward * move.y;
            directionXZ.y = 0;
            float speed = Swimming ? _settings.SwimSpeed : input.Sprint ? _settings.SprintSpeed : _settings.WalkSpeed;
            if (Swimming)
            {
                _platform = null;
                if (input.AimHeld && !MovementLocked) _vertical = -_settings.DiveSpeed;
                else _vertical += ((water - _settings.SwimDepth - transform.position.y) * _settings.SurfaceSpring - _vertical * _settings.SurfaceDamping) * dt;
                if (input.JumpPressed && !MovementLocked) _vertical = _settings.DiveSpeed;
            }
            else
            {
                if (Grounded && _vertical <= 0) _vertical = -2;
                if (Grounded && input.JumpPressed && !MovementLocked)
                { _vertical = Mathf.Sqrt(2 * _settings.Gravity * _settings.JumpHeight); Grounded = false; _platform = null; }
                else _vertical = Mathf.Max(-40, _vertical - _settings.Gravity * dt);
            }
            _controller.Move((directionXZ * speed + Vector3.up * _vertical) * dt);
            if ((_controller.collisionFlags & CollisionFlags.Above) != 0 && _vertical > 0) _vertical = 0;
            if (!Swimming && _vertical <= 0 && ProbeGround(out hit))
            {
                Grounded = true;
                var body = hit.collider.attachedRigidbody;
                _platform = body != null ? body.transform : null;
                if (_platform != null)
                { _platformAnchor = _platform.InverseTransformPoint(transform.position); _platformPrevious = _platform.localToWorldMatrix; }
            }
            else _platform = null;
            Velocity = (transform.position - before) / dt;
        }
        private bool ProbeGround(out RaycastHit hit)
        {
            int mask = _settings.GroundMask.value & ~(1 << gameObject.layer);
            return Physics.SphereCast(transform.position + Vector3.up * .37f, .27f, Vector3.down, out hit, .2f, mask, QueryTriggerInteraction.Ignore)
                && hit.normal.y >= Mathf.Cos(_settings.SlopeLimit * Mathf.Deg2Rad);
        }
        private void TransportWithDeck()
        {
            if (_platform == null) return;
            Vector3 previous = _platformPrevious.MultiplyPoint3x4(_platformAnchor);
            _controller.Move(_platform.TransformPoint(_platformAnchor) - previous);
            Vector3 oldForward = _platformPrevious.MultiplyVector(Vector3.forward);
            oldForward.y = 0;
            Vector3 newForward = _platform.forward; newForward.y = 0;
            transform.Rotate(Vector3.up, Vector3.SignedAngle(oldForward, newForward, Vector3.up), Space.World);
        }
        private void OnDisable() { _platform = null; MovementLocked = false; }
    }
}
