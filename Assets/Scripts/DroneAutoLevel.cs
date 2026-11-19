using UnityEngine;

namespace SimulationCourse
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class DroneAutoLevel : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float returnGain = 8f;
        [SerializeField, Min(0f)] private float dampingGain = 3f;

        private Rigidbody _body;
        private void Awake() => _body = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            if (Vector3.Dot(transform.up, Vector3.up) <= 0f) return;
            Quaternion error = Quaternion.Normalize(Quaternion.FromToRotation(transform.up, Vector3.up));
            error.ToAngleAxis(out float angleDegrees, out Vector3 axis);
            if (angleDegrees > 180f) angleDegrees -= 360f;
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            Vector3 tiltVelocity = Vector3.ProjectOnPlane(_body.angularVelocity, transform.up);
            Vector3 correction = axis * (angleRadians * returnGain) - tiltVelocity * dampingGain;
            _body.AddTorque(correction, ForceMode.Acceleration);
        }
    }
}
