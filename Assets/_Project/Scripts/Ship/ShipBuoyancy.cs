using System;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.Ship
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ShipBuoyancy : MonoBehaviour
    {
        [SerializeField] private BuoyancySettings _settings;
        private Rigidbody _body;
        private IOceanSurface _ocean;
        private Vector3[] _localPoints;
        private Vector3[] _positions;
        private float[] _heights;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        public Rigidbody Body => _body;
        public void Configure(BuoyancySettings settings)
        {
            _settings = settings;
            if (_body != null) Initialize();
        }
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _initialPosition = _body.position;
            _initialRotation = _body.rotation;
            Initialize();
        }
        private void Initialize()
        {
            if (_settings == null || _settings.LocalPoints == null || _settings.LocalPoints.Length == 0)
            {
                Debug.LogError("ShipBuoyancy requires settings with at least one sampling point.", this);
                enabled = false;
                return;
            }
            _localPoints = (Vector3[])_settings.LocalPoints.Clone();
            _positions = new Vector3[_localPoints.Length];
            _heights = new float[_localPoints.Length];
            _body.mass = Mathf.Max(1f, _settings.MassKg);
            _body.centerOfMass = _settings.CentreOfMass;
            _body.linearDamping = Mathf.Max(0f, _settings.LinearDamping);
            _body.angularDamping = Mathf.Max(0f, _settings.AngularDamping);
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        private void OnEnable()
        {
            Services.Changed += OnServicesChanged;
            RefreshOcean();
        }
        private void OnDisable() => Services.Changed -= OnServicesChanged;
        private void OnServicesChanged(Type type)
        {
            if (type == typeof(IOceanSurface)) RefreshOcean();
        }
        private void RefreshOcean() => Services.TryGet(out _ocean);
        private void FixedUpdate()
        {
            if (_ocean == null || _positions == null) return;
            int count = _positions.Length;
            for (int i = 0; i < count; i++) _positions[i] = transform.TransformPoint(_localPoints[i]);
            _ocean.GetHeights(_positions, _heights, count);
            float gravity = Mathf.Abs(Physics.gravity.y);
            for (int i = 0; i < count; i++)
            {
                float acceleration = BuoyancyMath.SupportAcceleration(_heights[i] - _positions[i].y,
                    _settings.FloatDepth, _settings.LiftMultiplier, _settings.VerticalDamping,
                    _body.GetPointVelocity(_positions[i]).y, gravity);
                _body.AddForceAtPosition(Vector3.up * (acceleration * _body.mass / count), _positions[i], ForceMode.Force);
            }
        }
        public void ResetPose()
        {
            _body.position = _initialPosition;
            _body.rotation = _initialRotation;
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.WakeUp();
        }
        private void OnDrawGizmosSelected()
        {
            if (_settings == null || _settings.LocalPoints == null) return;
            Gizmos.color = Color.cyan;
            foreach (Vector3 point in _settings.LocalPoints) Gizmos.DrawSphere(transform.TransformPoint(point), 0.1f);
        }
    }
}
