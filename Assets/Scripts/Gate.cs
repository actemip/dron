using UnityEngine;

namespace SimulationCourse
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Gate : MonoBehaviour
    {
        [SerializeField] private Renderer[] _frame;
        private GateCourse _course;
        public int Index { get; private set; }

        public void Bind(GateCourse course, int index)
        {
            _course = course;
            Index = index;
            GetComponent<BoxCollider>().isTrigger = true;
        }

        public void SetColor(Color color)
        {
            foreach (Renderer part in _frame)
                if (part != null) part.material.color = color;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_course == null || other.attachedRigidbody == null) return;
            _course.OnGateEntered(this, other.attachedRigidbody);
        }
    }
}
