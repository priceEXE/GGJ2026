using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public delegate void OnItemEnter(IActor actor, Item item);
    public delegate void OnItemAbsorb(IActor actor, Item item);
    public delegate void OnItemQuit(IActor actor, Item item);
    public struct Item
    {
        //物品名称
        public string m_itemName;
        //物品图标路径
        public string m_itemIconPath;
        //物品忽略持续时间（物品为武器时启用）
        public bool m_itemIgnoreDuration;
        //物品最长持续时间
        public float m_itemDuration;
        //物品当前持续时间
        public float m_itemCurrentDuration;
        //物品获得回调
        public OnItemEnter m_onItemEnter;
        //物品持有回调
        public OnItemAbsorb m_onItemAbsorb;
        //物品丢弃回调
        public OnItemQuit m_onItemQuit;
        //物品提供的玩家属性修改
        public PlayerInfo m_itemPlayerInfo;
        //物品提供的武器属性修改
        public WeaponInfo m_itemWeaponInfo;
    }
    
}
