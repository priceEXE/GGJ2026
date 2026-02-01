using UnityEngine;

namespace Practice
{
    public class ParticleTester : MonoBehaviour
    {
        [Header("Dust Ring Parameters")]
        public Vector2 ringNormal = Vector2.up;
        public float ringSize = 0.5f;
        public int ringCount = 5;
        public float ringForce = 3.0f;

        [Header("Dust Jet Parameters")]
        public Vector2 jetDirection = Vector2.right;
        public float jetSpread = 15f;
        public float jetSize = 0.5f;
        public int jetCount = 15;
        public float jetForce = 20.0f;

        [Header("Cream Jet Parameters")]
        public Vector2 creamDirection = Vector2.right;
        public Color creamColor = new Color(1.0f, 0.95f, 0.85f); // #FFF2D9
        public float creamSpread = 10f;
        public float creamSize = 0.2f;
        public int creamCount = 10;
        public float creamForce = 30.0f;

        [Header("Test Controls")]
        public KeyCode ringKey = KeyCode.T;
        public KeyCode jetKey = KeyCode.J;
        public KeyCode creamKey = KeyCode.C;

        private void Update()
        {
            if (Input.GetKeyDown(ringKey)) TriggerDustRing();
            if (Input.GetKeyDown(jetKey)) TriggerDustJet();
            if (Input.GetKeyDown(creamKey)) TriggerCreamJet();
        }

        [ContextMenu("Trigger Dust Ring")]
        public void TriggerDustRing()
        {
            if (ParticleManager.Instance != null)
                ParticleManager.Instance.SpawnDustRing(transform.position, ringNormal, ringCount);
        }

        [ContextMenu("Trigger Dust Jet")]
        public void TriggerDustJet()
        {
            if (ParticleManager.Instance != null)
                ParticleManager.Instance.SpawnDustJet(transform.position, jetDirection);
        }

        [ContextMenu("Trigger Cream Jet")]
        public void TriggerCreamJet()
        {
            if (ParticleManager.Instance != null)
                ParticleManager.Instance.SpawnCreamJet(transform.position, creamDirection, creamForce);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, (Vector3)ringNormal.normalized * 2f);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, (Vector3)jetDirection.normalized * 2f);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, (Vector3)creamDirection.normalized * 2f);
        }
    }
}
