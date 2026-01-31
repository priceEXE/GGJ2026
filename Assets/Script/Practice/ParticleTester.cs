using UnityEngine;

namespace Practice
{
    public class ParticleTester : MonoBehaviour
    {
        [Header("Dust Ring Parameters")]
        public Vector2 ringNormal = Vector2.up;
        public float ringSize = 1.0f;
        public int ringCount = 20;
        public float ringForce = 5.0f;

        [Header("Dust Jet Parameters")]
        public Vector2 jetDirection = Vector2.right;
        public float jetSpread = 30f;
        public float jetSize = 1.0f;
        public int jetCount = 25;
        public float jetForce = 8.0f;

        [Header("Cream Jet Parameters")]
        public Vector2 creamDirection = Vector2.right;
        public Color creamColor = new Color(1.0f, 0.95f, 0.85f); // #FFF2D9
        public float creamSpread = 15f;
        public float creamSize = 1.0f;
        public int creamCount = 30;
        public float creamForce = 10.0f;

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
                ParticleManager.Instance.PlayDustRing(transform.position, ringNormal, ringSize, ringCount, ringForce);
        }

        [ContextMenu("Trigger Dust Jet")]
        public void TriggerDustJet()
        {
            if (ParticleManager.Instance != null)
                ParticleManager.Instance.PlayDustJet(transform.position, jetDirection, jetSpread, jetSize, jetCount, jetForce);
        }

        [ContextMenu("Trigger Cream Jet")]
        public void TriggerCreamJet()
        {
            if (ParticleManager.Instance != null)
                ParticleManager.Instance.PlayCreamJet(transform.position, creamDirection, creamColor, creamSpread, creamSize, creamCount, creamForce);
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
