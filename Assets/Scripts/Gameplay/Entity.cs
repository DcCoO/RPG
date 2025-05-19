using UnityEngine;

namespace BitBox.Gameplay
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] protected EEntity _type;
        public EEntity Type => _type;

        public bool Contains(EEntity entity)
        {
            return (_type & entity) == entity;
        }
    }
}
