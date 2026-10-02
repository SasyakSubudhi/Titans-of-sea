using TitansOfTheSea.Contracts;
using UnityEngine;

namespace TitansOfTheSea.Core
{
    [DefaultExecutionOrder(-900)]
    public sealed class GameStateController : MonoBehaviour
    {
        [SerializeField] private bool _sandboxStartsPlaying = true;
        public GameSession Session { get; } = new GameSession();
        private float _previousTimeScale;
        private IPlayerInput _controlledInput;
        private bool _previousInputEnabled;
        private void OnEnable()
        {
            _previousTimeScale = Time.timeScale;
            Services.Register(Session);
            Session.Changed += Apply;
            Services.Changed += ServiceChanged;
            Apply(Session.State, Session.State);
        }
        private void Start()
        {
            if (_sandboxStartsPlaying) { Session.Transition(GameState.Loading); Session.Transition(GameState.Playing); }
            else Session.Transition(GameState.MainMenu);
        }
        private void ServiceChanged(System.Type type)
        {
            if (type == typeof(IPlayerInput)) Apply(Session.State, Session.State);
        }
        private void Apply(GameState previous, GameState current)
        {
            Time.timeScale = current == GameState.Playing || current == GameState.Cutscene ? 1 : 0;
            if (Services.TryGet<IPlayerInput>(out var input))
            {
                if (!ReferenceEquals(_controlledInput, input))
                {
                    RestoreInput();
                    _controlledInput = input;
                    _previousInputEnabled = input.Enabled;
                }
                input.Enabled = Session.AcceptsPlayerInput;
            }
        }
        private void RestoreInput()
        {
            if (_controlledInput != null) _controlledInput.Enabled = _previousInputEnabled;
            _controlledInput = null;
        }
        private void OnDisable()
        {
            Services.Changed -= ServiceChanged;
            Session.Changed -= Apply;
            Services.Unregister(Session);
            RestoreInput();
            Time.timeScale = _previousTimeScale;
        }
    }
}
