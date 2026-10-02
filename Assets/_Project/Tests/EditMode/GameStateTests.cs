using NUnit.Framework;
using TitansOfTheSea.Core;

namespace TitansOfTheSea.Tests
{
    public sealed class GameStateTests
    {
        [Test] public void StateMachineControlsPauseCutsceneAndRespawnTransitions()
        {
            var session = new GameSession();
            int transitions = 0;
            session.Changed += (_, __) => transitions++;
            Assert.Throws<System.InvalidOperationException>(() => session.Transition(GameState.Playing));
            session.Transition(GameState.MainMenu);
            session.Transition(GameState.Loading);
            session.Transition(GameState.Playing);
            Assert.That(session.AcceptsPlayerInput && session.AdvancesWorldTime, Is.True);
            session.Transition(GameState.Paused);
            Assert.That(session.AcceptsPlayerInput || session.AdvancesWorldTime, Is.False);
            session.Transition(GameState.Playing);
            session.Transition(GameState.Cutscene);
            Assert.That(session.AcceptsPlayerInput, Is.False);
            session.Transition(GameState.Dead);
            session.Transition(GameState.Loading);
            session.Transition(GameState.Playing);
            session.Transition(GameState.Ending);
            Assert.That(transitions, Is.EqualTo(10));
            Assert.That(session.Transition(GameState.Ending), Is.False);
        }
    }
}
