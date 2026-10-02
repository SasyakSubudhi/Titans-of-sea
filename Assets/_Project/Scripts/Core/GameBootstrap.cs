using UnityEngine;

namespace TitansOfTheSea.Core
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        // Static registries reset at SubsystemRegistration, including when domain reload is disabled.
        // Providers own registration/removal. Never clear another additive scene's services here.
        [SerializeField] private bool _logStartup = true;
        private void Start()
        {
            if (_logStartup) Debug.Log("Titans of the Sea — B sandbox ready.\n" + Services.Describe(), this);
        }
    }
}
