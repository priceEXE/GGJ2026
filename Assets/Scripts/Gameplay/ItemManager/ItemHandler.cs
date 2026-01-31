namespace Gameplay
{
    public class ItemHandler
    {
        /// <summary>
        /// 根据物品修改玩家属性（增量更新）
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void IncreasePlayerInfo(IActor actor, Item item)
        {
            if(actor is Player player)
            {
                player.m_playerInfo += item.m_itemPlayerInfo;
            }
        }
        /// <summary>
        /// 根据物品还原玩家属性（增量更新）
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void RevertPlayerInfo(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_playerInfo -= item.m_itemPlayerInfo;
            }
        }
        /// <summary>
        /// 设置玩家属性（部分覆盖，覆盖的部分通过重载的&运算符定义）
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void SetPlayerInfo(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_playerInfo &= item.m_itemPlayerInfo;
            }
        }
        /// <summary>
        /// 设置武器属性（全覆盖）
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item"></param>
        public static void SetWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                //先移除其他武器并触发移除回调恢复到默认武器
                for(int i = 0; i < player.m_itemContainer.m_ItemContainerCount; i++)
                {
                    var it = player.m_itemContainer[i];
                    if(it.m_enableAmmoCount && it.m_itemName != item.m_itemName)
                    {
                        player.m_itemContainer.RemoveItem(actor, it);
                    }
                }
                player.m_weaponColder.m_weaponInfo = item.m_itemWeaponInfo;
                player.m_weaponColder.ammoCounter = item.m_ammoCount;
            }
        }
        /// <summary>
        /// 删除武器属性
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="item"></param>
        public static void DeleteWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_weaponColder.m_weaponInfo = ItemConfig.DefaultItem.m_itemWeaponInfo;
            }
        }
        /// <summary>
        /// 提升武器属性（增量更新）
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void IncreaseWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_weaponColder.m_weaponInfo += item.m_itemWeaponInfo;
            }
        }
        /// <summary>
        /// 还原武器属性（增量更新）
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void RevertWeaponInfo(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_weaponColder.m_weaponInfo -= item.m_itemWeaponInfo;
            }
        }
        /// <summary>
        /// 恢复默认武器
        /// </summary>
        /// <param name="actor">Actor接口</param>
        /// <param name="item">源物品</param>
        public static void RecoverDefaultWeapon(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_weaponColder.m_weaponInfo = ItemConfig.DefaultItem.m_itemWeaponInfo;
                player.m_weaponColder.ammoCounter = 0;
            }
        }
        public static void ReplaceSpecialMaterial(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                ItemContainer container = player.m_itemContainer;
                for (int i = 0; i < container.m_ItemContainerCount; i++)
                {
                    var it = container[i];
                    if (it.m_isSpecialItem)
                    {
                        container.RemoveItem(actor, it);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 为Actor添加标记
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="item"></param>
        public static void AddTagToActor(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_itemContainer.AddTag(item.m_tag);
            }
        }
        public static void RemoveTagFromActor(IActor actor, Item item)
        {
            if (actor is Player player)
            {
                player.m_itemContainer.RemoveTag(item.m_tag);
            }
        }
    }
    
}
