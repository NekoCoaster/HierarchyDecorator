using System;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    // Generated textures are editor-only copies; Unity's shared icons are never modified.
    [InitializeOnLoad]
    internal static class ConstraintIconHues
    {
        internal static readonly string[] Names = { "Aim", "LookAt", "Parent", "Position", "Rotation", "Scale" };
        private static readonly Type[] Types = {
            typeof(UnityEngine.Animations.AimConstraint), typeof(UnityEngine.Animations.LookAtConstraint),
            typeof(UnityEngine.Animations.ParentConstraint), typeof(UnityEngine.Animations.PositionConstraint),
            typeof(UnityEngine.Animations.RotationConstraint), typeof(UnityEngine.Animations.ScaleConstraint)
        };
        private sealed class Entry
        {
            internal Texture Source;
            internal Color[] Pixels;
            internal Texture2D Shifted;
            internal float Hue = float.NaN;
        }
        private static readonly Entry[] Cache = new Entry[6];

        static ConstraintIconHues()
        {
            AssemblyReloadEvents.beforeAssemblyReload += Clear;
            EditorApplication.quitting += Clear;
            EditorApplication.projectChanged += Clear;
            Undo.undoRedoPerformed += EditorApplication.RepaintHierarchyWindow;
        }

        internal static int Index(Type type)
        {
            for (; type != null; type = type.BaseType)
                for (int i = 0; i < Names.Length; i++)
                    if (type == Types[i] || type.FullName == "VRC.SDK3.Dynamics.Constraint.Components.VRC" + Names[i] + "Constraint")
                        return i;
            return -1;
        }

        internal static GUIContent Resolve(Type type, GUIContent original, ComponentData settings)
        {
            if (!settings.constraintHueEnabled) return original;
            int index = Index(type);
            if (index < 0) return original;
            float hue = settings.constraintHueShifts != null && index < settings.constraintHueShifts.Length
                ? settings.constraintHueShifts[index] : 0f;
            var image = Get(index, hue);
            return image == null ? original : new GUIContent(original.text, image, original.tooltip);
        }

        internal static Texture Get(int index, float degrees)
        {
            var source = EditorGUIUtility.ObjectContent(null, Types[index]).image;
            if (source == null) return null;
            var entry = Cache[index];
            if (entry != null && entry.Source != source) { Destroy(entry); Cache[index] = entry = null; }
            if (Mathf.Approximately(Mathf.Repeat(degrees, 360f), 0f)) return source;
            if (entry == null)
            {
                // Built-in textures need not be CPU-readable. Read one temporary GPU copy.
                var active = RenderTexture.active;
                bool srgb = GL.sRGBWrite;
                var rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                Texture2D copy = null;
                try
                {
                    GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
                    Graphics.Blit(source, rt);
                    RenderTexture.active = rt;
                    copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
                    copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
                    copy.Apply();
                    entry = new Entry { Source = source, Pixels = copy.GetPixels(), Shifted = copy };
                    copy.hideFlags = HideFlags.HideAndDontSave;
                    copy.name = "HierarchyDecorator " + Names[index] + " hue preview";
                    copy.filterMode = source.filterMode;
                    copy.wrapMode = TextureWrapMode.Clamp;
                    Cache[index] = entry;
                }
                catch (Exception exception)
                {
                    if (copy != null) UnityEngine.Object.DestroyImmediate(copy);
                    Cache[index] = new Entry { Source = source };
                    Debug.LogWarning("HierarchyDecorator could not preview the " + Names[index] + " constraint hue: " + exception.Message);
                    return source;
                }
                finally
                {
                    RenderTexture.active = active;
                    GL.sRGBWrite = srgb;
                    RenderTexture.ReleaseTemporary(rt);
                }
            }
            if (entry.Pixels == null) return source;
            if (entry.Hue != degrees)
            {
                var pixels = new Color[entry.Pixels.Length];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = Shift(entry.Pixels[i], degrees);
                entry.Shifted.SetPixels(pixels);
                entry.Shifted.Apply();
                entry.Hue = degrees;
            }
            return entry.Shifted;
        }

        internal static Color Shift(Color color, float degrees)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            var shifted = Color.HSVToRGB(Mathf.Repeat(h + degrees / 360f, 1f), s, v);
            shifted.a = color.a;
            return shifted;
        }

        private static void Destroy(Entry entry)
        {
            if (entry?.Shifted != null) UnityEngine.Object.DestroyImmediate(entry.Shifted);
        }

        internal static void Clear()
        {
            for (int i = 0; i < Cache.Length; i++) { Destroy(Cache[i]); Cache[i] = null; }
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
