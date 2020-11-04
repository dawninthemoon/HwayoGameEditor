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
        [SerializeField] TilesetVisual _tilesetVisualPrefab = null;
        [SerializeField] EntityVisual _entityVisualPrefab = null;
        [SerializeField] EntityModel _entityModel = null;
        [SerializeField] EntityEditWindow _entityEditWindow = null;
        TileLayerWindow _tileLayerWindow;
        EntityLayerWindow _entityLayerWindow;
        Dictionary<int, Button> _buttons = new Dictionary<int, Button>();
        static readonly string DefaultTileLayerName = "Tile Layer";
        static readonly string DefaultEntityLayerName = "Entity Layer";
        int _selectedLayerIndex;
        public int SelectedLayerIDInWindow { get { return _selectedLayerIndex;} }
        int _numOfLayers;

        public override void Initalize() {
            base.Initalize();
            _tileLayerWindow = GetComponentInChildren<TileLayerWindow>(true);
            _entityLayerWindow = GetComponentInChildren<EntityLayerWindow>(true);
            gameObject.SetActive(false);

            CreateEntityLayer();
            (_layerModel.GetLayerByIndex(0) as EntityLayer).SetEntityModel(_entityModel);
        }

        public void CreateTileLayer() {
            if (_layerModel.IsLayerEmpty())
                _selectedLayerIndex = 0;

            var tilesetVisual = Instantiate(_tilesetVisualPrefab);
            tilesetVisual.Initalize(_tilesetModel.GetFirstMaterial());
            _tilesetModel.AddTilesetVisual(tilesetVisual);

            string name = DefaultTileLayerName + " " +_numOfLayers.ToString();
            var tilemapLayer = new TileLayer(name, _numOfLayers, 16, Vector3.zero);
            tilemapLayer.Visual = tilesetVisual;

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = name;
            _buttons.Add(_numOfLayers, button);

            int index = _numOfLayers;
            System.Action callback = () => { OnTileLayerButtonClick(tilemapLayer); };
            button.onClick.AddListener(() => { _selectedLayerIndex = index; });
            _layerModel.AddButtonOnClick(button, tilemapLayer, callback);
            _layerModel.AddLayer(tilemapLayer, button, callback);
            
            ++_numOfLayers;
        }

        void OnTileLayerButtonClick(TileLayer tilemapLayer) {
            _entityLayerWindow.gameObject.SetActive(false);
            _tileLayerWindow.gameObject.SetActive(true);
            _tileLayerWindow.SetDropdownValueWithIgnoreCallback(tilemapLayer);
            _tileLayerWindow.SetInputFieldTextWithIgnoreCallback(tilemapLayer);
        }

        public void CreateEntityLayer() {
            if (_layerModel.IsLayerEmpty())
                _selectedLayerIndex = 0;

            var entityVisual = Instantiate(_entityVisualPrefab);
            entityVisual.Initalize(_tilesetModel.EntityVisualMaterial);

            string name = DefaultEntityLayerName + " " +_numOfLayers.ToString();
            var entityLayer = new EntityLayer(name, _numOfLayers, 16, Vector3.zero);
            entityLayer.SetEntityEditWindow(_entityEditWindow);
            entityLayer.Visual = entityVisual;

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = name;
            _buttons.Add(_numOfLayers, button);

            int index = _numOfLayers;
            System.Action callback = () => { OnEntityButtonClick(entityLayer); _layerModel.SelectedLayerID = index; };
            button.onClick.AddListener(() => { _selectedLayerIndex = index; });
            _layerModel.AddButtonOnClick(button, entityLayer, callback);
            _layerModel.AddLayer(entityLayer, button, callback);

            ++_numOfLayers;
        }

        void OnEntityButtonClick(EntityLayer entityLayer) {
            _entityLayerWindow.gameObject.SetActive(false);
            _tileLayerWindow.gameObject.SetActive(true);
            _entityLayerWindow.SetInputFieldTextWithIgnoreCallback(entityLayer);
        }

        public void DeleteSelectedLayer() {
            if (_selectedLayerIndex == -1 || _selectedLayerIndex == 0) return;

            int curID = _selectedLayerIndex;
            var button = _buttons[curID];
            _buttons.Remove(curID);
            
            button.transform.SetParent(null);
            button.gameObject.SetActive(false);
            DestroyImmediate(button);

            _layerModel.DeleteLayerByID(curID);
            _selectedLayerIndex = -1;
        }

        public Button GetButtonByIndex(int index) {
            return _buttons[index];
        }
    }
}
