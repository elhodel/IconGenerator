using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace elhodel.IconGenerator
{

    public class IconGeneratorBuildProcessor : IPreprocessBuildWithReport
    {

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

            using (var iconGenerator = new IconGenerationScope(settings))
            {
                var persistentIcon = Utils.SaveTextureToAssetDatabase(iconGenerator.Icon, settings.SavePath);

                PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { persistentIcon }, IconKind.Application);
            }

        }

        private static bool TextMeshProExists()
        {
            /// Do the same check as TMPro makes in <see cref="TMP_Settings.instance"/>
            /// Don't check against <see cref="TMP_Settings.instance"/> because the Editor window should not be opened in case the instance is null
            TMP_Settings tmpSettings = Resources.Load<TMP_Settings>("TMP Settings");
            return tmpSettings != null;
        }


    }
}
