using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Gameplay
{
    public interface IActor
    {
        
    }
    /// <summary>
    /// 玩家属性结构体
    /// </summary>
    public struct PlayerInfo
    {
        //玩家名称
        public string m_actorName;
        //玩家移动速度
        public float m_moveSpeed;
        //玩家跳跃力
        public float m_jumpForce;
        //玩家受击百分比
        public int m_HitPrecentage;
        //玩家地面粘性
        public float m_groundStickiness;
        /// <summary>
        /// 重写加法修改玩家属性
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="scr"></param>
        /// <returns></returns>
        public static PlayerInfo operator + (PlayerInfo dest, PlayerInfo scr)
        {
            PlayerInfo result = new PlayerInfo();
            result.m_moveSpeed = dest.m_moveSpeed + scr.m_moveSpeed;
            result.m_jumpForce = dest.m_jumpForce + scr.m_jumpForce;
            result.m_HitPrecentage = dest.m_HitPrecentage + scr.m_HitPrecentage;
            result.m_groundStickiness = dest.m_groundStickiness + scr.m_groundStickiness;
            return result;
        }
        public static PlayerInfo operator -(PlayerInfo dest, PlayerInfo scr)
        {
            PlayerInfo result = new PlayerInfo();
            result.m_moveSpeed = dest.m_moveSpeed - scr.m_moveSpeed;
            result.m_jumpForce = dest.m_jumpForce - scr.m_jumpForce;
            result.m_HitPrecentage = dest.m_HitPrecentage - scr.m_HitPrecentage;
            result.m_groundStickiness = dest.m_groundStickiness - scr.m_groundStickiness;
            return result;
        }
    }
    
}
