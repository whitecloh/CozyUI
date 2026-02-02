using System.Linq;
using Game.Scripts.Data;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Game.Scripts.Editor_Utils
{
    [CustomPropertyDrawer(typeof(LocalizationKeyPopupAttribute))]
    public class LocalizationKeyPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
                return;
            }

            LocalizationData catalog = LocalizationDataFinder.FindCatalog();
            if (catalog == null)
            {
                EditorGUI.PropertyField(position, property, label);

                var helpPos = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 2f,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );

                EditorGUI.HelpBox(helpPos,
                    "LocalizationCatalog не найден. Создайте его: Create > UI > Localization > Localization Catalog.",
                    MessageType.Info);

                EditorGUI.EndProperty();
                return;
            }

            var keys = catalog.GetKeys();
            if (keys == null || keys.Count == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
                return;
            }

            string current = property.stringValue;
            int index = Mathf.Max(0, keys.ToList().FindIndex(k => k == current));

            string[] options = keys.ToArray();
            int newIndex = EditorGUI.Popup(position, label.text, index, options);

            if (newIndex >= 0 && newIndex < options.Length)
                property.stringValue = options[newIndex];

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool hasCatalog = LocalizationDataFinder.FindCatalog() != null;
            return hasCatalog
                ? EditorGUIUtility.singleLineHeight
                : EditorGUIUtility.singleLineHeight * 2f + 2f;
        }
    }
}
#endif