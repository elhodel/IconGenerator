using System;
using UnityEngine;

namespace elhodel.IconGenerator
{
    public class IconGenerationScope : IDisposable
    {
        public Texture2D Icon { get; private set; }

        private IconGeneratorSettings _settings;
        private GameObject _spawnedPrefab;

        public IconGenerationScope(IconGeneratorSettings iconSettings)
        {
            _settings = iconSettings;

            GameObject prefab = _settings.GetGeneratorPrefab();

            _spawnedPrefab = GameObject.Instantiate(prefab);
            IconController controller = _spawnedPrefab.GetComponentInChildren<IconController>();

            controller.IconSettings = _settings.IconSettings;

            Icon = controller.GenerateIcon(_settings.Resolution);
        }

        public void Dispose()
        {
            GameObject.DestroyImmediate(_spawnedPrefab);
        }
    }

}