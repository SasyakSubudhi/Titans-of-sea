namespace TitansOfTheSea.Contracts
{
    public interface IUiNotifier
    {
        void ShowToast(string text, float seconds = 3f);
        void ShowObjective(string text);
        void ShowDialogue(string speaker, string text, System.Action onClosed);
        void OpenShop(string merchantId);
        void OpenCraftingMenu(string stationId);
        void OpenBuildMenu();
        void ShowDeathScreen(System.Action onRespawnConfirmed);
        void ShowDamageDirection(UnityEngine.Vector3 worldSourcePos);
    }
}
