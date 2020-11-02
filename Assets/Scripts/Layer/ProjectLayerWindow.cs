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
        EntityLayerWindow _entityLayerWindow;
        Dictionary<int, Button> _buttons = new Dictionary<int, Button>();
        static readonly string DefaultTileLayerName = "Tile Layer";
        static readonly string DefaultEntityLayerName = "Entity Layer";
        int _numOfLayers;

        public override void Initalize() {
            base.Initalize();
            _tileLayerWindow = GetComponentInChildren<TileLayerWindow>(true);
            _entityLayerWindow = GetComponentInChildren<EntityLayerWindow>(true);
            gameObject.SetActive(false);
        }

        public void CreateTileLayer() {
            var tilemapVisual = Instantiate(_tilemapVisualPrefab);
            tilemapVisual.Initalize(_tilesetModel.GetFirstMaterial());
            _tilesetModel.AddTilemapVisual(tilemapVisual);

            var tilemapLayer = new TileLayer(DefaultTileLayerName + " " +_numOfLayers.ToString(), _numOfLayers, 16, 16, 16, Vector3.zero);
            tilemapLayer.SetTilemapVisual(tilemapVisual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = DefaultTileLayerName + _numOfLayers.ToString();
            _buttons.Add(_numOfLayers, button);

            System.Action callback = () => {
                _entityLayerWindow.gameObject.SetActive(false);
                _tileLayerWindow.gameObject.SetActive(true);
                _tileLayerWindow.SetDropdownValueWithIgnoreCallback(tilemapLayer);
                _tileLayerWindow.SetInputFieldTextWithIgnoreCallback(tilemapLayer);
            };
            _layerModel.AddButtonOnClick(button, _numOfLayers, tilemapVisual, callback);
            _layerModel.AddLayer(tilemapLayer, button, tilemapVisual, callback);
            
            ++_numOfLayers;
        }

        public void CreateEntityLayer() {
            var tilemapLayer = new EntityLayer(DefaultEntityLayerName + " " +_numOfLayers.ToString(), _numOfLayers, 16, 16, 16, Vector3.zero);
            //tilemapLayer.SetTilemapVisual(tilemapVisual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = DefaultEntityLayerName + _numOfLayers.ToString();
            _buttons.Add(_numOfLayers, button);

            ++_numOfLayers;
        }

        public Button GetButtonByIndex(int index) {
            return _buttons[index];
        }
    }
}
