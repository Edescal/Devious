using UnityEditor;
using UnityEditor.UI;

namespace Edescal.DialogueSystem
{
    [CustomEditor(typeof(SelectableButton))]
    public class SelectableButtonEditor : ButtonEditor
    {
        private bool defaultFoldout = true;
        private bool selectableFoldout = true;

        private SerializedProperty imageCanvasProperty;
        private SerializedProperty glowCanvasProperty;
        private SerializedProperty fadeTimeProperty;
        private SerializedProperty glowTimeProperty;
        private SerializedProperty tweenTypeProperty;
        private SerializedProperty selectedSound, pressedSound;
        private SerializedProperty buttonLabelProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            buttonLabelProperty = serializedObject.FindProperty("buttonLabel");
            imageCanvasProperty = serializedObject.FindProperty("imageCanvas");
            glowCanvasProperty = serializedObject.FindProperty("glowCanvas");
            fadeTimeProperty = serializedObject.FindProperty("fadeTime");
            glowTimeProperty = serializedObject.FindProperty("glowTime");
            tweenTypeProperty = serializedObject.FindProperty("tweenType");
            selectedSound = serializedObject.FindProperty("selectedSound");
            pressedSound = serializedObject.FindProperty("pressedSound");
        }

        public override void OnInspectorGUI()
        {
            selectableFoldout = EditorGUILayout.Foldout(selectableFoldout, "On selection settings");
            if (selectableFoldout)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(buttonLabelProperty);
                EditorGUILayout.PropertyField(imageCanvasProperty);
                EditorGUILayout.PropertyField(glowCanvasProperty);
                EditorGUILayout.PropertyField(fadeTimeProperty);
                EditorGUILayout.PropertyField(glowTimeProperty);
                EditorGUILayout.PropertyField(tweenTypeProperty);
                EditorGUILayout.PropertyField(selectedSound);
                EditorGUILayout.PropertyField(pressedSound);
                EditorGUILayout.Space();
                serializedObject.ApplyModifiedProperties();
            }

            defaultFoldout = EditorGUILayout.Foldout(defaultFoldout, "Button settings");
            if (defaultFoldout)
            {
                base.OnInspectorGUI();
            }
        }
    }

    [CustomEditor(typeof(ItemFrame))]
    public class ItemFrameEditor : SelectableButtonEditor
    {
        private bool itemFrameFoldout = true;
        private SerializedProperty itemImageProperty;
        private SerializedProperty inventoryUIProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            inventoryUIProperty = serializedObject.FindProperty("inventoryUI");
            itemImageProperty = serializedObject.FindProperty("itemImage");
        }

        public override void OnInspectorGUI()
        {
            itemFrameFoldout = EditorGUILayout.Foldout(itemFrameFoldout, "Item frame");
            if (itemFrameFoldout)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(inventoryUIProperty);
                EditorGUILayout.PropertyField(itemImageProperty);
                EditorGUILayout.Space();
                serializedObject.ApplyModifiedProperties();
            }
            base.OnInspectorGUI();
        }
    }
}
