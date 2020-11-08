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
        ProjectLayerWindow _projectLayerWindow;
        public bool _check;
        public static string LayerDictionaryKey = "Key_LayerModel_LayerDictionary";
        public static string GridWidthKey = "Key_LayerMode_GridWidth";
        public static string GridHeightKey = "Key_LayerMode_GridHeight";
        public static string OriginPositionKey = "Key_GridPosition_OriginPosition";
        List<string>[] _deletedKeyList =new List<string>[3] { new List<string>(), new List<string>(), new List<string>() };

        void Awake() {
            _projectLayerWindow = GameObject.Find("Project Layers").GetComponent<ProjectLayerWindow>();
            LoadLayers();
        }

        public void LoadLayers() {
            string keyName = LayerDictionaryKey + "_" + LevelModel.CurrentLevelID.ToString();

            int defaultGridSize = PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey) ? PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey) : 16;
            CurrentGridWidth = ES3.Load(GridWidthKey + "_" + LevelModel.CurrentLevelID.ToString(), defaultGridSize);
            CurrentGridHeight = ES3.Load(GridHeightKey + "_" + LevelModel.CurrentLevelID.ToString(), defaultGridSize);
            CurrentOriginPosition = ES3.Load(OriginPositionKey + "_" + LevelModel.CurrentLevelID.ToString(), Vector3.zero);
            if (_currentLayerDictionary != null) {
                RemoveAllVisuals();
            }

            _currentLayerDictionary = ES3.Load(keyName, new Dictionary<int, Layer>());
            _layerPicker.DeleteAllLayerButtons();
            foreach (var layer in _currentLayerDictionary.Values) {
                _layerPicker.AddLayerButton(this, layer);
            }
            _projectLayerWindow.LoadLayers();
            _onGridResized?.Invoke();
        }

        public void ResizeAllLayers(Vector3 originPosition, int widthDelta, int heightDelta) {
            CurrentGridWidth += widthDelta;
            CurrentGridHeight += heightDelta;

            if (widthDelta == 0 && heightDelta == 0) return;
            foreach (var layer in _currentLayerDictionary.Values) {
                layer.ResizeGrid(originPosition, widthDelta, heightDelta);
            }
            CurrentOriginPosition = originPosition;
            _onGridResized?.Invoke();

            string keyName = LayerDictionaryKey + "_" + LevelModel.CurrentLevelID.ToString();
            ES3.Save(keyName, _currentLayerDictionary);
        }

        public void SetOnGridResized(OnGridResized callback) {
            _onGridResized = callback;
        }

        public bool IsLayerEmpty() => (_currentLayerDictionary.Count == 0);
        public int NumOfLayers() => _currentLayerDictionary.Count;

        public void SaveAllLayers(int id) {
            ES3.Save(GridWidthKey + "_" + id.ToString(), CurrentGridWidth);
            ES3.Save(GridHeightKey + "_" + id.ToString(), CurrentGridHeight);
            ES3.Save(OriginPositionKey + "_" + id.ToString(), CurrentOriginPosition);

            string keyName = LayerDictionaryKey + "_" + id.ToString();
            ES3.Save(keyName, _currentLayerDictionary);

            for (int i = 0; i < _deletedKeyList.Length; ++i) {
                foreach (var key in _deletedKeyList[i]) {
                    ES3.DeleteKey(key);
                }
                _deletedKeyList[i].Clear();
            }
        }

        void RemoveAllVisuals() {
            foreach (var layer in _currentLayerDictionary.Values) {
                if (layer is TileLayer) {
                    var visual = (layer as TileLayer).Visual;
                    if (visual != null)
                        DestroyImmediate(visual.gameObject);
                }
                else if (layer is EntityLayer) {
                    var visual = (layer as EntityLayer).Visual;
                    if (visual != null)
                        DestroyImmediate(visual.gameObject);
                }
                else if (layer is CollisionLayer) {
                    var visual = (layer as CollisionLayer).Visual;
                    if (visual != null)
                        DestroyImmediate(visual.gameObject);
                }
            }
        }

        public void DeleteLayerSaves() {
            RemoveAllVisuals();
            _deletedKeyList[0].Add(GridWidthKey + "_" + LevelModel.CurrentLevelID.ToString());
            _deletedKeyList[1].Add(GridHeightKey + "_" + LevelModel.CurrentLevelID.ToString());
            _deletedKeyList[2].Add(LayerDictionaryKey + "_" + LevelModel.CurrentLevelID);
        }

        public void AddLayer(Layer layer) {
            _currentLayerDictionary.Add(layer.LayerID, layer);
            _layerPicker.AddLayerButton(this, layer);
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
    }
}
