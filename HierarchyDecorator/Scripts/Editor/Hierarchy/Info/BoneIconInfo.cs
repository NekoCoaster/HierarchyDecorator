using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HierarchyDecorator
{
    // Rebuild rig membership on edits, never scan the scene for each hierarchy row.
    [InitializeOnLoad]
    internal sealed class BoneIconInfo : HierarchyInfo
    {
        private static readonly HashSet<Transform> Bones = new HashSet<Transform>();
        private static bool dirty = true;
        private static GUIContent icon;

        static BoneIconInfo()
        {
            EditorApplication.hierarchyChanged += Invalidate;
            Undo.undoRedoPerformed += Invalidate;
            EditorApplication.projectChanged += Invalidate;
            EditorApplication.playModeStateChanged += _ => Invalidate();
#if UNITY_2021_2_OR_NEWER
            ObjectChangeEvents.changesPublished += OnObjectsChanged;
#endif
        }
#if UNITY_2021_2_OR_NEWER
        private static void OnObjectsChanged(ref ObjectChangeEventStream stream) { Invalidate(); }
#endif
        private static void Invalidate() { dirty = true; }

        internal static bool IsBoneOrAttachment(Transform transform)
        {
            if (dirty) Rebuild();
            for (var current = transform; current != null; current = current.parent)
                if (Bones.Contains(current)) return true;
            return false;
        }

        private static void Rebuild()
        {
            dirty = false;
            Bones.Clear();
            // Includes inactive rigs and the prefab stage; exclude project assets.
            foreach (var renderer in Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>())
            {
                if (EditorUtility.IsPersistent(renderer) || !renderer.gameObject.scene.IsValid()) continue;
                if (renderer.rootBone != null) Bones.Add(renderer.rootBone);
                foreach (var bone in renderer.bones)
                    if (bone != null) Bones.Add(bone);
            }
            foreach (var animator in Resources.FindObjectsOfTypeAll<Animator>())
            {
                if (EditorUtility.IsPersistent(animator) || !animator.gameObject.scene.IsValid() ||
                    animator.avatar == null || !animator.avatar.isValid || !animator.isHuman) continue;
                for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
                {
                    var bone = animator.GetBoneTransform((HumanBodyBones)i);
                    if (bone != null) Bones.Add(bone);
                }
            }
        }

        private static Texture2D CreateBoneIcon()
        {
            var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false)
            { name = "HierarchyDecorator Bone", hideFlags = HideFlags.HideAndDontSave };
            // Draw a diagonal bone with rounded paired ends; no external artwork dependency.
            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 32; x++)
                {
                    float u = (x + y - 31) * 0.7071f;
                    float v = (x - y) * 0.7071f;
                    bool shaft = Mathf.Abs(u) < 10 && Mathf.Abs(v) < 2.6f;
                    float du = Mathf.Abs(u) - 9f;
                    float dv = Mathf.Abs(v) - 3f;
                    bool end = du * du + dv * dv < 17;
                    texture.SetPixel(x, y, shaft || end ? new Color(0.78f, 0.72f, 0.57f, 1) : Color.clear);
                }
            texture.Apply();
            return texture;
        }

        protected override bool DrawerIsEnabled(HierarchyItem item, Settings settings)
        {
            return settings.Components.Enabled && settings.Components.ShowBoneIcons &&
                (settings.styleData.displayIcons || !settings.styleData.HasStyle(item.DisplayName)) &&
                IsBoneOrAttachment(item.Transform);
        }
        protected override int CalculateGridCount() { return 1; }
        protected override void DrawInfo(Rect rect, HierarchyItem item, Settings settings)
        {
            if (icon == null) icon = new GUIContent(CreateBoneIcon(),
                "Rig bone or object parented beneath a rig bone");
            GUI.Label(rect, icon, Style.ComponentIconStyle);
        }
    }
}
