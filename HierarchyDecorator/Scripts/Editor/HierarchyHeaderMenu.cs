using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HierarchyDecorator
{
    internal static class HierarchyHeaderMenu
    {
        private const string Root = "GameObject/Hierarchy Decorator/";

        [MenuItem(Root + "Header", false, 10)]
        private static void Header(MenuCommand command) { Create(command, "Header (Centered)", "Header"); }
        [MenuItem(Root + "Subheader", false, 11)]
        private static void Subheader(MenuCommand command) { Create(command, "Subheader", "Subheader"); }
        [MenuItem(Root + "Mini Header", false, 12)]
        private static void MiniHeader(MenuCommand command) { Create(command, "Mini Header (Centered)", "Mini Header"); }

        [MenuItem(Root + "Header", true)]
        private static bool CanHeader() { return FindStyle("Header (Centered)") != null; }
        [MenuItem(Root + "Subheader", true)]
        private static bool CanSubheader() { return FindStyle("Subheader") != null; }
        [MenuItem(Root + "Mini Header", true)]
        private static bool CanMiniHeader() { return FindStyle("Mini Header (Centered)") != null; }

        private static HierarchyStyle FindStyle(string name)
        {
            return HierarchyDecorator.Settings.styleData.styles.Find(style =>
                style != null && style.name == name && !style.isRegex && !string.IsNullOrEmpty(style.prefix));
        }

        internal static GameObject Create(MenuCommand command, string styleName, string label)
        {
            var style = FindStyle(styleName);
            if (style == null) return null;
            var context = command.context as GameObject;
            // Ignore project assets; only scene and prefab-stage objects are insertion targets.
            if (context != null && (!context.scene.IsValid() || EditorUtility.IsPersistent(context))) context = null;
            var header = new GameObject(style.prefix + (style.noSpaceAfterPrefix ? "" : " ") + label);
            if (context != null)
            {
                SceneManager.MoveGameObjectToScene(header, context.scene);
                GameObjectUtility.SetParentAndAlign(header, context.transform.parent != null ? context.transform.parent.gameObject : null);
                header.transform.SetSiblingIndex(context.transform.GetSiblingIndex());
            }
            else
            {
#if UNITY_2019_1_OR_NEWER
                UnityEditor.SceneManagement.StageUtility.PlaceGameObjectInCurrentStage(header);
#endif
            }
            Undo.RegisterCreatedObjectUndo(header, "Create Hierarchy " + label);
            Selection.activeGameObject = header;
            EditorApplication.RepaintHierarchyWindow();
            return header;
        }
    }
}
