using elhodel.EasyEditorSettings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace elhodel.IconGenerator
{

    [EasyEditorSettings(SettingsScope.Project, "ProjectSettings/Tools/IconGenerator.asset", "Tools/Icon Generator")]
    public partial class IconGeneratorSettings : ScriptableSingleton<IconGeneratorSettings>
    {
        [SerializeField]
        private bool _generateIcon = true;

        [SerializeField]
        private int _resolution = 1024;

        [SerializeField]
        private string _savePath = "ProjectIcon.png";

        [SerializeField]
        private Texture2D _background;

        [SerializeField]
        private IconSettings _iconSettings;
    }

}