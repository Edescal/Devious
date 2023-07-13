using System.Collections.Generic;
using UnityEngine;

namespace Edescal.DialogueSystem
{
    [CreateAssetMenu(fileName = "New dialogue", menuName = "Dialogue system/Create new dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        private string[] dialogueIds = new string[] { };

        [field: SerializeField]
        public ResponseType responseType { get; private set; }

        public string[] GetDialogues()
        {
            var localizedDialogues = new string[dialogueIds.Length];
            for (int i = 0; i < dialogueIds.Length; i++)
            {
                localizedDialogues[i] = Localization.GetString(dialogueIds[i]);
            }
            return localizedDialogues;
        }
    }

    public enum ResponseType
    {
        NO_OPTIONS = 0,
        OK = 1,
        OK_CANCEL = 2,
        YES_NO_CANCEL = 3
    }
}