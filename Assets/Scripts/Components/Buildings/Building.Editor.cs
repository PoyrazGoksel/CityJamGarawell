using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Components.Buildings
{
    public partial class Building
    {
        public void SetID(int newID)
        {
            _id = newID;
        }
        
        [Button]
        public void SavePreviewAsPNG()
        {
            Texture2D preview = AssetPreview.GetAssetPreview(gameObject);
            
            byte[] pngData = preview.EncodeToPNG();
        
            if (pngData != null)
            {
                string filePath = Path.Combine(Application.dataPath, gameObject.name + ".png");

                File.WriteAllBytes(filePath, pngData);
                Debug.Log($"Preview saved as PNG at: {filePath}");

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Failed to encode texture to PNG.");
            }
        }
    }
}
#endif