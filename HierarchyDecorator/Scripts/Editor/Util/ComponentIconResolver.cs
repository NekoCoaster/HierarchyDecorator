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
            EditorApplication.projectChanged += Icons.Clear;
        }

        internal static Texture Resolve(Type type, MonoScript script, Texture fallback)
        {
            if (type == null) return fallback;
            if (Icons.TryGetValue(type, out var cached)) return cached != null ? cached : fallback;
            Texture icon = null;
            for (var current = type; current != null; current = current.BaseType)
            {
                if (current.FullName == "VF.Component.VRCFuryComponent")
                {
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        "Packages/com.vrcfury.vrcfury/VrcfResources/logo.png");
                    break;
                }
                if (current.FullName == "VRC.Udon.UdonBehaviour" || current.FullName == "UdonSharp.UdonSharpBehaviour")
                {
                    icon = Resources.Load<Texture2D>("UdonSharpProgramAsset icon");
                    break;
                }
            }
#if UNITY_2021_2_OR_NEWER
            if (icon == null && script != null)
                icon = EditorGUIUtility.GetIconForObject(script);
#endif
            Icons[type] = icon;
            return icon != null ? icon : fallback;
        }
    }
}
