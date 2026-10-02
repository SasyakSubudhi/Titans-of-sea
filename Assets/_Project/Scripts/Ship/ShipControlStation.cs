using TitansOfTheSea.Player;
using UnityEngine;
namespace TitansOfTheSea.Ship
{
    public enum ShipStationType { Wheel, SailRope, Anchor }
    public sealed class ShipControlStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private ShipSailing _ship;
        [SerializeField] private ShipStationType _type;
        private PlayerInteractor _occupant;
        public string Prompt => _occupant != null ? "Leave ship controls" : _type == ShipStationType.Anchor ? "Raise / drop anchor" : "Use " + _type;
        public void Configure(ShipSailing ship, ShipStationType type) { _ship = ship; _type = type; }
        public bool CanInteract(PlayerInteractor actor) => _ship != null && (_occupant == actor || (_occupant == null && !_ship.PlayerAtControls && Vector3.Distance(actor.transform.position, transform.position) < 3));
        public void Interact(PlayerInteractor actor)
        {
            if (_occupant == actor) { Release(); return; }
            if (!CanInteract(actor)) return;
            if (_type == ShipStationType.Anchor) { _ship.SetAnchor(!_ship.AnchorDown); return; }
            _occupant = actor; actor.SetActive(this); actor.Motor.MovementLocked = true;
            _ship.PlayerAtControls = true;
            _ship.ControlsSteering = _type == ShipStationType.Wheel;
            _ship.ControlsSails = _type == ShipStationType.SailRope;
        }
        private void Release()
        {
            if (_occupant != null) { _occupant.SetActive(null); _occupant.Motor.MovementLocked = false; }
            _occupant = null;
            if (_ship != null) { _ship.PlayerAtControls = false; _ship.ControlsSteering = _ship.ControlsSails = true; }
        }
        private void OnDisable() { if (_occupant != null) Release(); }
    }
}
