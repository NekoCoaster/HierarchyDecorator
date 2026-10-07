using UnityEngine;

namespace HierarchyDecorator
{
    /// <summary>
    /// ScriptableObject containing all settings and relevant data for the hierarchy
    /// </summary>
    public class Settings : ScriptableObject, ISerializationCallbackReceiver
    {
        // Fields

        public GlobalData globalData = new GlobalData ();
        public HierarchyStyleData styleData = new HierarchyStyleData ();

        [SerializeField]
        private ComponentData components = new ComponentData ();

        [SerializeField, HideInInspector] private int headerPrefixVersion;

        internal bool UpgradeHeaderPrefixes()
        {
            if (headerPrefixVersion >= 1) return false;
            if (styleData != null && styleData.styles != null)
                foreach (var style in styleData.styles)
                {
                    if (style == null || style.isRegex) continue;
                    if (style.name == "Header (Centered)" && style.prefix == "=") style.prefix = "===";
                    else if (style.name == "Subheader" && style.prefix == "-") style.prefix = "---";
                    else if (style.name == "Mini Header (Centered)" && style.prefix == "+") style.prefix = "+++";
                }
            headerPrefixVersion = 1;
            return true;
        }

        // Properties

        public ComponentData Components
        {
            get
            {
                return components;
            }
        }

        // Settings Creation

        private void OnEnable()
        {
            components.OnInitialize ();
        }

        /// <summary>
        /// Setup defaults for the new settings asset
        /// </summary>
        internal void SetDefaults(bool isDarkMode)
        {
            components.UpdateData ();
            styleData.UpdateStyles (isDarkMode);
        }

        // Serialization

        public void OnBeforeSerialize()
        {
            components.UpdateData ();
        }

        public void OnAfterDeserialize()
        {
            components.UpdateData ();
        }
    }
}
