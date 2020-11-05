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
        public Dictionary<int, Layer> CurrentLayerDictionary { get { return _currentLayerDictionary; } }
        public int SelectedLayerID { get; set; } = -1;
        public static int CurrentGridWidth;
        public static int CurrentGridHeight;
        public static Vector3 CurrentOriginPosition;
        OnGridResized _onGridResized;
        public bool _check;
        static string LayerDictionaryKey = "Key_LayerModel_LayerDictionary";

        void Awake() {
            _currentLayerDictionary = ES3.Load(LayerDictionaryKey, new Dictionary<int, Layer>());
            foreach (var layer in _currentLayerDictionary.Values) {
                _layerPicker.AddLayerButton(this, layer);
            }
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
            ES3.Save(LayerDictionaryKey, _currentLayerDictionary);
        }

        public void SetOnGridResized(OnGridResized callback) {
            _onGridResized = callback;
        }

        public bool IsLayerEmpty() => (_currentLayerDictionary.Count == 0);
        public int NumOfLayers() => _currentLayerDictionary.Count;

        public void AddLayer(Layer layer) {
            _currentLayerDictionary.Add(layer.LayerID, layer);
            _layerPicker.AddLayerButton(this, layer);

            ES3.Save(LayerDictionaryKey, _currentLayerDictionary);
        }

        public void DeleteLayerByID(int layerID) {
            _currentLayerDictionary.Remove(layerID);
            _layerPicker.DeleteLayerButton(layerID);
            ES3.Save(LayerDictionaryKey, _currentLayerDictionary);
        }

        public void SetTile(Vector3 worldPosition, int tileIndex) {
            if (_currentLayerDictionary.Count == 0) return;
            _currentLayerDictionary[SelectedLayerID].SetTileIndex(worldPosition, tileIndex);
            ES3.Save(LayerDictionaryKey, _currentLayerDictionary);
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
    }
}
