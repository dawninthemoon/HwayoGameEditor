using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTilemap {
    public delegate void OnTilesetChanged(TilemapVisual tilemapVisual);
    public class LayerModel : MonoBehaviour {
        List<Layer> _currentLayers;
        Image _selectedButtonImage;
        int _selectedLayerIndex;

        OnTilesetChanged _onTilesetChanged;

        void Awake() {
            _currentLayers = new List<Layer>();
        }

        public void SetOnTilesetChanged(OnTilesetChanged callback) {
            _onTilesetChanged = callback;
        }

        public void AddLayer(Layer layer, Button button, TilemapVisual visual, System.Action callback) {
            if (_currentLayers.Count == 0) {
                OnButtonClick(button, 0, visual, callback);
            }
            _currentLayers.Add(layer);
        }

        public void SetTile(Vector3 worldPosition, int tileIndex) {
            if (_currentLayers.Count == 0) return;
            _currentLayers[_selectedLayerIndex].SetTileIndex(worldPosition, tileIndex);
        }

        public Layer GetSelectedLayer() {
            for (int i = 0; i < _currentLayers.Count; ++i) {
                if (_currentLayers[i].LayerIndex == _selectedLayerIndex) {
                    return _currentLayers[i];
                }
            }
            return null;
        }

        public void AddButtonOnClick(Button button, int layerIndex, TilemapVisual tilemapVisual, System.Action callback) {
            button.onClick.AddListener(() => { 
                OnButtonClick(button, layerIndex, tilemapVisual, callback);
            });
        }

        void OnButtonClick(Button button, int layerIndex, TilemapVisual tilemapVisual, System.Action callback) {
            if (_selectedButtonImage != null) {
                _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
                _selectedButtonImage.GetComponentInChildren<Text>().color = Color.white;
            }
            _selectedLayerIndex = layerIndex;
            _selectedButtonImage = button.image;

            button.image.color = new Color(1f, 0.7626624f, 0.2122642f);
            _selectedButtonImage.GetComponentInChildren<Text>().color = Color.black;

            _onTilesetChanged(tilemapVisual);
            callback();
        }
    }
}
