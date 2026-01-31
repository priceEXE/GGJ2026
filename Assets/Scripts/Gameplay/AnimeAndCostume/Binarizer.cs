using UnityEngine;
using System.Collections.Generic;

namespace CharacterCosmetics
{
    /// <summary>
    /// Binarizer - Controls color states for all child SpriteRenderers and MeshRenderers (Spine)
    /// 0: No Effect (Original)
    /// 1: All Black (Silhouette)
    /// 2: All White (Silhouette)
    /// </summary>
    public class Binarizer : MonoBehaviour
    {
        [Range(0, 2)]
        [Tooltip("0: No Effect, 1: Black, 2: White")]
        public int mode = 0;

        private int lastMode = -1;
        private static Material solidColorMat;
        
        // Data to restore
        private class RendererData
        {
            public Color color;
            public Material[] materials;  // Changed to array to support MeshRenderer
            public bool isSpriteRenderer;
        }
        private Dictionary<Renderer, RendererData> originalData = new Dictionary<Renderer, RendererData>();

        private void Start()
        {
            EnsureMaterial();
            UpdateRenderers();
        }

        private void EnsureMaterial()
        {
            if (solidColorMat == null)
            {
                Shader s = Shader.Find("Custom/SolidColorSprite");
                if (s != null)
                {
                    solidColorMat = new Material(s);
                    solidColorMat.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    Debug.LogError("[Binarizer] Missing 'Custom/SolidColorSprite' shader!");
                }
            }
        }

        private void Update()
        {
            if (mode != lastMode)
            {
                ApplyEffect();
                lastMode = mode;
            }
        }

        [ContextMenu("Refresh List")]
        public void UpdateRenderers()
        {
            originalData.Clear();
            
            // Handle SpriteRenderers
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in spriteRenderers)
            {
                if (!originalData.ContainsKey(r))
                {
                    originalData[r] = new RendererData 
                    { 
                        color = r.color, 
                        materials = new Material[] { r.sharedMaterial },
                        isSpriteRenderer = true
                    };
                }
            }
            
            // Handle MeshRenderers (Spine)
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in meshRenderers)
            {
                if (!originalData.ContainsKey(r))
                {
                    originalData[r] = new RendererData 
                    { 
                        color = Color.white,  // MeshRenderer doesn't have a color property
                        materials = r.sharedMaterials,
                        isSpriteRenderer = false
                    };
                }
            }
            
            ApplyEffect();
        }

        private void ApplyEffect()
        {
            EnsureMaterial();
            
            // Process SpriteRenderers
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in spriteRenderers)
            {
                if (!originalData.ContainsKey(r))
                {
                    originalData[r] = new RendererData 
                    { 
                        color = r.color, 
                        materials = new Material[] { r.sharedMaterial },
                        isSpriteRenderer = true
                    };
                }

                if (mode == 0)
                {
                    r.color = originalData[r].color;
                    r.sharedMaterial = originalData[r].materials[0];
                }
                else
                {
                    r.sharedMaterial = solidColorMat;
                    r.color = (mode == 1) ? Color.black : Color.white;
                }
            }
            
            // Process MeshRenderers (Spine)
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in meshRenderers)
            {
                if (!originalData.ContainsKey(r))
                {
                    originalData[r] = new RendererData 
                    { 
                        color = Color.white, 
                        materials = r.sharedMaterials,
                        isSpriteRenderer = false
                    };
                }

                if (mode == 0)
                {
                    r.sharedMaterials = originalData[r].materials;
                }
                else
                {
                    // Create material instances with proper color for all mesh slots
                    Color targetColor = (mode == 1) ? Color.black : Color.white;
                    Material[] solidMats = new Material[r.sharedMaterials.Length];
                    for (int i = 0; i < solidMats.Length; i++)
                    {
                        // Create a new material instance
                        solidMats[i] = new Material(solidColorMat);
                        // Set the _Color property for the shader
                        solidMats[i].SetColor("_Color", targetColor);
                    }
                    r.materials = solidMats;  // Use .materials to apply instances per-renderer
                }
            }
        }

        [ContextMenu("Test Flash")]
        public void TestFlash()
        {
            Flash();
        }

        /// <summary>
        /// Flash effect - alternates between white and black
        /// </summary>
        /// <param name="interval">Duration of each flash state in seconds (default: 0.1)</param>
        /// <param name="count">Number of flashes (default: 3)</param>
        public void Flash(float interval = 0.1f, int count = 3)
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[Binarizer] Flash can only be called in Play mode.");
                return;
            }
            
            StopAllCoroutines(); // Stop any ongoing flash
            StartCoroutine(FlashCoroutine(interval, count));
        }

        private System.Collections.IEnumerator FlashCoroutine(float interval, int count)
        {
            int originalMode = mode;
            
            for (int i = 0; i < count; i++)
            {
                // Alternate: White (even), Black (odd)
                mode = (i % 2 == 0) ? 2 : 1;
                ApplyEffect();
                yield return new UnityEngine.WaitForSeconds(interval);
            }
            
            // Restore original mode
            mode = originalMode;
            ApplyEffect();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                ApplyEffect();
            }
        }
    }
}

