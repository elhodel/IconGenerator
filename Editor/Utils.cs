using System;
using System.IO;

using UnityEditor;
using UnityEngine;

namespace elhodel.IconGenerator
{
    public static class Utils
    {
      
        /// <summary>
        /// Save given Texture on Disk and import it to AssetDatabase.
        /// </summary>
        /// <param name="texture">Texture to save</param>
        /// <param name="path">Path relative to <see cref="Application.dataPath"/></param>
        /// <returns>Saved Texture loaded from AssetDatabase</returns>
        public static Texture2D SaveTextureToAssetDatabase(Texture2D texture, string path)
        {

            string outputPath = Path.Combine(Application.dataPath, path);

            var textureBytes = texture.EncodeToPNG();

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            File.WriteAllBytes(outputPath, textureBytes);

            string assetDatabasePath = GetAssetDatabasePath(outputPath);

            AssetDatabase.ImportAsset(assetDatabasePath);

            var loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetDatabasePath);

            return loadedTexture;
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
