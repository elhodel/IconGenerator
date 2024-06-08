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
        private const string _prefabGuid = "effab360f12d890429b41bc75807e34a";


        [SerializeField]
        private bool _generateIcon = true;

        [SerializeField]
        private int _resolution = 1024;

        [SerializeField]
        private string _savePath = "ProjectIcon.png";

        [SerializeField]
        private IconSettings _iconSettings;

        public GameObject GetGeneratorPrefab()
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(_prefabGuid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            return prefab;
        }

        [ContextMenu("Test Icon Generation")]
        private void TestIconGeneration()
        {
            using (var iconGenerator = new IconGenerationScope(this))
            {
                var persistentIcon = Utils.SaveTextureToAssetDatabase(iconGenerator.Icon, SavePath);

            }
        }

    }

}