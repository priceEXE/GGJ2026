using UnityEngine;

namespace GGJ2026
{
    /// <summary>
    /// 冲刺状态 - 斜向冲刺，期间无法控制
    /// </summary>
    public class PlayerDashState : PlayerState
    {
        private float oldGravityScale;

        public PlayerDashState(PlayerInstance owner, string animaName)
        {
            this.owner = owner;
            this.animaName = animaName;
        }

        public override void Enter()
        {
            base.Enter();
            
            // 记录冲刺开始时间
            owner.dashStartTime = Time.time;
            owner.lastDashTime = Time.time;
            
            // 冲刺方向：优先使用输入，无输入时使用朝向
            float horizontal;
            if (Mathf.Abs(owner.moveValue.x) > 0.1f)
            {
                // 有输入：使用输入方向
                horizontal = Mathf.Sign(owner.moveValue.x);
            }
            else
            {
                // 无输入：使用面朝方向
                horizontal = owner.FacingDirection;
            }
            
            // 保留重力，让冲刺形成弧线
            // 不修改 gravityScale
            
            // 清空所有速度
            owner.SetVelocity(0, 0);
            
            // 计算竖直分量（地面冲刺有额外加成）
            float verticalVelocity = PlayerConfig.DashVertical;
            if (owner.GroundDetected)
            {
                verticalVelocity += PlayerConfig.DashGroundVerticalBonus;
                Debug.Log($"[{owner.name}] Ground dash bonus applied!");
            }
            
            // 设置冲刺初始速度（不归一化，直接使用配置值）
            Vector2 dashVelocity = new Vector2(
                horizontal * PlayerConfig.DashHorizontal,  // 水平分量
                verticalVelocity                            // 竖直分量（地面有加成）
            );
            owner.SetVelocity(dashVelocity.x, dashVelocity.y);
            
            Debug.Log($"[{owner.name}] Dash started: direction={horizontal}, velocity={dashVelocity}");
        }

        public override void Update()
        {
            base.Update();
            
            // 冲刺期间保持速度不变（无法控制）
            // 不调用 owner.SetVelocity，让初始速度保持
            
            // 检查退出条件
            float dashDuration = Time.time - owner.dashStartTime;
            
            // 1. 撞墙 -> 进入 Fall
            if (owner.WallDetected)
            {
                Debug.Log($"[{owner.name}] Dash ended: hit wall");
                owner.stateMachine.ChangeState(PlayerStates.Fall);
                return;
            }
            
            // 2. 落地 -> 进入 Idle（但需要至少冲刺 0.1 秒才能检测落地）
            if (owner.GroundDetected && dashDuration > 0.1f)
            {
                Debug.Log($"[{owner.name}] Dash ended: landed");
                owner.stateMachine.ChangeState(PlayerStates.Idle);
                return;
            }
            
            // 3. 超时 -> 进入 Fall 并恢复二段跳
            if (dashDuration >= PlayerConfig.DashMaxDuration)
            {
                Debug.Log($"[{owner.name}] Dash ended: timeout (restoring double jump)");
                owner.hasDoubleJump = true; // 恢复二段跳权限
                owner.stateMachine.ChangeState(PlayerStates.Fall);
                return;
            }
        }

        public override void Exit()
        {
            base.Exit();
            // 不需要恢复重力，因为从未禁用
        }
    }
}