// 此文件由 SpineEnumGenerator 自动生成
// 请勿手动修改
// 来源: PLAYER_SkeletonData

using System.Collections.Generic;

namespace Gameplay.AnimeAndCostume
{
    /// <summary>
    /// Spine 插槽枚举
    /// </summary>
    public enum SpineSlots
    {
        None = 0,
        sound = 1,
        lefthand = 2,
        leftleg = 3,
        body = 4,
        rightleg = 5,
        face2 = 6,
        face1 = 7,
        righthand = 8,
        cake = 9,
        face3 = 10,
        cakeFace = 11,
        cakeFaceIcecream = 12,
        cakeFaceSoda = 13,
        cakeFacecherry = 14,
        cakeIcecream = 15,
        cakeCherry = 16,
        cakeSoda = 17,
        Cannon = 18,
        Gun = 19,
    }

    /// <summary>
    /// Spine 动画枚举
    /// </summary>
    public enum SpineAnimations
    {
        None = 0,
        attack = 1,
        dash = 2,
        diaup = 3,
        idle = 4,
        jump = 5,
        run = 6,
        struck = 7,
    }

    /// <summary>
    /// 枚举到Spine原始名称的映射工具
    /// </summary>
    public static class SpineNames
    {
        /// <summary>
        /// 插槽枚举 -> Spine原始名称
        /// </summary>
        public static readonly Dictionary<SpineSlots, string> SlotNames = new Dictionary<SpineSlots, string>
        {
            { SpineSlots.sound, "sound" },
            { SpineSlots.lefthand, "lefthand" },
            { SpineSlots.leftleg, "leftleg" },
            { SpineSlots.body, "body" },
            { SpineSlots.rightleg, "rightleg" },
            { SpineSlots.face2, "face2" },
            { SpineSlots.face1, "face1" },
            { SpineSlots.righthand, "righthand" },
            { SpineSlots.cake, "cake" },
            { SpineSlots.face3, "face3" },
            { SpineSlots.cakeFace, "cake-face" },
            { SpineSlots.cakeFaceIcecream, "cake-face-icecream" },
            { SpineSlots.cakeFaceSoda, "cake-face-soda" },
            { SpineSlots.cakeFacecherry, "cake-facecherry" },
            { SpineSlots.cakeIcecream, "cake-icecream" },
            { SpineSlots.cakeCherry, "cake-cherry" },
            { SpineSlots.cakeSoda, "cake-soda" },
            // 非Spine槽位（使用SpriteRenderer，这里的映射仅用于完整性）
            { SpineSlots.Cannon, "Cannon" },
            { SpineSlots.Gun, "Gun" },
        };

        /// <summary>
        /// 动画枚举 -> Spine原始名称
        /// </summary>
        public static readonly Dictionary<SpineAnimations, string> AnimationNames = new Dictionary<SpineAnimations, string>
        {
            { SpineAnimations.attack, "attack" },
            { SpineAnimations.dash, "dash" },
            { SpineAnimations.diaup, "diaup" },
            { SpineAnimations.idle, "idle" },
            { SpineAnimations.jump, "jump" },
            { SpineAnimations.run, "run" },
            { SpineAnimations.struck, "struck" },
        };

        /// <summary>
        /// 获取插槽的Spine原始名称
        /// </summary>
        public static string GetSlotName(SpineSlots slot)
        {
            return SlotNames.TryGetValue(slot, out string name) ? name : slot.ToString();
        }

        /// <summary>
        /// 获取动画的Spine原始名称
        /// </summary>
        public static string GetAnimationName(SpineAnimations anim)
        {
            return AnimationNames.TryGetValue(anim, out string name) ? name : anim.ToString();
        }
    }
}
