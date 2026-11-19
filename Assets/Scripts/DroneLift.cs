using UnityEngine;
using UnityEngine.InputSystem;

namespace SimulationCourse
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class DroneLift : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float climbAcceleration = 12f;
        [SerializeField, Min(0f)] private float verticalDamping = 4f;
        [SerializeField, Min(0f)] private float maxVerticalSpeed = 5f;

        private Rigidbody _body;

        private void Awake() => _body = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            float climb = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.spaceKey.isPressed) climb += 1f;
                if (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.cKey.isPressed) climb -= 1f;
            }

            float verticalSpeed = Vector3.Dot(_body.linearVelocity, Vector3.up);
            float hoverCorrection = -verticalSpeed * verticalDamping;
            float acceleration = Physics.gravity.magnitude + hoverCorrection + climb * climbAcceleration;
            acceleration = Mathf.Clamp(acceleration, 0f, Physics.gravity.magnitude + climbAcceleration);
            if (Mathf.Abs(verticalSpeed) > maxVerticalSpeed && Mathf.Sign(acceleration - Physics.gravity.magnitude) == Mathf.Sign(verticalSpeed))
                acceleration = Physics.gravity.magnitude;

            _body.AddForce(transform.up * acceleration, ForceMode.Acceleration);
        }
    }
}
