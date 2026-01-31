using UnityEngine;
using UnityEditor;
using Spine.Unity;
using System.IO;
using System.Text;

namespace Gameplay.AnimeAndCostume.Editor
{
    /// <summary>
    /// Spine 枚举生成工具
    /// 右键 SkeletonDataAsset 生成统一的插槽和动画枚举
    /// </summary>
    public static class SpineEnumGenerator
    {
        // 生成的枚举文件固定路径
        private const string OUTPUT_PATH = "Assets/Scripts/Gameplay/AnimeAndCostume/SpineEnums.cs";

        [MenuItem("Assets/Spine/生成枚举到 SpineEnums.cs")]
        private static void GenerateEnums()
        {
            Object selected = Selection.activeObject;
            if (selected == null || !(selected is SkeletonDataAsset))
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个 SkeletonDataAsset！", "确定");
                return;
            }

            SkeletonDataAsset asset = selected as SkeletonDataAsset;
            var skeletonData = asset.GetSkeletonData(true);

            if (skeletonData == null)
            {
                EditorUtility.DisplayDialog("错误", "无法加载 SkeletonData！", "确定");
                return;
            }

            // 收集插槽名称
            StringBuilder slotsEnum = new StringBuilder();
            slotsEnum.AppendLine("        None = 0,");
            for (int i = 0; i < skeletonData.Slots.Count; i++)
            {
                string slotName = skeletonData.Slots.Items[i].Name;
                string enumName = SanitizeName(slotName);
                slotsEnum.AppendLine($"        {enumName} = {i + 1},");
            }

            // 收集动画名称
            StringBuilder animsEnum = new StringBuilder();
            animsEnum.AppendLine("        None = 0,");
            for (int i = 0; i < skeletonData.Animations.Count; i++)
            {
                string animName = skeletonData.Animations.Items[i].Name;
                string enumName = SanitizeName(animName);
                animsEnum.AppendLine($"        {enumName} = {i + 1},");
            }

            // 生成完整代码
            string code = $@"// 此文件由 SpineEnumGenerator 自动生成
// 请勿手动修改
// 来源: {asset.name}

namespace Gameplay.AnimeAndCostume
{{
    /// <summary>
    /// Spine 插槽枚举
    /// </summary>
    public enum SpineSlots
    {{
{slotsEnum}    }}

    /// <summary>
    /// Spine 动画枚举
    /// </summary>
    public enum SpineAnimations
    {{
{animsEnum}    }}
}}
";

            // 写入文件
            File.WriteAllText(OUTPUT_PATH, code, Encoding.UTF8);
            AssetDatabase.Refresh();

            Debug.Log($"[SpineEnumGenerator] 已更新 SpineEnums.cs\n" +
                      $"插槽: {skeletonData.Slots.Count} 个\n" +
                      $"动画: {skeletonData.Animations.Count} 个");

            EditorUtility.DisplayDialog("成功",
                $"已更新 SpineEnums.cs\n\n" +
                $"插槽: {skeletonData.Slots.Count} 个\n" +
                $"动画: {skeletonData.Animations.Count} 个",
                "确定");

            // 自动选中生成的文件
            Object generatedFile = AssetDatabase.LoadAssetAtPath<Object>(OUTPUT_PATH);
            if (generatedFile != null)
            {
                Selection.activeObject = generatedFile;
                EditorGUIUtility.PingObject(generatedFile);
            }
        }

        [MenuItem("Assets/Spine/生成枚举到 SpineEnums.cs", true)]
        private static bool ValidateGenerateEnums()
        {
            return Selection.activeObject != null && Selection.activeObject is SkeletonDataAsset;
        }

        /// <summary>
        /// 清理名称为合法的 C# 标识符
        /// </summary>
        private static string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unknown";

            StringBuilder sb = new StringBuilder();
            bool capitalizeNext = false;

            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(capitalizeNext ? char.ToUpper(c) : c);
                    capitalizeNext = false;
                }
                else if (c == '_' || c == '-' || c == ' ')
                {
                    capitalizeNext = true;
                }
            }

            string result = sb.ToString();

            // 处理以数字开头的情况
            if (result.Length > 0 && char.IsDigit(result[0]))
                result = "_" + result;

            return string.IsNullOrEmpty(result) ? "Unknown" : result;
        }
    }
}
