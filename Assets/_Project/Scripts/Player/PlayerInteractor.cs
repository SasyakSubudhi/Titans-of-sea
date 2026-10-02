using System;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.Player
{
    // New B-local gameplay API; does not modify the shared Part 5 contracts.
    public interface IInteractable
    {
        string Prompt { get; }
        bool CanInteract(PlayerInteractor actor);
        void Interact(PlayerInteractor actor);
    }
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Transform _eye;
        [SerializeField, Min(.1f)] private float _range = 2.5f;
        [SerializeField] private LayerMask _mask = ~0;
        private IInteractable _active;
        private bool _pressed;
        public IInteractable Focused { get; private set; }
        public IInteractable Active => _active;
        public PlayerMotor Motor => GetComponent<PlayerMotor>();
        public event Action<string> PromptChanged;
        public void Configure(Transform eye) { _eye = eye; }
        public void SetActive(IInteractable target) { _active = target; }
        public void Scan()
        {
            IInteractable next = null;
            if (_eye != null && Physics.Raycast(_eye.position, _eye.forward, out var hit, _range, _mask.value & ~(1 << gameObject.layer), QueryTriggerInteraction.Collide))
            {
                var candidate = hit.collider.GetComponentInParent<IInteractable>();
                if (candidate != null && candidate.CanInteract(this)) next = candidate;
            }
            if (!ReferenceEquals(next, Focused)) { Focused = next; PromptChanged?.Invoke(next?.Prompt ?? ""); }
        }
        public bool TryInteract()
        {
            var target = _active ?? Focused;
            if (target is UnityEngine.Object obj && obj == null) return false;
            if (target == null || !target.CanInteract(this)) return false;
            target.Interact(this); return true;
        }
        private void Update()
        {
            if (Services.TryGet<GameSession>(out var session) && !session.AcceptsPlayerInput) return;
            Scan();
            bool pressed = Services.TryGet<IPlayerInput>(out var input) && input.Enabled && input.Current.InteractPressed;
            if (pressed && !_pressed) TryInteract();
            _pressed = pressed;
        }
        public void ReleaseInteraction()
        {
            var active = _active; _active = null;
            if (active is UnityEngine.Object obj && obj == null) active = null;
            if (active != null) active.Interact(this);
        }
        private void OnDisable()
        {
            ReleaseInteraction();
            _pressed = false; Focused = null;
        }
    }
}
