using UnityEngine;

namespace SimulationCourse
{
    public sealed class GateCourse : MonoBehaviour
    {
        private static readonly Color PendingColor = new(0.25f, 0.38f, 0.48f);
        private static readonly Color NextColor = new(1f, 0.68f, 0.2f);
        private static readonly Color PassedColor = new(0.22f, 0.86f, 0.62f);

        [SerializeField] private Rigidbody _vehicle;
        [SerializeField] private Gate[] _gates;
        private bool _running;

        public int Passed { get; private set; }
        public int Total => _gates.Length;
        public bool Finished => Passed >= _gates.Length;
        public float Elapsed { get; private set; }

        private void Awake()
        {
            for (int i = 0; i < _gates.Length; i++) _gates[i].Bind(this, i);
            RefreshColors();
        }

        private void Update()
        {
            if (_running && !Finished) Elapsed += Time.deltaTime;
        }

        public void OnGateEntered(Gate gate, Rigidbody body)
        {
            if (body != _vehicle || Finished || gate.Index != Passed) return;
            if (Vector3.Dot(body.linearVelocity, gate.transform.forward) <= 0f) return;
            _running = true;
            Passed++;
            RefreshColors();
        }

        private void RefreshColors()
        {
            for (int i = 0; i < _gates.Length; i++)
                _gates[i].SetColor(i < Passed ? PassedColor : i == Passed ? NextColor : PendingColor);
        }
    }
}
