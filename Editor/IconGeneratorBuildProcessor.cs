using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;

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

            if (!TextMeshProExists())
            {
                int result = EditorUtility.DisplayDialogComplex("TextMeshPro missing for Icon Generator!", "TextMeshPro Essentials are missing but are required for the Icon Generator to work. \n\n Should it be imported now?", "Import now (Cancels the Build)", "Don't generate Icon this time", "Disable Icon Generator");

                if (result == 0)
                {
                    TMP_PackageUtilities.ImportProjectResourcesMenu();
                    throw new BuildFailedException("Icon Generator needs TextMeshPro Essentials . Import the Essentials now.");
                }
                else if (result == 1)
                {
                    return;
                }
                else
                {
                    IconGeneratorSettings.instance.GenerateIcon = false;
                    return;
                }

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

        private static bool TextMeshProExists()
        {
            /// Do the same check as TMPro makes in <see cref="TMP_Settings.instance"/>
            /// Don't check against <see cref="TMP_Settings.instance"/> because the Editor window should not be opened in case the instance is null
            TMP_Settings tmpSettings = Resources.Load<TMP_Settings>("TMP Settings");
            return tmpSettings != null;
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
