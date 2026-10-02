using System.IO;
using TitansOfTheSea.Ship;
using TitansOfTheSea.World;
using TitansOfTheSea.World.Sandbox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TitansOfTheSea.Core.Editor
{
    public static class BSandboxBuilder
    {
        private const string SCENE_PATH = "Assets/_Project/Scenes/B_Sandbox_Buoyancy.unity";
        public static void OpenSandbox()
        {
            if (!File.Exists(SCENE_PATH)) throw new System.InvalidOperationException("Generate the B sandbox first.");
            EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);
            EditorApplication.ExecuteMenuItem("Window/General/Game");
        }
        // CLI entry point for a newly created B project; does not alter integration scenes.
        public static void CreateBatch()
        {
            if (File.Exists(SCENE_PATH)) throw new System.InvalidOperationException("Sandbox already exists. Rebuild explicitly through the menu.");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            ConfigureLayersAndTags();
            Create();
            if (!File.Exists(SCENE_PATH)) throw new System.InvalidOperationException("Sandbox creation did not complete.");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(SCENE_PATH, true) };
        }
        private static void ConfigureLayersAndTags()
        {
            var manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = manager.FindProperty("layers");
            foreach (string name in new[] { "Water", "Terrain", "Ship", "Player", "Enemy", "Projectile", "Interactable", "Buildable", "Treasure", "Titan", "Trigger" })
            {
                bool found = false;
                for (int i = 0; i < layers.arraySize; i++) if (layers.GetArrayElementAtIndex(i).stringValue == name) found = true;
                if (found) continue;
                for (int i = 8; i < layers.arraySize; i++)
                    if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                    { layers.GetArrayElementAtIndex(i).stringValue = name; break; }
            }
            var tags = manager.FindProperty("tags");
            foreach (string name in new[] { "Ship", "Cannon", "Dig_Spot", "Merchant", "Raider" })
            {
                bool found = false;
                for (int i = 0; i < tags.arraySize; i++) if (tags.GetArrayElementAtIndex(i).stringValue == name) found = true;
                if (found) continue;
                int index = tags.arraySize; tags.InsertArrayElementAtIndex(index);
                tags.GetArrayElementAtIndex(index).stringValue = name;
            }
            manager.ApplyModifiedProperties();
        }
        [MenuItem("Titans of the Sea/Person B/Create Buoyancy Sandbox")]
        public static void Create()
        {
            if (Application.isPlaying) { Debug.LogWarning("Exit Play mode before creating the sandbox."); return; }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (File.Exists(SCENE_PATH) && !EditorUtility.DisplayDialog("Replace B sandbox?", "Rebuild only B_Sandbox_Buoyancy? Existing custom sandbox edits will be replaced.", "Rebuild", "Cancel")) return;
            Directory.CreateDirectory("Assets/_Project/Scenes");
            Directory.CreateDirectory("Assets/_Project/Data");
            Directory.CreateDirectory("Assets/_Project/Prefabs");
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("B_Bootstrap").AddComponent<GameBootstrap>();
            var ocean = new GameObject("B_MockOcean_LogicOnly").AddComponent<FlatOcean>();
            var clock = new GameObject("B_TimeOfDay").AddComponent<TimeOfDayService>();
            clock.Configure(GetWorldSettings<TimeOfDaySettings>("B_TimeOfDay"));
            var weather = new GameObject("B_WeatherLogic").AddComponent<WeatherService>();
            weather.Configure(GetWorldSettings<WeatherSettings>("B_Weather"));
            var shipSettings = GetSettings("B_ShipBuoyancy", false);
            var boxSettings = GetSettings("B_BoxBuoyancy", true);
            var ship = CreateHull("B_TestShip", new Vector3(3, 2, 0), new Vector3(3, 1.5f, 6), shipSettings);
            var box = CreateHull("B_FloatingBox", new Vector3(-4, 2, 0), new Vector3(2, 1, 2), boxSettings);
            PrefabUtility.SaveAsPrefabAsset(ship.gameObject, "Assets/_Project/Prefabs/B_TestShip.prefab");
            PrefabUtility.SaveAsPrefabAsset(box.gameObject, "Assets/_Project/Prefabs/B_FloatingBox.prefab");
            var console = new GameObject("B_DebugConsole").AddComponent<DebugCheatConsole>();
            console.Configure(ocean, new[] { ship, box });
            var camera = new GameObject("B_SandboxCamera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(12, 10, -16);
            camera.transform.LookAt(Vector3.zero);
            camera.farClipPlane = 200;
            camera.backgroundColor = new Color(0.12f, 0.2f, 0.3f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            var light = new GameObject("B_SandboxLight").AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(45, -30, 0);
            // Visible Y=0 markers are test apparatus, not a rendered ocean.
            for (int i = -2; i <= 2; i++)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marker.name = "B_ZeroWaterHeightMarker";
                marker.transform.position = new Vector3(i * 5, 0, 8);
                marker.transform.localScale = new Vector3(4, 0.03f, 0.1f);
                Object.DestroyImmediate(marker.GetComponent<Collider>());
            }
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), SCENE_PATH);
            AssetDatabase.SaveAssets();
            Debug.Log("B sandbox created. Press Play; use the development panel. Y=0 markers show flat-water height.");
        }
        private static T GetWorldSettings<T>(string name) where T : ScriptableObject
        {
            string path = "Assets/_Project/Data/" + name + ".asset";
            var settings = AssetDatabase.LoadAssetAtPath<T>(path);
            if (settings != null) return settings;
            settings = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(settings, path);
            return settings;
        }
        private static BuoyancySettings GetSettings(string name, bool box)
        {
            string path = "Assets/_Project/Data/" + name + ".asset";
            var settings = AssetDatabase.LoadAssetAtPath<BuoyancySettings>(path);
            if (settings != null) return settings;
            settings = ScriptableObject.CreateInstance<BuoyancySettings>();
            if (box)
            {
                settings.MassKg = 100;
                settings.CentreOfMass = new Vector3(0, -0.25f, 0);
                settings.LocalPoints = new[] { new Vector3(-0.8f,-0.4f,-0.8f), new Vector3(0.8f,-0.4f,-0.8f), new Vector3(-0.8f,-0.4f,0.8f), new Vector3(0.8f,-0.4f,0.8f) };
            }
            AssetDatabase.CreateAsset(settings, path);
            return settings;
        }
        private static ShipBuoyancy CreateHull(string name, Vector3 position, Vector3 size, BuoyancySettings settings)
        {
            // Unit-scale physics root prevents scaled transforms from distorting points or mass offsets.
            var root = new GameObject(name);
            root.transform.position = position;
            root.AddComponent<BoxCollider>().size = size;
            root.AddComponent<Rigidbody>();
            var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "B_PlaceholderGeometry";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = size;
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            var buoyancy = root.AddComponent<ShipBuoyancy>();
            buoyancy.Configure(settings);
            return buoyancy;
        }
    }
}
