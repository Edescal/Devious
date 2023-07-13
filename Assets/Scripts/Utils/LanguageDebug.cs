using UnityEngine;
using TMPro;

namespace Edescal
{
    public class LanguageDebug : MonoBehaviour
    {
        public Languages language;
        public string key;
        public TMP_Text textMesh;


        private void Start()
        {
            Localization.Init();
            GetKey();
        }

        [ContextMenu("Load")]
        public void Translate()
        {
            Localization.SetLanguage(language);
            GetKey();
        }

        [ContextMenu("Get key")]
        public void GetKey()
        {
            if (textMesh == null) return;
            textMesh.text = Localization.GetString(key);
        }
    }
}