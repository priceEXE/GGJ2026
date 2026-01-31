using UnityEngine;
using UnityEditor;
using System.IO;

namespace Practice.Editor
{
    public class TextureGenerator
    {
        public static Sprite GetOrGenerateCircleSprite()
        {
            string folderPath = "Assets/Res/Textures";
            string filePath = folderPath + "/SolidCircle.png";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Check if exists
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
            if (existing != null) return existing;

            // Generate
            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size * 0.45f; // Leave some padding
            float rSquared = radius * radius;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d2 = (x - center.x) * (x - center.x) + (y - center.y) * (y - center.y);
                    if (d2 <= rSquared)
                        colors[y * size + x] = Color.white;
                    else
                        colors[y * size + x] = Color.clear;
                }
            }

            tex.SetPixels(colors);
            tex.Apply();

            // Save Key
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();

            // Set Import Settings to Sprite
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
        }
    }
}
