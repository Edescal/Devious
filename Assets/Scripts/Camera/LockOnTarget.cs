using UnityEngine;

namespace Edescal
{
    public class LockOnTarget : MonoBehaviour
    {
        [field: SerializeField]
        public TargetType Type { get; private set; } = TargetType.Object;
    }

    public enum TargetType
    {
        Object,
        Enemy
    }
}