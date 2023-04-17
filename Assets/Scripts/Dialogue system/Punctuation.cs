using System;
using UnityEngine;

namespace Edescal.DialogueSystem
{
    [CreateAssetMenu(fileName = "New punctuation data", menuName = "Dialogue system/Create new punctuation data")]
    public class Punctuation : ScriptableObject
    {
        [Serializable]
        private struct CharSets
        {
            [field: SerializeField]
            public char Character { get; private set; }
            [field: SerializeField]
            public float TimeValue { get; private set; }
        }

        [SerializeField]
        private CharSets[] charSets;

        public bool ContainsChar(char c, out float timeValue)
        {
            for(int i = 0; i < charSets.Length; i++)
            {
                var set = charSets[i];
                if (set.Character == c)
                {
                    timeValue = set.TimeValue;
                    return true;
                }
            }
            timeValue = 0;
            return false;
        }
    }

}