using System;
using UnityEngine;
namespace TitansOfTheSea.Contracts
{
    public static class GameEvents
    {
        public static event Action<string, int, float, Vector3> OnShipDamaged;
        public static void RaiseShipDamaged(string shipId, int hullSection, float amount, Vector3 hitPos) => OnShipDamaged?.Invoke(shipId, hullSection, amount, hitPos);
        public static event Action<string> OnShipSunk;
        public static void RaiseShipSunk(string shipId) => OnShipSunk?.Invoke(shipId);
        public static event Action<string, int, Vector3, Vector3> OnCannonFired;
        public static void RaiseCannonFired(string shipId, int cannonIndex, Vector3 pos, Vector3 dir) => OnCannonFired?.Invoke(shipId, cannonIndex, pos, dir);
        public static event Action<Vector3, string> OnCannonballImpact;
        public static void RaiseCannonballImpact(Vector3 pos, string surfaceType) => OnCannonballImpact?.Invoke(pos, surfaceType);
        public static event Action<float, Vector3> OnPlayerHit;
        public static void RaisePlayerHit(float amount, Vector3 sourcePos) => OnPlayerHit?.Invoke(amount, sourcePos);
        public static event Action<string> OnPlayerDied;
        public static void RaisePlayerDied(string cause) => OnPlayerDied?.Invoke(cause);
        public static event Action<Vector3> OnPlayerRespawned;
        public static void RaisePlayerRespawned(Vector3 pos) => OnPlayerRespawned?.Invoke(pos);
        public static event Action<string, Vector3> OnMeleeSwing;
        public static void RaiseMeleeSwing(string weaponId, Vector3 pos) => OnMeleeSwing?.Invoke(weaponId, pos);
        public static event Action<string, Vector3, Vector3> OnGunFired;
        public static void RaiseGunFired(string weaponId, Vector3 pos, Vector3 dir) => OnGunFired?.Invoke(weaponId, pos, dir);
        public static event Action<string, Vector3> OnEnemyKilled;
        public static void RaiseEnemyKilled(string enemyId, Vector3 pos) => OnEnemyKilled?.Invoke(enemyId, pos);
        public static event Action<string, int> OnItemPicked;
        public static void RaiseItemPicked(string itemId, int count) => OnItemPicked?.Invoke(itemId, count);
        public static event Action<string, Vector3> OnTreasureDug;
        public static void RaiseTreasureDug(string treasureId, Vector3 pos) => OnTreasureDug?.Invoke(treasureId, pos);
        public static event Action<string> OnQuestChanged;
        public static void RaiseQuestChanged(string questId) => OnQuestChanged?.Invoke(questId);
        public static event Action<string> OnChapterChanged;
        public static void RaiseChapterChanged(string chapterId) => OnChapterChanged?.Invoke(chapterId);
        public static event Action<string> OnIslandEntered;
        public static void RaiseIslandEntered(string islandId) => OnIslandEntered?.Invoke(islandId);
        public static event Action<string> OnIslandLeft;
        public static void RaiseIslandLeft(string islandId) => OnIslandLeft?.Invoke(islandId);
        public static event Action<string> OnRaidStarted;
        public static void RaiseRaidStarted(string islandId) => OnRaidStarted?.Invoke(islandId);
        public static event Action<string, bool> OnRaidEnded;
        public static void RaiseRaidEnded(string islandId, bool success) => OnRaidEnded?.Invoke(islandId, success);
        public static event Action<string> OnTitanSpawned;
        public static void RaiseTitanSpawned(string titanId) => OnTitanSpawned?.Invoke(titanId);
        public static event Action<string, int> OnTitanPhaseChanged;
        public static void RaiseTitanPhaseChanged(string titanId, int phase) => OnTitanPhaseChanged?.Invoke(titanId, phase);
        public static event Action<string, Vector3> OnBuildingPlaced;
        public static void RaiseBuildingPlaced(string buildingId, Vector3 pos) => OnBuildingPlaced?.Invoke(buildingId, pos);
        public static event Action<string> OnCropHarvested;
        public static void RaiseCropHarvested(string cropId) => OnCropHarvested?.Invoke(cropId);
        public static event Action<string> OnMealCooked;
        public static void RaiseMealCooked(string mealId) => OnMealCooked?.Invoke(mealId);
        public static event Action<int, int> OnDebtChanged;
        public static void RaiseDebtChanged(int amountLeft, int daysLeft) => OnDebtChanged?.Invoke(amountLeft, daysLeft);
        public static event Action<int> OnFameChanged;
        public static void RaiseFameChanged(int newFame) => OnFameChanged?.Invoke(newFame);
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetForNewSession()
        {
            OnShipDamaged = null;
            OnShipSunk = null;
            OnCannonFired = null;
            OnCannonballImpact = null;
            OnPlayerHit = null;
            OnPlayerDied = null;
            OnPlayerRespawned = null;
            OnMeleeSwing = null;
            OnGunFired = null;
            OnEnemyKilled = null;
            OnItemPicked = null;
            OnTreasureDug = null;
            OnQuestChanged = null;
            OnChapterChanged = null;
            OnIslandEntered = null;
            OnIslandLeft = null;
            OnRaidStarted = null;
            OnRaidEnded = null;
            OnTitanSpawned = null;
            OnTitanPhaseChanged = null;
            OnBuildingPlaced = null;
            OnCropHarvested = null;
            OnMealCooked = null;
            OnDebtChanged = null;
            OnFameChanged = null;
        }
    }
}
