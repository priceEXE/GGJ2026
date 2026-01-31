#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CharacterCosmetics
{
    [CustomEditor(typeof(Binarizer))]
    public class BinarizerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Binarizer binarizer = (Binarizer)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();
            
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Flash Test (0.1s, 3 frames)"))
            {
                binarizer.Flash();
            }
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Flash can only be tested in Play Mode.", MessageType.Info);
            }
            GUI.enabled = true;
        }
    }
}
#endif
