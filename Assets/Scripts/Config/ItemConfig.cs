using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class ItemConfig
{
    public static readonly Item normalCake = new Item
    {
        m_itemName = "DefaultCake",
        m_itemIconPath = null,
        m_itemIgnoreDuration = true,
        m_itemDuration = 10f,
        m_onItemEnter = (actor, item) =>
        {
            ItemHandler.IncreasePlayerInfo(actor, item);
            ItemHandler.IncreaseWeaponInfo(actor, item);
        },
        m_onItemQuit = (actor, item) =>
        {
            ItemHandler.RevertPlayerInfo(actor, item);
            ItemHandler.RevertWeaponInfo(actor, item);
        },
    };
}
