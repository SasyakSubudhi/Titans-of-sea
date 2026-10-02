using TitansOfTheSea.Ship;
using TitansOfTheSea.World;
using TitansOfTheSea.World.Sandbox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TitansOfTheSea.Core.Editor
{
    public static class BSailingSandboxBuilder
    {
        public const string Path = "Assets/_Project/Scenes/B_Sandbox_Sailing.unity";
        public static void FinalizePrototype()
        {
            if (!System.IO.File.Exists(Path)) Create();
            EditorSceneManager.OpenScene(Path);
            if (Object.FindFirstObjectByType<GameStateController>() == null)
                new GameObject("B_GameStates").AddComponent<GameStateController>();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            AssetDatabase.SaveAssets();
        }
        [MenuItem("Titans of the Sea/Person B/Create Sailing Sandbox")]
        public static void Create()
        {
            if (Application.isPlaying) throw new System.InvalidOperationException("Exit Play mode first.");
            if (System.IO.File.Exists(Path)) throw new System.InvalidOperationException("Sailing sandbox exists; preserve edits.");
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("B_Bootstrap").AddComponent<GameBootstrap>();
            var ocean = new GameObject("B_MockOcean").AddComponent<FlatOcean>();
            new GameObject("B_Clock").AddComponent<TimeOfDayService>().Configure(AssetDatabase.LoadAssetAtPath<TimeOfDaySettings>("Assets/_Project/Data/B_TimeOfDay.asset"));
            new GameObject("B_Weather").AddComponent<WeatherService>().Configure(AssetDatabase.LoadAssetAtPath<WeatherSettings>("Assets/_Project/Data/B_Weather.asset"));
            var settings = ScriptableObject.CreateInstance<SailingSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/_Project/Data/B_SloopSailing.asset");
            var hull = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/B_TestShip.prefab"));
            hull.name = "B_SailingSloop";
            hull.transform.position = new Vector3(0, 2, 0);
            var sailing = hull.AddComponent<ShipSailing>();
            sailing.Configure(settings);
            PrefabUtility.SaveAsPrefabAsset(hull, "Assets/_Project/Prefabs/B_SailingSloop.prefab");
            new GameObject("B_InputStandIn").AddComponent<SandboxShipControls>().Configure(sailing);
            new GameObject("B_Console").AddComponent<DebugCheatConsole>().Configure(ocean, new[] { hull.GetComponent<ShipBuoyancy>() });
            var camera = new GameObject("B_TestOverviewCamera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(25, 45, -40);
            camera.transform.LookAt(new Vector3(0, 0, 25));
            camera.farClipPlane = 400;
            var light = new GameObject("B_TestLight").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
            for (int z = 0; z <= 100; z += 10)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marker.name = "B_DistanceMarker_" + z;
                marker.transform.position = new Vector3(8, 0, z);
                marker.transform.localScale = new Vector3(1, .1f, 1);
                Object.DestroyImmediate(marker.GetComponent<Collider>());
            }
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), Path);
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.Add(new EditorBuildSettingsScene(Path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets();
        }
    }
}
