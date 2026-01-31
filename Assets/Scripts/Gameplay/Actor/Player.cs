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
}
