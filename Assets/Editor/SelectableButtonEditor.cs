using UnityEditor;
using UnityEditor.UI;

namespace Edescal.DialogueSystem
{
    [CustomEditor(typeof(SelectableButton))]
    public class SelectableButtonEditor : ButtonEditor
    {
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
            serializedObject.Update();
            EditorGUILayout.HelpBox("Selected button image properties", MessageType.Info);
            EditorGUILayout.PropertyField(buttonLabelProperty);
            EditorGUILayout.PropertyField(imageCanvasProperty);
            EditorGUILayout.PropertyField(glowCanvasProperty);
            EditorGUILayout.PropertyField(fadeTimeProperty);
            EditorGUILayout.PropertyField(glowTimeProperty);
            EditorGUILayout.PropertyField(tweenTypeProperty);
            EditorGUILayout.HelpBox("Sound FX", MessageType.Info);
            EditorGUILayout.PropertyField(selectedSound);
            EditorGUILayout.PropertyField(pressedSound);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Inherited Button properties", MessageType.Info);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(ItemFrame))]
    public class ItemFrameEditor : SelectableButtonEditor
    {
        private SerializedProperty itemImageProperty;
        private SerializedProperty itemNameProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            itemImageProperty = serializedObject.FindProperty("itemImage");
            itemNameProperty = serializedObject.FindProperty("itemName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox("Item frame properties", MessageType.Info);
            EditorGUILayout.PropertyField(itemImageProperty);
            EditorGUILayout.PropertyField(itemNameProperty);
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
