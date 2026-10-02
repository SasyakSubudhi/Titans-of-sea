using TitansOfTheSea.Player;
using TitansOfTheSea.Ship;
using TitansOfTheSea.World.Sandbox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace TitansOfTheSea.Core.Editor
{
    public static class BPlayerSandboxBuilder
    {
        public const string Path = "Assets/_Project/Scenes/B_Sandbox_Player.unity";
        public static void FinalizePlayer()
        {
            if (!System.IO.File.Exists(Path)) Create();
            EditorSceneManager.OpenScene(Path);
            var movement = AssetDatabase.LoadAssetAtPath<PlayerMovementSettings>("Assets/_Project/Data/B_PlayerMovement.asset");
            Object.FindFirstObjectByType<PlayerMotor>().Configure(movement);
            var prefab = PrefabUtility.LoadPrefabContents("Assets/_Project/Prefabs/B_Player.prefab");
            prefab.GetComponent<PlayerMotor>().Configure(movement);
            PrefabUtility.SaveAsPrefabAsset(prefab, "Assets/_Project/Prefabs/B_Player.prefab");
            PrefabUtility.UnloadPrefabContents(prefab);
            if (GameObject.Find("B_LadderLanding") == null)
            {
                var landing = new GameObject("B_LadderLanding").transform; landing.position = new Vector3(9, 3.55f, 3.5f);
                var floor = GameObject.CreatePrimitive(PrimitiveType.Cube); floor.name = "B_LadderLandingPlatform";
                floor.transform.position = new Vector3(9.5f, 3.25f, 3.5f); floor.transform.localScale = new Vector3(2, .5f, 2);
                Object.FindFirstObjectByType<Climbable>().Configure(GameObject.Find("B_LadderBottom").transform, GameObject.Find("B_LadderTop").transform, landing);
            }
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
        }
        public static void Open()
        { EditorSceneManager.OpenScene(Path); EditorApplication.ExecuteMenuItem("Window/General/Game"); }
        [MenuItem("Titans of the Sea/Person B/Create Player Sandbox")]
        public static void Create()
        {
            if (Application.isPlaying || System.IO.File.Exists(Path)) throw new System.InvalidOperationException("Preserve the existing scene; create only outside Play mode.");
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(BSailingSandboxBuilder.Path);
            var oldInput = Object.FindFirstObjectByType<SandboxShipControls>();
            if (oldInput != null) Object.DestroyImmediate(oldInput.gameObject);
            var ship = Object.FindFirstObjectByType<ShipSailing>();
            var settings = ScriptableObject.CreateInstance<PlayerMovementSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/_Project/Data/B_PlayerMovement.asset");
            var player = new GameObject("B_Player"); player.layer = LayerMask.NameToLayer("Player");
            player.transform.position = ship.transform.TransformPoint(new Vector3(0, .85f, -1));
            player.AddComponent<CharacterController>();
            var motor = player.AddComponent<PlayerMotor>(); motor.Configure(settings);
            var eye = new GameObject("B_InteractionEye").transform;
            eye.SetParent(player.transform, false); eye.localPosition = new Vector3(0, 1.5f, 0);
            player.AddComponent<PlayerInteractor>().Configure(eye);
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "B_PlayerStandIn"; visual.transform.SetParent(player.transform, false);
            visual.transform.localPosition = new Vector3(0, .9f, 0); visual.transform.localScale = new Vector3(.5f, .8f, .5f);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            PrefabUtility.SaveAsPrefabAsset(player, "Assets/_Project/Prefabs/B_Player.prefab");
            new GameObject("B_PlayerInputStandIn").AddComponent<SandboxPlayerControls>().Configure(motor, ship.transform);
            Station(ship, ShipStationType.Wheel, new Vector3(0, 1.5f, 1));
            Station(ship, ShipStationType.SailRope, new Vector3(-1, 1.5f, -1));
            Station(ship, ShipStationType.Anchor, new Vector3(1, 1.5f, -1));
            var shore = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shore.name = "B_ShoreStandIn"; shore.transform.position = new Vector3(8, 0, 0); shore.transform.localScale = new Vector3(6, 1, 12);
            var step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.name = "B_StepTest"; step.transform.position = new Vector3(8, .6f, 2); step.transform.localScale = new Vector3(2, .2f, 1);
            var ladder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ladder.name = "B_LadderStandIn"; ladder.transform.position = new Vector3(8, 2, 4); ladder.transform.localScale = new Vector3(.5f, 3, .1f);
            ladder.GetComponent<Collider>().isTrigger = true;
            var bottom = new GameObject("B_LadderBottom").transform; bottom.position = new Vector3(8, .55f, 3.5f);
            var top = new GameObject("B_LadderTop").transform; top.position = new Vector3(8, 3.5f, 3.5f);
            ladder.AddComponent<Climbable>().Configure(bottom, top);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), Path);
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.Add(new EditorBuildSettingsScene(Path, true)); EditorBuildSettings.scenes = scenes.ToArray(); AssetDatabase.SaveAssets();
        }
        private static void Station(ShipSailing ship, ShipStationType type, Vector3 position)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube); obj.name = "B_" + type + "_InteractionStandIn";
            obj.transform.SetParent(ship.transform, false); obj.transform.localPosition = position;
            obj.transform.localScale = new Vector3(.3f, 2, .3f); obj.GetComponent<Collider>().isTrigger = true;
            obj.AddComponent<ShipControlStation>().Configure(ship, type);
        }
    }
}
