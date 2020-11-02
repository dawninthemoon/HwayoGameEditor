using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTilemap {
    public class ProjectLayerWindow : SlideableUI {
        [SerializeField] Transform _contentTransform = null;
        [SerializeField] Button _layerButtonPrefab = null;
        [SerializeField] TilesetModel _tilesetModel = null;
        [SerializeField] LayerModel _layerModel = null;
        [SerializeField] TilemapVisual _tilemapVisualPrefab = null;
        TileLayerWindow _tileLayerWindow;
        List<Button> _buttons = new List<Button>();
        static readonly string DefaultLayerName = "Default Layer";
        int _numOfLayers;

        public override void Initalize() {
            base.Initalize();
            _tileLayerWindow = GetComponentInChildren<TileLayerWindow>();
            gameObject.SetActive(false);
        }

        public void CreateTileLayer() {
            var tilemapVisual = Instantiate(_tilemapVisualPrefab);
            tilemapVisual.Initalize(_tilesetModel.GetFirstMaterial());
            _tilesetModel.AddTilemapVisual(tilemapVisual);

            var tilemapLayer = new Layer(DefaultLayerName + " " +_numOfLayers.ToString(), LayerType.Tile, _numOfLayers, 16, 16, 16, Vector3.zero);
            tilemapLayer.SetTilemapVisual(tilemapVisual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = DefaultLayerName + _numOfLayers.ToString();
            _buttons.Add(button);

            System.Action callback = () => {
                _tileLayerWindow.SetDropdownValueWithIgnoreCallback(tilemapLayer);
                _tileLayerWindow.SetInputFieldTextWithIgnoreCallback(tilemapLayer);
            };
            _layerModel.AddButtonOnClick(button, _numOfLayers, tilemapVisual, callback);
            _layerModel.AddLayer(tilemapLayer, button, tilemapVisual, callback);
            
            ++_numOfLayers;
        }

        public void CreateEntityLayer() {
            
        }

        public Button GetButtonByIndex(int index) {
            if (index >= _buttons.Count) return null;
            return _buttons[index];
        }
    }
}
