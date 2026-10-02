using System;

namespace TitansOfTheSea.Core
{
    public enum GameState { Boot, MainMenu, Loading, Playing, Paused, Cutscene, Dead, Ending }
    public sealed class GameSession
    {
        public GameState State { get; private set; } = GameState.Boot;
        public bool AcceptsPlayerInput => State == GameState.Playing;
        public bool AdvancesWorldTime => State == GameState.Playing;
        public event Action<GameState, GameState> Changed;
        public bool Transition(GameState next)
        {
            if (State == next) return false;
            bool allowed = next == GameState.MainMenu ||
                (State == GameState.Boot && next == GameState.Loading) ||
                (State == GameState.MainMenu && next == GameState.Loading) ||
                (State == GameState.Loading && next == GameState.Playing) ||
                (State == GameState.Playing && (next == GameState.Paused || next == GameState.Cutscene || next == GameState.Dead || next == GameState.Ending)) ||
                ((State == GameState.Paused || State == GameState.Cutscene) && (next == GameState.Playing || next == GameState.Dead || next == GameState.Ending)) ||
                (State == GameState.Dead && next == GameState.Loading) ||
                (State == GameState.Ending && next == GameState.Loading);
            if (!allowed) throw new InvalidOperationException($"Invalid game state transition: {State} -> {next}");
            var previous = State;
            State = next;
            Changed?.Invoke(previous, next);
            return true;
        }
    }
}
