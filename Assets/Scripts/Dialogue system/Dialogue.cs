using System.Collections.Generic;
using UnityEngine;

namespace Edescal.DialogueSystem
{
    [CreateAssetMenu(fileName = "New dialogue", menuName = "Dialogue system/Create new dialogue")]
    public class Dialogue : ScriptableObject
    {
        [field: SerializeField, TextArea]
        public string[] dialogues { get; private set; }
        [field: SerializeField]
        public ResponseType responseType { get; private set; }

        //DEPRECATED
        public int[] dialogueIds { get; private set; }
        public string[] GetDialogues(int language = 1)
        {
            var csv = Resources.Load<TextAsset>("Dialogues");
            var split = csv.text.Split("\n");
            var dialogues = new List<string>();

            for(int i = 1; i < split.Length; i++)
            {
                var col = split[i].Split(";");
                if (int.TryParse(col[0], out int result))
                {
                    foreach(int id in dialogueIds)
                    {
                        if (result == id)
                        {
                            dialogues.Add(col[language]);
                        }
                    }
                }
            }
            return dialogues.ToArray();
        }
    }

    public enum Language
    {
        ESP = 1,
        ENG = 2
    }

    public enum ResponseType
    {
        NO_OPTIONS = 0,
        OK = 1,
        OK_CANCEL = 2,
        YES_NO_CANCEL = 3
    }
}