using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public class Bullet : MonoBehaviour , IActor
    {
        public IActor m_owner;
        public Vector2 m_moveDirection;
        public string m_AddItemName;
        public string m_RemoveItemName;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public void Initialize(IActor actor)
        {
            m_owner = actor;
        }

        public void SetMoveCommand(Vector2 direction)
        {
            m_moveDirection = direction.normalized;
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            IActor actor = collision.gameObject.GetComponent<IActor>();
            if(actor != null && actor is Player player)
            {
                if(player == (Player)m_owner)
                {
                    return;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
