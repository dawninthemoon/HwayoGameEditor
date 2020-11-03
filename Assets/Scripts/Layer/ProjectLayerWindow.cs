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
        Image _selectedButtonImage;
        int _numOfLayers;

        public override void Initalize() {
            base.Initalize();
            _tileLayerWindow = GetComponentInChildren<TileLayerWindow>(true);
            _entityLayerWindow = GetComponentInChildren<EntityLayerWindow>(true);
            gameObject.SetActive(false);
        }

        public void CreateTileLayer() {
            if (_layerModel.IsLayerEmpty())
                _layerModel.SelectedLayerID = 0;

            var tilemapVisual = Instantiate(_tilemapVisualPrefab);
            tilemapVisual.Initalize(_tilesetModel.GetFirstMaterial());
            _tilesetModel.AddTilemapVisual(tilemapVisual);

            string name = DefaultTileLayerName + " " +_numOfLayers.ToString();
            var tilemapLayer = new TileLayer(name, _numOfLayers, 16, Vector3.zero);
            tilemapLayer.SetTilemapVisual(tilemapVisual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = name;
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
            if (_layerModel.IsLayerEmpty())
                _layerModel.SelectedLayerID = 0;

            string name = DefaultEntityLayerName + " " +_numOfLayers.ToString();
            var entityLayer = new EntityLayer(name, _numOfLayers, 16, Vector3.zero);
            //entityLayer.SetTilemapVisual(tilemapVisual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = name;
            _buttons.Add(_numOfLayers, button);

            ++_numOfLayers;
        }

        public void DeleteSelectedLayer() {
            if (_layerModel.SelectedLayerID == -1) return;

            int curID = _layerModel.SelectedLayerID;
            var button = _buttons[curID];
            
            button.transform.SetParent(null);
            button.gameObject.SetActive(false);
            DestroyImmediate(button);

            _layerModel.DeleteLayerByID(curID);
            _layerModel.SelectedLayerID = -1;
        }

        void HighlightImage(Image buttonImage) {
            if (_selectedButtonImage != null)
                _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
            buttonImage.color = new Color(1f, 0.7626624f, 0.2122642f);
        }

        public Button GetButtonByIndex(int index) {
            return _buttons[index];
        }
    }
}
