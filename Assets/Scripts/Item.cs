using UnityEngine;

namespace Edescal
{
    [CreateAssetMenu(menuName ="Items/New item", fileName = "New Item")]
    public class Item : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
    }
}