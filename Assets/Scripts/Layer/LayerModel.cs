﻿using System.Collections;
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
        static string LayerDictionaryKey = "Key_LayerModel_LayerDictionary";
        static string GridWidthKey = "Key_LayerMode_GridWidth";
        static string GridHeightKey = "Key_LayerMode_GridHeight";

        void Awake() {
            int defaultGridSize = PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey) ? PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey) : 16;
            CurrentGridWidth = ES3.Load(GridWidthKey + "_" + LevelModel.CurrentLevelID.ToString(), defaultGridSize);
            CurrentGridHeight = ES3.Load(GridHeightKey + "_" + LevelModel.CurrentLevelID.ToString(), defaultGridSize);

            _projectLayerWindow = GameObject.Find("Project Layers").GetComponent<ProjectLayerWindow>();
            LoadLayers();
        }

        public void LoadLayers() {
            string keyName = LayerDictionaryKey + "_" + LevelModel.CurrentLevelID.ToString();

            int defaultGridSize = PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey) ? PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey) : 16;
            CurrentGridWidth = ES3.Load(GridWidthKey + "_" + LevelModel.CurrentLevelID.ToString(), defaultGridSize);
            CurrentGridHeight = ES3.Load(GridHeightKey + "_" + LevelModel.CurrentLevelID.ToString(), defaultGridSize);

            if (_currentLayerDictionary != null) {
                foreach (var layer in _currentLayerDictionary.Values) {
                    if (layer is TileLayer) {
                        var visual = (layer as TileLayer).Visual;
                        DestroyImmediate(visual.gameObject);
                    }
                    else if (layer is EntityLayer) {
                        var visual = (layer as EntityLayer).Visual;
                        DestroyImmediate(visual.gameObject);
                    }
                    else if (layer is CollisionLayer) {
                        var visual = (layer as CollisionLayer).Visual;
                        DestroyImmediate(visual.gameObject);
                    }
                }
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
            ES3.Save(GridWidthKey + "_" + LevelModel.CurrentLevelID.ToString(), CurrentGridWidth);
            ES3.Save(GridHeightKey + "_" + LevelModel.CurrentLevelID.ToString(), CurrentGridHeight);

            if (widthDelta == 0 && heightDelta == 0) return;
            foreach (var layer in _currentLayerDictionary.Values) {
                layer.ResizeGrid(originPosition, widthDelta, heightDelta);
            }
            CurrentOriginPosition = originPosition;
            _onGridResized?.Invoke();

            SaveLayer();
        }

        public void SetOnGridResized(OnGridResized callback) {
            _onGridResized = callback;
        }

        public bool IsLayerEmpty() => (_currentLayerDictionary.Count == 0);
        public int NumOfLayers() => _currentLayerDictionary.Count;

        public void SaveLayer() {
            string keyName = LayerDictionaryKey + "_" + LevelModel.CurrentLevelID;
            ES3.Save(keyName, _currentLayerDictionary);
        }

        public void AddLayer(Layer layer) {
            _currentLayerDictionary.Add(layer.LayerID, layer);
            _layerPicker.AddLayerButton(this, layer);
            SaveLayer();
        }

        public void DeleteLayerByID(int layerID) {
            _currentLayerDictionary.Remove(layerID);
            _layerPicker.DeleteLayerButton(layerID);
            SaveLayer();
        }

        public void SetTile(Vector3 worldPosition, int tileIndex) {
            if (_currentLayerDictionary.Count == 0) return;
            _currentLayerDictionary[SelectedLayerID].SetTileIndex(worldPosition, tileIndex);
            SaveLayer();
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
