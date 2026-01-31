using UnityEngine;
namespace Gameplay
{
    public delegate void OnWeaponFire(IActor actor,WeaponInfo weaponInfo);
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
        //武器射击回调
        public OnWeaponFire m_onWeaponFire;
        //物品使用的子弹
        public BulletInfo m_itemBulletInfo;

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
        public IActor m_owner;
        //武器信息
        public WeaponInfo m_weaponInfo;
        //启用忽略弹药计数 
        public bool ignoreAmmoCounter;
        //弹药计数
        public int ammoCounter;
        //是否可以触发射击
        public bool ableToTrigger => m_weaponInfo.m_currentColdDuration >= m_weaponInfo.m_coldDuration && ammoCounter > 0;
        /// <summary>
        /// 构造函数注入依赖
        /// </summary>
        /// <param name="actor"></param>
        public WeaponColder(IActor actor)
        {
            m_owner = actor;
        }
        
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
        /// <summary>
        /// 射击事件函数
        /// </summary>
        public void OnFire()
        {
            m_weaponInfo.m_currentColdDuration = 0f;
            if(ignoreAmmoCounter)
            {
                return;
            }
            ammoCounter--;
            m_weaponInfo.m_onWeaponFire?.Invoke(m_owner, m_weaponInfo);
        }
    }
    
}
