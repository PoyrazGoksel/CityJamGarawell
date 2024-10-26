#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components.Buildings
{
    public partial class BuildingCollider
    {
        [Button]
        private void EncapsulateMeshRenderers(Transform parentTransform)
        {
            MeshRenderer[] meshRenderers = parentTransform.GetComponentsInChildren<MeshRenderer>();

            if (meshRenderers.Length == 0)
            {
                Debug.LogWarning("No MeshRenderers found under the specified parent.");
                return;
            }

            Bounds combinedBounds = meshRenderers[0].bounds;

            for (int i = 1; i < meshRenderers.Length; i++)
            {
                combinedBounds.Encapsulate(meshRenderers[i].bounds);
            }

            BoxCollider boxCollider = GetComponent<BoxCollider>();

            boxCollider.center = combinedBounds.center - parentTransform.position;
            boxCollider.size = combinedBounds.size;
        }
    }
}
#endif