using UnityEngine;

namespace AlienProject
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Alien Project/Resources/Health Point Component")]
    public class CHealthPoint : ResourceBase
    {
        public void ApplyDamage(float damage)
        {
            _currentValue -= damage;
        }
    } // class CHealthPoint
} // namespace AlienProject