using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    // Optional integrations: neither SDK is a compile-time dependency.
    [InitializeOnLoad]
    internal static class ComponentIconResolver
    {
        private static readonly Dictionary<Type, Texture> Icons = new Dictionary<Type, Texture>();

        static ComponentIconResolver()
        {
            EditorApplication.projectChanged += ClearCache;
        }

        internal static void ClearCache() { Icons.Clear(); }

        internal static Texture Resolve(Type type, MonoScript script, Texture fallback)
        {
            if (type == null) return fallback;
            if (Icons.TryGetValue(type, out var cached)) return cached != null ? cached : fallback;
            // Explicit associations (e.g. AudioLink) take precedence over the generic U# family fallback.
            Texture icon = type.FullName == "VRC.Udon.UdonBehaviour"
                ? Resources.Load<Texture2D>("UdonSharpProgramAsset icon") : null;
            if (icon == null) icon = CustomComponentIcons.Find(type.FullName) ?? CustomComponentIcons.Find(type.Name);
            if (icon == null) icon = ResolveDismayIcon(type, script);
            for (var current = type; icon == null && current != null; current = current.BaseType)
            {
                if (current.FullName == "VRC.Udon.UdonBehaviour" || current.FullName == "UdonSharp.UdonSharpBehaviour")
                {
                    icon = Resources.Load<Texture2D>("UdonSharpProgramAsset icon");
                    break;
                }
            }
            if (icon == null)
                for (var current = type; current != null; current = current.BaseType)
                    if (current.FullName == "VF.Component.VRCFuryComponent")
                    {
                        icon = CustomComponentIcons.Find("VRCFury") ?? AssetDatabase.LoadAssetAtPath<Texture2D>(
                            "Packages/com.vrcfury.vrcfury/VrcfResources/logo.png");
                        break;
                    }
#if UNITY_2021_2_OR_NEWER
            if (icon == null && script != null)
                icon = EditorGUIUtility.GetIconForObject(script);
#endif
            if (typeof(MonoBehaviour).IsAssignableFrom(type) && IsGeneric(icon != null ? icon : fallback))
                icon = CustomComponentIcons.Find("Default") ?? icon;
            Icons[type] = icon;
            return icon != null ? icon : fallback;
        }

        private static Texture ResolveDismayIcon(Type type, MonoScript script)
        {
            if (!typeof(MonoBehaviour).IsAssignableFrom(type)) return null;
            const string pcs = "Assets/!Dismay Custom/Penetration Contact System/";
            const string lms = "Assets/!Dismay Custom/Lovestick Massages SFX/";
            string folder = null;
            for (var current = type; current != null; current = current.BaseType)
            {
                if (current.FullName == "DismayCustom.PCS.PCSComponentBase") { folder = pcs; break; }
                // LMS has no runtime component yet: restrict provisional matching to vendor namespaces.
                if (InNamespace(current, "DismayCustom.LMS") || InNamespace(current, "DMCustom.LMS")) { folder = lms; break; }
            }
            if (folder == null && script != null)
            {
                var path = AssetDatabase.GetAssetPath(script);
                if (path.StartsWith(pcs, StringComparison.Ordinal)) folder = pcs;
                else if (path.StartsWith(lms, StringComparison.Ordinal)) folder = lms;
            }
            if (folder == null) return null;
            return AssetDatabase.LoadAssetAtPath<Texture2D>(folder + "Assets/Icons/" + (folder == pcs ? "PCS" : "LMS") + " Icon.png");
        }

        private static bool InNamespace(Type type, string name)
        {
            return type.Namespace == name || (type.Namespace != null && type.Namespace.StartsWith(name + ".", StringComparison.Ordinal));
        }

        private static bool IsGeneric(Texture texture)
        {
            return texture == null || texture == EditorGUIUtility.IconContent("cs Script Icon").image ||
                texture == EditorGUIUtility.IconContent("d_cs Script Icon").image ||
                texture == EditorGUIUtility.IconContent("dll Script Icon").image;
        }
    }
}
