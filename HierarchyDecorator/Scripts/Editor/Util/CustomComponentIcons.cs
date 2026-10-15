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
        internal static readonly Color DefaultTint = new Color(96f / 255f, 218f / 255f, 230f / 255f, 1f);
        internal static readonly string[] TintComponents = { "VRCSceneDescriptor", "VRCContactSender", "VRCContactReceiver", "VRCAvatarDescriptor", "PipelineManager", "VRCPhysBone", "VRCPhysBoneCollider", "GestureManager", "LyumaAv3Osc", "AudioLink", "Missing" };
        internal static readonly string[] TintLabels = { "Scene descriptor", "Contact sender", "Contact receiver", "Avatar descriptor", "Pipeline manager", "PhysBone", "PhysBone collider", "Gesture Manager", "Lyuma AV3 OSC", "AudioLink family", "Missing script" };
        private static readonly Dictionary<Texture2D, int> TintIndices = new Dictionary<Texture2D, int>();
        private static readonly Dictionary<Texture2D, Texture2D> Tinted = new Dictionary<Texture2D, Texture2D>();
        private static readonly HashSet<Texture2D> BundledArtwork = new HashSet<Texture2D>();
        private static readonly Dictionary<string, string> Associations = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "VRCSceneDescriptor", "icons8-inscription-48" },
            { "VRCContactSender", "icons8-external-48" },
            { "VRCContactReceiver", "icons8-internal-48" },
            { "VRCAvatarDescriptor", "icons8-profile-50" },
            { "PipelineManager", "icons8-identification-documents-50" },
            { "VRCPhysBone", "icons8-bone-48" },
            { "VRCPhysBoneCollider", "icons8-sphere-48" },
            { "GestureManager", "icons8-stop-gesture-48" },
            { "LyumaAv3Osc", "icons8-ethernet-settings-50" },
            { "AudioLink", "icons8-itunes-note-32" },
            { "AudioLinkController", "icons8-itunes-note-32" },
            { "AudioLinkControllerV0", "icons8-itunes-note-32" },
            { "AudioLinkZone", "icons8-itunes-note-32" },
            { "Missing", "icons8-questions-48" }
        };

        static CustomComponentIcons()
        {
            EditorApplication.projectChanged += Refresh;
            AssemblyReloadEvents.beforeAssemblyReload += ReleaseTextures;
            EditorApplication.quitting += ReleaseTextures;
            Undo.undoRedoPerformed += Refresh;
        }

        private static void ReleaseTextures()
        {
            foreach (var texture in Tinted.Values)
                if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
            Tinted.Clear();
            BundledArtwork.Clear();
            TintIndices.Clear();
        }

        [MenuItem("Tools/HierarchyDecorator/Refresh Custom Icons")]
        internal static void Refresh()
        {
            loaded = false;
            ReleaseTextures();
            Icons.Clear();
            ComponentIconResolver.ClearCache();
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem("Tools/HierarchyDecorator/Icon Artwork Credits")]
        private static void ShowArtworkCredits()
        {
            if (EditorUtility.DisplayDialog("Icon Artwork Credits",
                "Icons by Icons8: https://icons8.com\n\n" +
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
            if (!Icons.TryGetValue(componentName, out var icon)) return null;
            if (!BundledArtwork.Contains(icon)) return icon;
            if (Tinted.TryGetValue(icon, out var tinted)) return tinted;
            // Only our bundled white artwork is tinted. User overrides and SDK icons retain their colours.
            var pixels = icon.GetPixels();
            var colors = HierarchyDecorator.Settings.Components.icons8Tints;
            var index = TintIndices[icon];
            var tint = colors != null && index < colors.Length ? colors[index] : DefaultTint;
            tint.a = 1f; // Colour controls never change artwork opacity.
            for (int i = 0; i < pixels.Length; i++) pixels[i] *= tint;
            tinted = new Texture2D(icon.width, icon.height, TextureFormat.RGBA32, false)
            {
                name = icon.name + " #" + ColorUtility.ToHtmlStringRGB(tint).ToLowerInvariant(), hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp
            };
            tinted.SetPixels(pixels);
            tinted.Apply();
            Tinted[icon] = tinted;
            return tinted;
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
                foreach (var association in Associations)
                    if (Icons.TryGetValue(association.Value, out var source))
                    {
                        Icons[association.Key] = source;
                        BundledArtwork.Add(source);
                    }
                for (int i = 0; i < TintComponents.Length; i++)
                    if (Icons.TryGetValue(TintComponents[i], out var source)) TintIndices[source] = i;
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
