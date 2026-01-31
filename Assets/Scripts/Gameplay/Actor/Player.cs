using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
public class Player : MonoBehaviour, IActor
{
    public PlayerInfo m_playerInfo;
    public WeaponColder m_weaponColder;
    public ItemContainer m_itemContainer;
    void Start()
    {
        
    }

    void Update()
    {
        m_weaponColder.OnUpdate(Time.deltaTime);
        m_itemContainer.OnUpdate(this, Time.deltaTime);
    }
    public PlayerInfo GetPlayerInfo()
    {
        return m_playerInfo;
    }
}
