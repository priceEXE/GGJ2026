using System.Collections;
using System.Collections.Generic;
namespace Gameplay
{
    public class ItemContainer
    {
        private List<Item> m_itemList;
        private HashSet<string> m_tags;
        public int m_ItemContainerCount => m_itemList.Count;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ItemContainer()
        {
            m_itemList = new List<Item>();
            m_tags = new HashSet<string>();
        }
        /// <summary>
        /// 添加一个Item，并触发物品获得回调
        /// 初始化添加物品的当前持续时间为0
        /// </summary>
        /// <param name="actor">执行添加动作的Actor</param>
        /// <param name="item">被添加的物品</param>
        public void AddItem(IActor actor, Item item)
        {
            item.m_itemCurrentDuration = 0f;
            int idx = m_itemList.FindIndex(i => i.m_itemName == item.m_itemName);
            if(idx >= 0 && !item.m_itemIgnoreDuration)
            {
                var existing = m_itemList[idx];
                existing.m_itemCurrentDuration = 0f;
                m_itemList[idx] = existing;
                return;
            }
            m_itemList.Add(item);
            item.m_onItemEnter?.Invoke(actor, item);
        }
        /// <summary>
        /// 强行移除一个Item，并触发物品丢弃回调
        /// 将其从List中直接移除
        /// </summary>
        /// <param name="actor">执行移除动作的Actor</param>
        /// <param name="item">被移除的物品</param>
        public void RemoveItem(IActor actor, Item item)
        {
            if (m_itemList == null || m_itemList.Count == 0) return;
            int idx = m_itemList.FindIndex(i => i.m_itemName == item.m_itemName);
            if (idx < 0) return;
            var existing = m_itemList[idx];
            existing.m_onItemQuit?.Invoke(actor, existing);
            m_itemList.RemoveAt(idx);
        }
        /// <summary>
        /// 更新当前容器内的物品持续时间，每更新一次增加Time.deltaTime的时间
        /// 若启用物品忽略持续时间，则不更新该物品的持续时间，一直为0
        /// 当物品持续时间达到最大持续时间时，自动移除该物品并触发物品丢弃回调
        /// 若未达到最大持续时间，则触发物品持有回调
        /// 当物品启用了数量计算方式时，检查物品数量是否为0，若为0则移除该物品并触发物品丢弃回调
        /// 移除采用临时的List收集需要移除的物品，避免在遍历时修改List导致异常
        /// </summary>
        /// <param name="actor"></param>
        public void OnUpdate(IActor actor,float deltaTime)
        {
            if (m_itemList == null || m_itemList.Count == 0) return;
            List<Item> toRemove = new List<Item>();
            if(actor is Player player)
            {
                for (int i = 0; i < m_itemList.Count; i++)
                {
                    var it = m_itemList[i];
                    if (it.m_itemName == player.m_weaponColder.m_weaponInfo.m_weaponName && 
                        it.m_enableAmmoCount &&
                        player.m_weaponColder.ammoCounter <= 0)
                    {

                        toRemove.Add(it);
                    }
                }
            }
            float dt = deltaTime;
            for (int i = 0; i < m_itemList.Count; i++)
            {
                var it = m_itemList[i];
                if (it.m_itemIgnoreDuration)
                {
                    it.m_onItemAbsorb?.Invoke(actor, it);
                    continue;
                }
                it.m_itemCurrentDuration += dt;
                m_itemList[i] = it;
                if (it.m_itemCurrentDuration >= it.m_itemDuration)
                {
                    toRemove.Add(it);
                    continue;
                }
                it.m_onItemAbsorb?.Invoke(actor, it);
            }
            if (toRemove.Count > 0)
            {
                foreach (var rem in toRemove)
                {
                    int idx = m_itemList.FindIndex(i => i.m_itemName == rem.m_itemName);
                    if (idx < 0) continue;
                    var existing = m_itemList[idx];
                    existing.m_onItemQuit?.Invoke(actor, existing);
                    m_itemList.RemoveAt(idx);
                }
            }
        }
        public Item this[int index]
        {
            get
            {
                if(index < 0 || index >= m_itemList.Count)
                {
                    return default;
                }
                return m_itemList[index];
            }
        }

        public void AddTag(string tag)
        {
            m_tags.Add(tag);
        }
        public void RemoveTag(string tag)
        {
            m_tags.Remove(tag);
        }
        public bool HasTag(string tag)
        {
            return m_tags.Contains(tag);
        }
    }
    
}
