using Gameplay;
using UnityEngine;

public class ItemConfig
{
    //空物体
    public static readonly Item DefaultItem = new Item
    {
        m_itemName = "DefaultItem",
        m_itemIconPath = null,
        m_itemIgnoreDuration = false,
        m_itemDuration = 0f,
        m_itemCurrentDuration = 0f,
        m_onItemEnter = null,
        m_onItemAbsorb = null,
        m_onItemQuit = null,
        m_itemPlayerInfo = new PlayerInfo(),
        m_itemWeaponInfo = new WeaponInfo(),
        m_enableAmmoCount = false,
        m_ammoCount = 0,
        m_gravityScale = 1f,
        m_jumpForceScale = 1f,
        m_hitBackForeceScale = 1f,
        m_isSpecialItem = false,
        m_tag = string.Empty,
    };
    //默认蛋糕物品
    public static readonly Item NormalCake = new Item
    {
        m_itemName = "DefaultCake",//物品名称
        m_itemIconPath = null,//物品图标路径
        m_itemIgnoreDuration = true,//物品忽略持续时间（物品为武器时启用）
        m_onItemEnter = (actor, item) =>//物品获得回调
        {
            ItemHandler.SetWeaponInfo(actor, item);
            ItemHandler.SetPlayerInfo(actor, item);
        },
        m_enableAmmoCount = false,//是否启用弹药数量计算方式
        m_isSpecialItem = false,//物品是否为特殊物品
    };

    public static readonly Item Icecream = new Item
    {
        m_itemName = "IceCream",
        m_itemIconPath = null,
        m_itemIgnoreDuration = false,//未启用忽略持续时间，则需要配置最大持续时间
        m_itemDuration = 15f,
        m_onItemEnter = (actor, item) =>
        {
            ItemHandler.IncreasePlayerInfo(actor, item);
        },
        m_onItemQuit = (actor, item) =>
        {
            ItemHandler.RevertPlayerInfo(actor, item);
        },
        m_itemPlayerInfo = new PlayerInfo()
        {
            m_moveSpeed = 2f,//移动速度
            m_jumpForce = 0,//跳跃力
            m_HitPrecentage = 0,
            m_groundStickiness = 0.5f,//地面粘性
        },
        //非武器物品无需配置武器属性修改
        m_enableAmmoCount = false,
        m_isSpecialItem = false,
    };

    public static readonly Item CreamRifle = new Item
    {
        m_itemName = "CreamRifle",
        m_itemIconPath = null,
        m_itemIgnoreDuration = true,
        m_onItemEnter = (actor, item) =>
        {
            ItemHandler.IncreasePlayerInfo(actor, item);
            ItemHandler.SetWeaponInfo(actor, item);
            
        },
        m_onItemQuit = (actor, item) =>
        {
            ItemHandler.RevertPlayerInfo(actor, item);
            ItemHandler.RecoverDefaultWeapon(actor,item);
        },
        m_itemWeaponInfo = new WeaponInfo()
        {
            m_weaponName = "CreamRifle",
            m_coldDuration = 0.5f,
            m_HitPrecentageIncrease = 0.2f,
            m_bulletPrefab = null,//需配置子弹预制体,当前为空
        },
        m_enableAmmoCount = true,//武器类物品启用弹药数量计算方式
        m_ammoCount = 30,//初始弹药数量
        m_isSpecialItem = false,
    };

    public static readonly Item SpecialMaterial = new Item
    {
        m_itemName = "SpecialMaterial",
        m_itemIconPath = null,
        m_itemIgnoreDuration = false,
        m_itemDuration = 15f,
        m_onItemEnter = (actor, item) =>
        {
            //赋予能力类的在物品获得时添加Tag到玩家
            ItemHandler.ReplaceSpecialMaterial(actor, item);
            ItemHandler.AddTagToActor(actor, item);
        },
        m_onItemQuit = (actor, item) =>
        {
            //移除能力类的在物品丢弃时从玩家移除Tag
            ItemHandler.RemoveTagFromActor(actor, item);
        },
        m_enableAmmoCount = false,//武器类物品启用弹药数量计算方式
        m_isSpecialItem = true,
        m_tag = "CanDoubleJump",//赋予玩家二段跳能力的标记
    };
}
