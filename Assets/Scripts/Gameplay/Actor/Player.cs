using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
public class Player : MonoBehaviour, IActor
{
    public PlayerInfo m_playerInfo;
    public WeaponColder m_weaponColder = null;
    public ItemContainer m_itemContainer = null;
    public Vector2 m_moveDirection;
    public Vector2 m_fireDirection;
    public bool m_isFiring = false;
    public bool m_enableRedirectTransform = false;
    public Transform m_redirectTarget;
    void Start()
    {
        
    }

    public void Initialize(IActor actor)
    {
        m_playerInfo = default;
        m_itemContainer = new ItemContainer();
        m_weaponColder = new WeaponColder(this);
    }

    void Update()
    {
        if(m_isFiring && m_weaponColder.ableToTrigger)
        {
            m_weaponColder?.OnFire();
        }
        m_weaponColder?.OnUpdate(Time.deltaTime);
        m_itemContainer?.OnUpdate(this, Time.deltaTime);
    }
    public PlayerInfo GetPlayerInfo()
    {
        return m_playerInfo;
    }
    public void SetMoveCommand(Vector2 direction)
    {
        m_moveDirection = direction.normalized;
    }
    void OnCollisionEnter2D(Collision2D collision2d)
    {
        //被子弹命中
        if(collision2d.gameObject.CompareTag("Bullet"))
        {
            IActor actor = collision2d.gameObject.GetComponent<IActor>();
            if(actor != null && actor is Bullet bullet)
            {
                IActor actor1 = bullet.m_owner;
                if(actor1 != null  && actor1 is Player player)
                {
                    if(player == this)
                    {
                        return;
                    }
                    PlayerInfo attackerInfo = player.GetPlayerInfo();
                    WeaponInfo weaponInfo = player.m_weaponColder.m_weaponInfo;
                    Debug.Log("在这里操作受击玩家属性变化");
                    //例如最简单的增加受击玩家的受击百分比
                    m_playerInfo.m_HitPrecentage += weaponInfo.m_HitPrecentageIncrease;
                    //为玩家添加生效物品(触发一个buff)
                    if(bullet.m_AddItemName != string.Empty)
                    {
                        m_itemContainer.AddItem(this, ItemConfig.m_ItemLists[bullet.m_AddItemName]);
                    }
                    //为玩家移除生效物品（强制移除一个buff）
                    if(bullet.m_RemoveItemName != string.Empty)
                    {
                        m_itemContainer.RemoveItem(this, ItemConfig.m_ItemLists[bullet.m_RemoveItemName]);
                    }
                }
            }
        }
        //捡起物品
        if(collision2d.gameObject.CompareTag("Item"))
        {
            string itemName = collision2d.gameObject.name;
            m_itemContainer.AddItem(this, ItemConfig.m_ItemLists[itemName]);
        }
    }
}
