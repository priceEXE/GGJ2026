using UnityEngine;

namespace GGJ2026
{
    /// <summary>
    /// 全局玩家配置类 - 集中管理所有玩家移动参数
    /// </summary>
    public static class PlayerConfig
    {
        // ========== 基础移动 ==========
        /// <summary>地面移动速度</summary>
        public static float MoveSpeed = 8f;
        
        /// <summary>空中速度改变空控制倍率（相对于地面速度）</summary>
        public static float AirSpeedMultiplier = 0.5f;
        
        /// <summary>最大运动速率（速度上限）</summary>
        public static float MaxSpeed = 20f;

        // ========== 物理参数 ==========
        /// <summary>重力加速度（gravityScale）</summary>
        public static float GravityScale = 3f;

        // ========== 跳跃 ==========
        /// <summary>第一段跳跃力度</summary>
        public static float JumpForce = 10f;
        
        /// <summary>第二段跳跃力度</summary>
        public static float DoubleJumpForce = 8f;

        // ========== 冲刺 ==========
        /// <summary>冲刺水平分量</summary>
        public static float DashHorizontal = 15f;
        
        /// <summary>冲刺竖直分量</summary>
        public static float DashVertical = 5f;
        
        /// <summary>地面冲刺竖直加成（在地面冲刺时额外增加的竖直速度）</summary>
        public static float DashGroundVerticalBonus = 5f;
        
        /// <summary>冲刺最大持续时间（秒）</summary>
        public static float DashMaxDuration = 1f;

        // ========== 墙跳 ==========
        /// <summary>蹬墙跳水平分量</summary>
        public static float WallJumpHorizontal = 5f;
        
        /// <summary>墙跳垂直力度</summary>
        public static float WallJumpVertical = 12f;
        
        /// <summary>墙跳后的输入锁定时间（防止立即吸回墙壁）</summary>
        public static float WallJumpInputLock = 0.04f;

        // ========== 碰撞检测 ==========
        /// <summary>地面检测距离</summary>
        public static float GroundCheckDistance = 0.55f;
        
        /// <summary>墙壁检测距离（用于被动物理检测：防穿墙、冲刺撞墙等）</summary>
        public static float WallCheckDistance = 0.55f;

        // ========== 墙壁滑行 ==========
        /// <summary>墙壁滑行速度倍率</summary>
        public static float SlideSpeedMultiplier = 0.5f;
        
        /// <summary>墙壁滑行最大持续时间</summary>
        public static float SlideDuration = 2f;
    }
}
