using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public struct BulletInfo
    {
        //子弹速度
        public float m_speed;
        //子弹受到的重力尺度
        public float m_gravityScale;
        public static BulletInfo operator +(BulletInfo dest, BulletInfo scr)
        {
            BulletInfo result = new BulletInfo();
            result.m_speed = dest.m_speed + scr.m_speed;
            result.m_gravityScale = dest.m_gravityScale + scr.m_gravityScale;
            return result;
        }
        public static BulletInfo operator -(BulletInfo dest, BulletInfo scr)
        {
            BulletInfo result = new BulletInfo();
            result.m_speed = dest.m_speed - scr.m_speed;
            result.m_gravityScale = dest.m_gravityScale - scr.m_gravityScale;
            return result;
        }
    }
    
}
