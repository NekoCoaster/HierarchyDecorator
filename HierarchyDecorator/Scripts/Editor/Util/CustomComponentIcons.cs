using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    // Icons are associated by filename, matching the component's class name.
    [InitializeOnLoad]
    internal static class CustomComponentIcons
    {
        private static readonly Dictionary<string, Texture2D> Icons =
            new Dictionary<string, Texture2D>(StringComparer.Ordinal);
        private static bool loaded;

        static CustomComponentIcons()
        {
            EditorApplication.projectChanged += Refresh;
        }

        [MenuItem("Tools/HierarchyDecorator/Refresh Custom Icons")]
        internal static void Refresh()
        {
            loaded = false;
            Icons.Clear();
            ComponentIconResolver.ClearCache();
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem("Tools/HierarchyDecorator/Icon Artwork Credits")]
        private static void ShowArtworkCredits()
        {
            if (EditorUtility.DisplayDialog("Icon Artwork Credits",
                "Icons8 artwork, when installed: https://icons8.com\n\n" +
                "Reusing Icons8 icons requires an active Icons8 license. " +
                "The MIT license for HierarchyDecorator code does not cover Icons8 artwork.\n\n" +
                "Other project-supplied icons retain their respective licenses.",
                "Visit Icons8", "Close"))
                Application.OpenURL("https://icons8.com");
        }

        internal static Texture2D Find(string componentName)
        {
            if (string.IsNullOrEmpty(componentName)) return null;
            if (!loaded) Load();
            return Icons.TryGetValue(componentName, out var icon) ? icon : null;
        }

        private static void Load()
        {
            loaded = true;
            Icons.Clear();
            // Resolve relative to our own script so UPM and Assets installs both work.
            var scriptPath = AssetDatabase.GUIDToAssetPath("2594b6a58510492db42bf488fc6aafa2");
            if (!string.IsNullOrEmpty(scriptPath))
            {
                var root = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                    Path.GetDirectoryName(scriptPath))));
                LoadFolder((root + "/CustomIcons").Replace('\\', '/'));
            }
            // Project-local artwork overrides bundled filenames and survives updates.
            LoadFolder("Assets/HierarchyDecorator/CustomIcons");
        }

        private static void LoadFolder(string folder)
        {
            if (!AssetDatabase.IsValidFolder(folder)) return;
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            var paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++) paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            Array.Sort(paths, StringComparer.Ordinal);
            foreach (var path in paths)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture != null) Icons[Path.GetFileNameWithoutExtension(path)] = texture;
            }
        }
    }
}
