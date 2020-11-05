using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aroma;

namespace CustomTilemap {
    public delegate void OnTilesetChanged(TileLayer tileLayer);
    public class LayerModel : MonoBehaviour {
        [SerializeField] LayerPicker _layerPicker = null;
        Dictionary<int, Layer> _currentLayerDictionary;
        Image _selectedButtonImage;
        public int SelectedLayerID { get; set; } = -1;
        public static int CurrentGridWidth;
        public static int CurrentGridHeight;
        public static Vector3 CurrentOriginPosition;
        OnGridResized _onGridResized;

        void Awake() {
            _currentLayerDictionary = new Dictionary<int, Layer>();
            int gridSize = PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey) ? PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey) : 16;
            CurrentGridWidth = CurrentGridHeight = gridSize;
        }

        public void ResizeAllLayers(Vector3 originPosition, int widthDelta, int heightDelta) {
            LayerModel.CurrentGridWidth += widthDelta;
            LayerModel.CurrentGridHeight += heightDelta;

            if (widthDelta == 0 && heightDelta == 0) return;
            foreach (var layer in _currentLayerDictionary.Values) {
                layer.ResizeGrid(originPosition, widthDelta, heightDelta);
            }
            CurrentOriginPosition = originPosition;
            _onGridResized?.Invoke();
        }

        public void SetOnGridResized(OnGridResized callback) {
            _onGridResized = callback;
        }

        public bool IsLayerEmpty() => (_currentLayerDictionary.Count == 0);
        public int NumOfLayers() => _currentLayerDictionary.Count;

        public void AddLayer(Layer layer, Button button, System.Action callback) {
            _currentLayerDictionary.Add(layer.LayerID, layer);
            _layerPicker.AddLayerButton(this, layer);

            if (_currentLayerDictionary.Count == 1) {
                OnButtonClick(button, layer, callback);
            }
        }

        public void DeleteLayerByID(int layerID) {
            _currentLayerDictionary.Remove(layerID);
            _layerPicker.DeleteLayerButton(layerID);
        }

        public void SetTile(Vector3 worldPosition, int tileIndex) {
            if (_currentLayerDictionary.Count == 0) return;
            _currentLayerDictionary[SelectedLayerID].SetTileIndex(worldPosition, tileIndex);
        }

        public Layer GetSelectedLayer() {
            return GetLayerByIndex(SelectedLayerID);
        }

        public Layer GetLayerByIndex(int index) {
             for (int i = 0; i < _currentLayerDictionary.Count; ++i) {
                if (_currentLayerDictionary[i].LayerID == index) {
                    return _currentLayerDictionary[i];
                }
            }
            return null;
        }

        public void AddButtonOnClick(Button button, Layer layer, System.Action callback) {
            button.onClick.AddListener(() => { 
                OnButtonClick(button, layer, callback);
            });
        }

        void OnButtonClick(Button button, Layer layer, System.Action callback) {
            if (_selectedButtonImage != null) {
                _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
                _selectedButtonImage.GetComponentInChildren<Text>().color = Color.white;
            }
            _selectedButtonImage = button.image;

            button.image.color = new Color(1f, 0.7626624f, 0.2122642f);
            _selectedButtonImage.GetComponentInChildren<Text>().color = Color.black;

            callback();
        }
    }
}
