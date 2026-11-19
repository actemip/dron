using UnityEngine;
using UnityEngine.InputSystem;

namespace SimulationCourse
{
    public sealed class DroneCourseHUD : MonoBehaviour
    {
        [SerializeField] private GateCourse _course;
        [SerializeField] private Rigidbody _drone;
        private GUIStyle _panel, _title, _body, _status;

        private void OnGUI()
        {
            EnsureStyles();
            GUILayout.BeginArea(new Rect(24, 22, 380, 222), _panel);
            GUILayout.Label("DRONE COURSE  /  FLIGHT SCHOOL", _title);
            GUILayout.Space(5);
            GUILayout.Label($"GATES     {_course.Passed:00} / {_course.Total:00}       TIME     {_course.Elapsed:0.0}s", _body);
            GUILayout.Label(_course.Finished ? "COURSE COMPLETE  •  CLEAN RUN" : "NEXT GATE  •  FOLLOW THE LIGHT", _status);
            GUILayout.Space(9);
            GUILayout.Label("W / S     pitch & move forward / back", _body);
            GUILayout.Label("A / D     roll     •     Q / E     yaw", _body);
            GUILayout.Label("SPACE     rise     •     CTRL / C     descend", _body);
            GUILayout.Label("R     reset position", _body);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(24, Screen.height - 52, 300, 34));
            GUILayout.Label($"ALT  {_drone.position.y:0.0} m     VERTICAL  {_drone.linearVelocity.y:+0.0;-0.0;0.0} m/s", _body);
            GUILayout.EndArea();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                _drone.position = new Vector3(0f, 2.5f, -5f);
                _drone.rotation = Quaternion.identity;
                _drone.linearVelocity = Vector3.zero;
                _drone.angularVelocity = Vector3.zero;
            }
        }

        private void EnsureStyles()
        {
            if (_panel != null) return;
            _panel = new GUIStyle(GUI.skin.box) { padding = new RectOffset(18, 18, 14, 14), alignment = TextAnchor.UpperLeft };
            _panel.normal.background = MakeTexture(new Color(0.035f, 0.07f, 0.11f, 0.88f));
            _title = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, normal = { textColor = new Color(0.36f, 0.91f, 0.78f) } };
            _body = new GUIStyle(GUI.skin.label) { fontSize = 13, normal = { textColor = new Color(0.82f, 0.88f, 0.93f) } };
            _status = new GUIStyle(_body) { fontStyle = FontStyle.Bold, normal = { textColor = new Color(1f, 0.72f, 0.35f) } };
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
