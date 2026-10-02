using UnityEngine;
namespace TitansOfTheSea.Player
{
    public sealed class Climbable : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _bottom, _top, _landing;
        public string Prompt => "Climb ladder / rope";
        public void Configure(Transform bottom, Transform top, Transform landing = null) { _bottom = bottom; _top = top; _landing = landing; }
        public bool CanInteract(PlayerInteractor actor) => _bottom != null && _top != null && actor.Motor != null;
        public void Interact(PlayerInteractor actor) { actor.Motor.BeginClimb(_bottom.position, _top.position, _landing == null ? (Vector3?)null : _landing.position); }
    }
}
