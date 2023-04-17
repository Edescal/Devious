using System.Collections;
using UnityEngine;

namespace Edescal
{
    public class KeyHolder : MonoBehaviour
    {
        [field: SerializeField] 
        public int Keys { get; private set; }
        public bool HasKey => Keys > 0;

        public void AddKey()
        {
            Keys++;
        }

        public void UseKey()
        {
            if (!HasKey) return;
            Keys--;
        }
    }
}