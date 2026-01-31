using UnityEngine;

namespace GGJ2026
{
    public class PlayerStateInfo : MonoBehaviour
    {
        [SerializeField, Tooltip("落地射线检测的长度")] public float groundCheckDistance;
        [SerializeField, Tooltip("墙壁射线检测的长度")] public float wallCheckDistance;
        [SerializeField, Tooltip("移动速度")] public float moveSpeed;
        [SerializeField, Tooltip("跳跃高度")] public float jumpForce;
        [SerializeField, Tooltip("冲刺速度")] public float dashSpeed;
        [SerializeField, Tooltip("能否贴墙")]public bool canSlide;
        [SerializeField, Tooltip("贴墙下降速度 = this * yVelocity")] public float slideSpeedMulti;
        [SerializeField, Tooltip("贴墙持续时间")] public float slideDuration;
        [SerializeField, Tooltip("冲刺持续时间")] public float dashDuration;
        [SerializeField, Tooltip("冲刺冷却时间")] public float dashColdDown;
        [SerializeField, Tooltip("蹬墙跳方向")]public Vector2 wallJumpDirection;
        [SerializeField, Tooltip("能否二段跳")]public bool canJumpInAir;
    }
}