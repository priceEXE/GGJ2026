using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
namespace Gameplay
{
    public class WeaponHandler
    {
        public static void OnWeaponFire(IActor actor, WeaponInfo weaponInfo)
        {
            if(actor is Player player)
            {
                GameObject gameObject = GameObject.Instantiate(weaponInfo.m_bulletPrefab);
                if(player.m_redirectTarget)
                {
                    gameObject.transform.SetParent(player.m_redirectTarget);
                }
                gameObject.transform.position = (actor as MonoBehaviour).transform.position;
                gameObject.GetComponent<IActor>().Initialize(actor);
                gameObject.GetComponent<IActor>().SetMoveCommand(player.m_fireDirection);
            }
        }
    }
}


