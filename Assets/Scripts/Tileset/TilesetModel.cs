using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTilemap {
    public class TilesetModel : MonoBehaviour {
        List<TilesetVisual> _currentTilemapVisuals;
        Dictionary<string, Material> _currentTilesetMaterials;
        Dictionary<string, Sprite> _currentTilesetSprites;
        public Material EntityVisualMaterial { get; private set; }
        int _selectedTilesetIndex;
        public static string DefaultTilesetName;
        void Awake() {
            _currentTilemapVisuals = new List<TilesetVisual>();
            _currentTilesetMaterials = new Dictionary<string, Material>();
            _currentTilesetSprites = new Dictionary<string, Sprite>();
            EntityVisualMaterial = Resources.Load<Material>("EntityVisual/EntityVisual");
        }

        public void ChangeTileset(string tilesetName, TilemapVisual visual) {
            Material material = _currentTilesetMaterials[tilesetName];
            visual.Initalize(material);
        }

        public Material GetMaterialByName(string name) {
            return _currentTilesetMaterials[name];
        }

        public Material GetFirstMaterial() {
            foreach (var material in _currentTilesetMaterials.Values) {
                return material;
            }
            return null;
        }

        public void SetTilesets(Material[] materials, Sprite[] sprites) {
            if (materials.Length != sprites.Length)
                Debug.LogError("Material-Sprite pair error");
            for (int i = 0; i < materials.Length; ++i) {
                string key = sprites[i].name;
                _currentTilesetSprites.Add(key, sprites[i]);
                _currentTilesetMaterials.Add(key, materials[i]);
            }
        }

        public void SetTilesetIndex(int index) {
            _selectedTilesetIndex = index;
        }

        public void AddTilesetVisual(TilesetVisual visual) {
            _currentTilemapVisuals.Add(visual);
        }

        public Sprite GetTilesetSprite(string tilesetName) {
            return _currentTilesetSprites[tilesetName];
        }
    }
}
