using UnityEngine;

namespace SimulationCourse
{
    public sealed class DroneCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new(0f, 7f, -12f);
        [SerializeField, Min(0.1f)] private float _smooth = 4f;

        private void LateUpdate()
        {
            if (_target == null) return;
            Vector3 desired = _target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * _smooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.position + Vector3.up * 1.5f - transform.position), Time.deltaTime * _smooth);
        }
    }
}
