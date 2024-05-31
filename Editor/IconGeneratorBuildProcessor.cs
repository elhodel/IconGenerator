using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace elhodel.IconGenerator
{

    public class IconGeneratorBuildProcessor : IPreprocessBuildWithReport
    {
        private const string _prefabGuid = "effab360f12d890429b41bc75807e34a";

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!IconGeneratorSettings.instance.GenerateIcon)
            {
                return;
            }

            IconGeneratorSettings settings = IconGeneratorSettings.instance;

            string prefabPath = AssetDatabase.GUIDToAssetPath(_prefabGuid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            GameObject spawned = GameObject.Instantiate(prefab);
            IconController controller = spawned.GetComponentInChildren<IconController>();

            controller.IconSettings = settings.IconSettings;
            controller.SetBackground(settings.Background);

            var icon = controller.GenerateIcon(settings.Resolution);


            string outputPath = Path.Combine(Application.dataPath, settings.SavePath);

            var textureBytes = icon.EncodeToPNG();

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            File.WriteAllBytes(outputPath, textureBytes);

            string assetDatabasePath = GetAssetDatabasePath(outputPath);

            AssetDatabase.ImportAsset(assetDatabasePath);

            var loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDatabasePath);

            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { loadedTexture }, IconKind.Application);

            GameObject.DestroyImmediate(spawned);


        }

        public static string GetRelativePath(string fullPath, string basePath, bool useBackwardSlashes = false, bool keepFileExtension = true)
        {
            // Require trailing backslash for path
            if (!basePath.EndsWith("\\"))
                basePath += "\\";

            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(fullPath);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            //unescaping the Uri replaces keeps spaces that would otherwise be %20.
            string result = Uri.UnescapeDataString(relativeUri.ToString());
            string relativeUriUnescaped = Uri.UnescapeDataString(relativeUri.ToString());

            if (useBackwardSlashes)
                result = relativeUriUnescaped.Replace("/", "\\");

            if (!keepFileExtension)
                result = Path.ChangeExtension(relativeUriUnescaped, null);

            return result;
        }

        /// <summary>
        /// Convert FullPath to Path relative to Project Root that can be used to load from AssetDatabase
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>Path for AssetDatabase starting with "Assets/..."</returns>
        public static string GetAssetDatabasePath(string fullPath)
        {
            return Path.Combine("Assets", GetRelativePath(fullPath, Application.dataPath));
        }
    }
}
