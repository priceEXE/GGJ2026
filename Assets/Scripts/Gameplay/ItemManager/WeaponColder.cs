using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public struct WeaponInfo
    {
        //武器名称
        public string m_weaponName;
        //武器射击冷却时间
        public float m_coldDuration;
        //武器当前冷却时间
        public float m_currentColdDuration;
        //武器命中击退率提升百分比
        public float m_HitPrecentageIncrease;
        //武器子弹预制体
        public GameObject m_bulletPrefab;
        public static WeaponInfo operator +(WeaponInfo dest, WeaponInfo scr)
        {
            WeaponInfo result = new WeaponInfo();
            result.m_coldDuration = dest.m_coldDuration + scr.m_coldDuration;
            result.m_HitPrecentageIncrease = dest.m_HitPrecentageIncrease + scr.m_HitPrecentageIncrease;
            return result;
        }
        public static WeaponInfo operator -(WeaponInfo dest, WeaponInfo scr)
        {
            WeaponInfo result = new WeaponInfo();
            result.m_coldDuration = dest.m_coldDuration - scr.m_coldDuration;
            result.m_HitPrecentageIncrease = dest.m_HitPrecentageIncrease - scr.m_HitPrecentageIncrease;
            return result;
        }
    }
    public class WeaponColder
    {
        //武器信息
        public WeaponInfo m_weaponInfo; 
        //是否可以触发射击
        public bool ableToTrigger => m_weaponInfo.m_currentColdDuration >= m_weaponInfo.m_coldDuration;
        /// <summary>
        /// 更新事件函数
        /// </summary>
        /// <param name="deltaTime">间隔时间</param>
        public void OnUpdate(float deltaTime)
        {
            if(m_weaponInfo.m_currentColdDuration < m_weaponInfo.m_coldDuration)
            {
                m_weaponInfo.m_currentColdDuration += deltaTime;
            }
        }
    }
    
}
