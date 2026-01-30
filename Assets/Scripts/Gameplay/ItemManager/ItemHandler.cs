using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public class ItemHandler
    {
        /// <summary>
        /// 根据物品修改玩家属性
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void IncreasePlayerInfo(IActor actor, Item item)
        {
            if(actor is Player)
            {
                (actor as Player).m_playerInfo += item.m_itemPlayerInfo;
            }
        }
        /// <summary>
        /// 根据物品还原玩家属性
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void RevertPlayerInfo(IActor actor, Item item)
        {
            if (actor is Player)
            {
                (actor as Player).m_playerInfo -= item.m_itemPlayerInfo;
            }
        }
        /// <summary>
        /// 更改武器属性
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item"></param>
        public static void ChangeWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player)
            {
                (actor as Player).m_weaponColder.m_weaponInfo = item.m_itemWeaponInfo;
            }
        }
        /// <summary>
        /// 提升武器属性
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void IncreaseWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player)
            {
                (actor as Player).m_weaponColder.m_weaponInfo += item.m_itemWeaponInfo;
            }
        }
        /// <summary>
        /// 还原武器属性
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void RevertWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player)
            {
                (actor as Player).m_weaponColder.m_weaponInfo -= item.m_itemWeaponInfo;
            }
        }
    }
    
}
