using UnityEngine;
using UnityEngine.InputSystem;

namespace SimulationCourse
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class DroneManualAttitude : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float pitchAcceleration = 3f;
        [SerializeField, Min(0f)] private float rollAcceleration = 3f;
        [SerializeField, Min(0f)] private float yawAcceleration = 2f;

        private Rigidbody _body;

        private void Awake() => _body = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            if (Keyboard.current == null) return;
            float pitch = Axis(Keyboard.current.wKey.isPressed, Keyboard.current.sKey.isPressed);
            float roll = Axis(Keyboard.current.dKey.isPressed, Keyboard.current.aKey.isPressed);
            float yaw = Axis(Keyboard.current.eKey.isPressed, Keyboard.current.qKey.isPressed);
            _body.AddRelativeTorque(new Vector3(pitch * pitchAcceleration, yaw * yawAcceleration, -roll * rollAcceleration), ForceMode.Acceleration);
        }

        private static float Axis(bool positive, bool negative) => (positive ? 1f : 0f) - (negative ? 1f : 0f);
    }
}
