using System.Collections.Generic;
using SimulationCourse;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimulationCourse.Editor
{
    public static class DroneCourseSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/DroneCourse.unity";

        [MenuItem("Simulation Course/Build Drone Course Scene")]
        public static void BuildScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.42f, 0.48f, 0.56f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.72f, 0.79f, 0.84f);
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.006f;

            Material groundMat = Material("Ground", new Color(0.13f, 0.19f, 0.24f));
            Material deckMat = Material("Deck", new Color(0.19f, 0.27f, 0.32f));
            Material droneMat = Material("Drone enamel", new Color(0.13f, 0.83f, 0.72f), 0.35f, 0.45f);
            Material darkMat = Material("Graphite", new Color(0.045f, 0.075f, 0.10f), 0.15f, 0.7f);
            Material rotorMat = Material("Rotor", new Color(0.9f, 0.95f, 0.96f), 0.2f, 0.3f);
            Material gateMat = Material("Gate pending", new Color(0.25f, 0.38f, 0.48f), 0.25f, 0.4f);
            Material trimMat = Material("Gate trim", new Color(0.14f, 0.29f, 0.37f), 0.2f, 0.45f);

            GameObject environment = new("Environment");
            Cube("Flight deck", environment.transform, new Vector3(0, -0.22f, 33f), new Vector3(42, 0.4f, 100), deckMat, true);
            Cube("Ground", environment.transform, new Vector3(0, -1.2f, 33f), new Vector3(240, 1.4f, 150), groundMat, true);
            for (int x = -18; x <= 18; x += 3)
                Cube("Deck marking", environment.transform, new Vector3(x, -0.009f, 32f), new Vector3(0.025f, 0.02f, 86f), trimMat);
            for (int z = -10; z <= 75; z += 5)
                Cube("Deck marking", environment.transform, new Vector3(0, -0.008f, z), new Vector3(38f, 0.02f, 0.025f), trimMat);

            var lightObject = new GameObject("Key Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            light.color = new Color(0.81f, 0.9f, 1f);
            lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            var drone = new GameObject("Drone");
            drone.transform.position = new Vector3(0f, 2.5f, -5f);
            var body = drone.AddComponent<Rigidbody>();
            body.mass = 1f;
            body.linearDamping = 0.08f;
            body.angularDamping = 1f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            drone.AddComponent<BoxCollider>().size = new Vector3(1.1f, 0.36f, 1.1f);
            Cube("Central shell", drone.transform, Vector3.zero, new Vector3(1.35f, 0.34f, 1.35f), droneMat);
            Cube("Nose marker", drone.transform, new Vector3(0, 0.22f, 0.45f), new Vector3(0.18f, 0.08f, 0.2f), rotorMat);
            Vector3[] arms = { new(-0.73f, 0.04f, -0.73f), new(0.73f, 0.04f, -0.73f), new(-0.73f, 0.04f, 0.73f), new(0.73f, 0.04f, 0.73f) };
            foreach (Vector3 arm in arms)
            {
                Cube("Motor arm", drone.transform, arm * 0.48f, new Vector3(0.95f, 0.09f, 0.12f), darkMat).transform.localRotation = Quaternion.Euler(0, 45f * Mathf.Sign(arm.x * arm.z), 0);
                Cube("Motor", drone.transform, arm, new Vector3(0.34f, 0.18f, 0.34f), darkMat);
                Cube("Rotor blade", drone.transform, arm + Vector3.up * 0.14f, new Vector3(0.72f, 0.035f, 0.1f), rotorMat);
            }
            drone.AddComponent<DroneLift>();
            drone.AddComponent<DroneManualAttitude>();
            drone.AddComponent<DroneAutoLevel>();

            var gatesRoot = new GameObject("Gate Corridor");
            var gates = new List<Gate>();
            float[] heights = { 2.5f, 3.4f, 2.7f, 3.6f, 2.9f };
            for (int i = 0; i < heights.Length; i++)
                gates.Add(CreateGate(gatesRoot.transform, i, new Vector3(0, heights[i], 7f + i * 15f), gateMat, trimMat));

            var courseRoot = new GameObject("Gate Course");
            var course = courseRoot.AddComponent<GateCourse>();
            var courseSO = new SerializedObject(course);
            courseSO.FindProperty("_vehicle").objectReferenceValue = body;
            SerializedProperty gateArray = courseSO.FindProperty("_gates");
            gateArray.arraySize = gates.Count;
            for (int i = 0; i < gates.Count; i++) gateArray.GetArrayElementAtIndex(i).objectReferenceValue = gates[i];
            courseSO.ApplyModifiedPropertiesWithoutUndo();

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 220f;
            cameraObject.AddComponent<AudioListener>();
            var follow = cameraObject.AddComponent<DroneCameraFollow>();
            var followSO = new SerializedObject(follow);
            followSO.FindProperty("_target").objectReferenceValue = drone.transform;
            followSO.ApplyModifiedPropertiesWithoutUndo();
            var hudObject = new GameObject("Flight HUD");
            var hud = hudObject.AddComponent<DroneCourseHUD>();
            var hudSO = new SerializedObject(hud);
            hudSO.FindProperty("_course").objectReferenceValue = course;
            hudSO.FindProperty("_drone").objectReferenceValue = body;
            hudSO.ApplyModifiedPropertiesWithoutUndo();

            var startMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            startMarker.name = "Launch pad";
            startMarker.transform.position = new Vector3(0f, 0.02f, -5f);
            startMarker.transform.localScale = new Vector3(3f, 0.025f, 3f);
            startMarker.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.DestroyImmediate(startMarker.GetComponent<Collider>());

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Selection.activeGameObject = drone;
            Debug.Log($"Drone course created at {ScenePath}: one Rigidbody, five gates, controls and HUD.");
        }

        private static Gate CreateGate(Transform parent, int index, Vector3 position, Material frameMaterial, Material trimMaterial)
        {
            var root = new GameObject($"Frame {index + 1:00}");
            root.transform.SetParent(parent);
            root.transform.position = position;
            var renderers = new List<Renderer>();
            renderers.Add(Cube("Left post", root.transform, new Vector3(-2.625f, 0, 0), new Vector3(0.25f, 4.5f, 0.35f), frameMaterial, true).GetComponent<Renderer>());
            renderers.Add(Cube("Right post", root.transform, new Vector3(2.625f, 0, 0), new Vector3(0.25f, 4.5f, 0.35f), frameMaterial, true).GetComponent<Renderer>());
            renderers.Add(Cube("Top beam", root.transform, new Vector3(0, 2.125f, 0), new Vector3(5.5f, 0.25f, 0.35f), frameMaterial, true).GetComponent<Renderer>());
            renderers.Add(Cube("Bottom beam", root.transform, new Vector3(0, -2.125f, 0), new Vector3(5.5f, 0.25f, 0.35f), frameMaterial, true).GetComponent<Renderer>());
            Cube("Gate marker", root.transform, new Vector3(0, 0, 0.22f), new Vector3(5.4f, 0.035f, 0.035f), trimMaterial);
            var trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(5f, 4f, 0.55f);
            var gate = root.AddComponent<Gate>();
            var so = new SerializedObject(gate);
            SerializedProperty frameArray = so.FindProperty("_frame");
            frameArray.arraySize = renderers.Count;
            for (int i = 0; i < renderers.Count; i++) frameArray.GetArrayElementAtIndex(i).objectReferenceValue = renderers[i];
            so.ApplyModifiedPropertiesWithoutUndo();
            return gate;
        }

        private static GameObject Cube(string name, Transform parent, Vector3 localPosition, Vector3 scale, Material material, bool collider = false)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            if (!collider) Object.DestroyImmediate(cube.GetComponent<Collider>());
            return cube;
        }

        private static Material Material(string name, Color color, float metallic = 0f, float smoothness = 0.25f)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            const string folder = "Assets/Materials";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "Materials");
            string path = $"{folder}/{name.Replace(' ', '_')}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }
            material.shader = shader;
            material.color = color;
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
