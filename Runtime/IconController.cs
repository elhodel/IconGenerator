using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace elhodel.IconGenerator
{


    public class IconController : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private RawImage _background;

        [SerializeField]
        private TMP_Text _projectNameField;
        [SerializeField]
        private TMP_Text _timestampField;
        [SerializeField]
        private TMP_Text _versionField;


        public IconSettings IconSettings
        {
            get => _iconSettings;
            set
            {
                _iconSettings = value;
            }
        }
        private IconSettings _iconSettings;

        public int MaxColors = 16;

        [ContextMenu("Update Content")]
        public void UpdateContent()
        {
            _projectNameField.gameObject.SetActive(_iconSettings.ShowProjectName);
            _projectNameField.text = Application.productName;

            _timestampField.gameObject.SetActive(_iconSettings.ShowTimestamp);
            _timestampField.text = DateTime.Now.ToString("yyMMdd_HHmmss");

            _versionField.gameObject.SetActive(_iconSettings.ShowVersion);
            _versionField.text = Application.version;

            if (!_iconSettings.TintIcon)
            {
                return;
            }
            if (_iconSettings.RandomColor)
            {
                Color randomColor = Color.HSVToRGB(UnityEngine.Random.value, 0.8f, 0.7f);
                _background.color = randomColor;
            }
            else
            {
                var versionParts = Application.version.Split(".").Select(f =>
                {
                    if (int.TryParse(f, out int result))
                    {
                        return result;
                    }
                    return -1;
                }).Where(v => v >= 0).ToArray();


                int hueIndex = versionParts[^1] % MaxColors;

                if (versionParts[^1] % 2 == 0)
                {
                    hueIndex = (versionParts[^1] + MaxColors / 2) % MaxColors;
                }
                float hue = Map(hueIndex, 0, MaxColors - 1, 0, 1);
                Color color = Color.HSVToRGB(hue, 0.8f, 0.7f);
                _background.color = color;
            }
        }

        public Texture2D GenerateIcon(int resolution)
        {
            UpdateContent();

            RenderTexture renderTexture = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGB32);
            _camera.targetTexture = renderTexture;
            _camera.forceIntoRenderTexture = true;

            _camera.Render();

            return renderTexture.ToTexture2D();

        }

        public float Map(float input, float inMin, float inMax, float outMin, float outMax)
        {
            return (input - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

        }

        public void SetBackground(Texture2D background)
        {
            _background.texture = background;
        }
    }


    public static class ExtensionMethod
    {
        public static Texture2D ToTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }
    }
}