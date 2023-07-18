using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Edescal
{
    public class LocalizeTMP : MonoBehaviour
    {
        [SerializeField]
        TMP_Text tMesh;
        [SerializeField]
        string labelKey;

        void OnEnable()
        {
            Localization.onLanguageChanged += UpdateText;
        }

        void OnDisable()
        {
            Localization.onLanguageChanged -= UpdateText;
        }

        void Start()
        {
            UpdateText();
        }

        void UpdateText()
        {
            if (tMesh == null) return;
            tMesh.text = Localization.GetString(labelKey);
        }
    }
}