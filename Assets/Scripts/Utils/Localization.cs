using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edescal
{
    public static class Localization
    {
        public static Languages CurrentLanguage { get; private set; } = Languages.ESP;
        public static event Action onLanguageChanged;

        private static Dictionary<string, string> translations = new Dictionary<string, string>();

        private const string CSV_PATH = "translations";
        private const char SEPARATOR = ';';

        public static void Init() => SetLanguage(CurrentLanguage);

        public static void SetLanguage(Languages lang)
        {
            TextAsset csv = Resources.Load(CSV_PATH) as TextAsset;
            if (csv != null)
            {
                string[] row = csv.text.Split(new string[] {"\r\n", "\n"}, System.StringSplitOptions.RemoveEmptyEntries);
                //Mayor a 1 porque la fila 0 es para los nombres de las columnas
                if (row.Length > 1)
                {
                    //Checar si el índice del Language existe (no se pase del array)
                    var index = (int)lang;
                    var indexExist = row[0].Split(SEPARATOR).Length > (int)lang;
                    if (indexExist)
                    {
                        translations.Clear();
                        for (int i = 1; i < row.Length; i++)
                        {
                            //Si la fila no está vacía...
                            if (row[i].Trim().Length > 0)
                            {
                                var columns = row[i].Split(SEPARATOR);
                                //Si la key no es un string vacío añadirlo
                                if (!string.IsNullOrWhiteSpace(columns[0]))
                                {
                                    translations.Add(columns[0].ToLower(), columns[index]);
                                }
                                else
                                {
                                    Debug.LogWarning($"Error at loading translations at index {index}, row {i}");
                                    continue;
                                }
                            }
                        }

                        CurrentLanguage = lang;
                        onLanguageChanged?.Invoke();
                        Debug.Log($"Translations from language {CurrentLanguage} loaded correctly:");
                    }
                    else Debug.LogWarning($"Error at language index {index}, column {index} may not exist");
                }
                else Debug.LogWarning($"File {CSV_PATH} has no rows...");
            }
            else Debug.LogWarning($"File {CSV_PATH} not found in resources...");
        }

        public static string GetString(string key)
        {
            if (!translations.TryGetValue(key.ToLower(), out var value))
            {
                Debug.LogError($"Localized string with key {key} not found");
                return "THIS IS AN ERROR";
            }
            return value;
        }
    }
}