#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class InputManagerConfigurator : EditorWindow
{
    [MenuItem("Tools/GGJ2026/一键配置 InputManager")]
    public static void SetupInputManager()
    {
        SerializedObject inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axes = inputManager.FindProperty("m_Axes");

        // 清空现有轴（或者你可以保留，这里为了纯净我先不删，追加）
        // axes.ClearArray();
        
        // 添加 Joy1-Joy4 的 X/Y 轴
        AddAxis(axes, "Joy1 X", 0, 1); // X Axis
        AddAxis(axes, "Joy1 Y", 0, 2, true); // Y Axis (通常需要反转)
        
        AddAxis(axes, "Joy2 X", 1, 1);
        AddAxis(axes, "Joy2 Y", 1, 2, true);
        
        AddAxis(axes, "Joy3 X", 2, 1);
        AddAxis(axes, "Joy3 Y", 2, 2, true);
        
        AddAxis(axes, "Joy4 X", 3, 1);
        AddAxis(axes, "Joy4 Y", 3, 2, true);

        inputManager.ApplyModifiedProperties();
        Debug.Log("InputManager 配置完成！现在脚本可以读取 Joy1 X 等轴了。");
    }

    private static void AddAxis(SerializedProperty axes, string name, int joyNum, int axisNum, bool invert = false)
    {
        // 检查是否存在
        for (int i = 0; i < axes.arraySize; i++)
        {
            SerializedProperty axis = axes.GetArrayElementAtIndex(i);
            if (axis.FindPropertyRelative("m_Name").stringValue == name)
            {
                // 已存在，暂不修改
                return;
            }
        }

        axes.InsertArrayElementAtIndex(axes.arraySize);
        SerializedProperty newAxis = axes.GetArrayElementAtIndex(axes.arraySize - 1);

        newAxis.FindPropertyRelative("m_Name").stringValue = name;
        newAxis.FindPropertyRelative("descriptiveName").stringValue = "";
        newAxis.FindPropertyRelative("descriptiveNegativeName").stringValue = "";
        newAxis.FindPropertyRelative("negativeButton").stringValue = "";
        newAxis.FindPropertyRelative("positiveButton").stringValue = "";
        newAxis.FindPropertyRelative("altNegativeButton").stringValue = "";
        newAxis.FindPropertyRelative("altPositiveButton").stringValue = "";
        newAxis.FindPropertyRelative("gravity").floatValue = 0;
        newAxis.FindPropertyRelative("dead").floatValue = 0.19f;
        newAxis.FindPropertyRelative("sensitivity").floatValue = 1;
        newAxis.FindPropertyRelative("snap").boolValue = false;
        newAxis.FindPropertyRelative("invert").boolValue = invert;
        
        // 关键设置
        newAxis.FindPropertyRelative("type").intValue = 2; // Joystick Axis
        newAxis.FindPropertyRelative("axis").intValue = axisNum - 1; // 0=X, 1=Y
        newAxis.FindPropertyRelative("joyNum").intValue = joyNum + 1; // 1-based Joystick
    }
}
#endif
