using TMPro;
using UnityEngine;

namespace elhodel.IconGenerator
{
    [System.Serializable]
    public class IconSettings
    {
        public Texture2D Background;
        
        public TMP_FontAsset FontAsset;

        public bool TintIcon = true;

        public bool RandomColor = false;

        public bool ShowProjectName = true;

        public bool ShowTimestamp = true;

        public string TimestampFormat = "yyMMdd_HHmmss";

        public bool ShowVersion = true;
    }
}