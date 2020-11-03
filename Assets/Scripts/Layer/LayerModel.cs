using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aroma;

namespace CustomTilemap {
    public delegate void OnTilesetChanged(TilemapVisual tilemapVisual);
    public class LayerModel : MonoBehaviour {
        Dictionary<int, Layer> _currentLayerDictionary;
        Image _selectedButtonImage;
        public int SelectedLayerID { get; set; } = -1;
        OnTilesetChanged _onTilesetChanged;
        public static int CurrentGridWidth;
        public static int CurrentGridHeight;
        OnGridResized _onGridResized;

        void Awake() {
            _currentLayerDictionary = new Dictionary<int, Layer>();
            int gridSize = PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey) ? PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey) : 16;
            CurrentGridWidth = CurrentGridHeight = gridSize;
        }

        public void ResizeAllLayers(Vector3 originPosition, int widthDelta, int heightDelta) {
            if (widthDelta == 0 && heightDelta == 0) return;
            foreach (var layer in _currentLayerDictionary.Values) {
                layer.ResizeGrid(originPosition, widthDelta, heightDelta);
            }

            LayerModel.CurrentGridWidth += widthDelta;
            LayerModel.CurrentGridHeight += heightDelta;
            _onGridResized?.Invoke();
        }

        public void SetOnGridResized(OnGridResized callback) {
            _onGridResized = callback;
        }

        public bool IsLayerEmpty() => (_currentLayerDictionary.Count == 0);

        public void SetOnTilesetChanged(OnTilesetChanged callback) {
            _onTilesetChanged = callback;
        }

        public void AddLayer(Layer layer, Button button, TilemapVisual visual, System.Action callback) {
            if (_currentLayerDictionary.Count == 0) {
                OnButtonClick(button, 0, visual, callback);
            }
            _currentLayerDictionary.Add(layer.LayerID, layer);
        }

        public void DeleteLayerByID(int layerID) {
            _currentLayerDictionary.Remove(layerID);
        }

        public void SetTile(Vector3 worldPosition, int tileIndex) {
            if (_currentLayerDictionary.Count == 0) return;
            _currentLayerDictionary[SelectedLayerID].SetTileIndex(worldPosition, tileIndex);
        }

        public Layer GetSelectedLayer() {
            for (int i = 0; i < _currentLayerDictionary.Count; ++i) {
                if (_currentLayerDictionary[i].LayerID == SelectedLayerID) {
                    return _currentLayerDictionary[i];
                }
            }
            return null;
        }

        public void AddButtonOnClick(Button button, int layerID, TilemapVisual tilemapVisual, System.Action callback) {
            button.onClick.AddListener(() => { 
                OnButtonClick(button, layerID, tilemapVisual, callback);
            });
        }

        void OnButtonClick(Button button, int layerID, TilemapVisual tilemapVisual, System.Action callback) {
            if (_selectedButtonImage != null) {
                _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
                _selectedButtonImage.GetComponentInChildren<Text>().color = Color.white;
            }
            SelectedLayerID = layerID;
            _selectedButtonImage = button.image;

            button.image.color = new Color(1f, 0.7626624f, 0.2122642f);
            _selectedButtonImage.GetComponentInChildren<Text>().color = Color.black;

            _onTilesetChanged(tilemapVisual);
            callback();
        }
    }
}
