using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTilemap {
    public class TilesetModel : MonoBehaviour {
        List<TilemapVisual> _currentTilemapVisuals;
        Dictionary<string, Material> _currentTilesetMaterials;
        Dictionary<string, Sprite> _currentTilesetSprites;
        int _selectedTilesetIndex;
        void Awake() {
            _currentTilemapVisuals = new List<TilemapVisual>();
            _currentTilesetMaterials = new Dictionary<string, Material>();
            _currentTilesetSprites = new Dictionary<string, Sprite>();
        }

        public TilemapVisual GetTilemapVisual(string tilesetName) {
            foreach (var visual in _currentTilemapVisuals) {
                if (visual.Equals(tilesetName)) return visual;
            }
            return null;
        }

        public void ChangeTileset(string tilesetName, int layerIndex) {
            Material material = _currentTilesetMaterials[tilesetName];
            _currentTilemapVisuals[layerIndex].Initalize(material);
        }

        public Material GetMaterialByName(string name) {
            return _currentTilesetMaterials[name];
        }
        
        public TilemapVisual GetFirstTilemapVisual() {
            TilemapVisual visual = (_currentTilemapVisuals.Count == 0) ? null : _currentTilemapVisuals[0];
            return visual;
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

        public void AddTilemapVisual(TilemapVisual visual) {
            _currentTilemapVisuals.Add(visual);
        }

        public Sprite GetTilesetSprite(string tilesetName) {
            return _currentTilesetSprites[tilesetName];
        }
    }
}
