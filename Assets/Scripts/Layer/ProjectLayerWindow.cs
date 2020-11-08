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
        [SerializeField] CollisionVisual _collisionVisualPrefab = null;
        [SerializeField] EntityModel _entityModel = null;
        [SerializeField] EntityEditWindow _entityEditWindow = null;
        TileLayerWindow _tileLayerWindow;
        EntityLayerWindow _entityLayerWindow;
        CollisionLayerWindow _collisionLayerWindow;
        Image _selectedButtonImage;
        Dictionary<int, Button> _buttons = new Dictionary<int, Button>();
        static readonly string DefaultTileLayerName = "Tile Layer";
        static readonly string DefaultEntityLayerName = "Entity Layer";
        static readonly string DefaultCollisionLayerName = "Collision Layer";
        int _selectedLayerIndex;
        public int SelectedLayerIDInWindow { get { return _selectedLayerIndex;} }
        int _numOfLayers;

        public override void Initalize() {
            base.Initalize();
            _tileLayerWindow = GetComponentInChildren<TileLayerWindow>(true);
            _entityLayerWindow = GetComponentInChildren<EntityLayerWindow>(true);
            _collisionLayerWindow = GetComponentInChildren<CollisionLayerWindow>(true);
            gameObject.SetActive(false);
        }

        public void LoadLayers() {
            foreach (var button in _buttons.Values) {
                button.transform.SetParent(null);
                DestroyImmediate(button);
            }
            _buttons.Clear();

            int max = -1;
            foreach (var layer in _layerModel.CurrentLayerDictionary.Values) {
                if (layer is TileLayer) {
                    TileLayer tilemapLayer = layer as TileLayer;
                    tilemapLayer.LoadGridArray();
                    CreateButtonByTileLayer(tilemapLayer);
                }
                else if (layer is EntityLayer) {
                    EntityLayer entityLayer = layer as EntityLayer;
                    entityLayer.LoadGridArray();
                    CreateButtonByEntityLayer(entityLayer);
                    entityLayer.SetEntityModel(_entityModel);
                }
                else if (layer is CollisionLayer) {
                    CreateButtonByCollisionLayer(layer as CollisionLayer);
                }
                max = Mathf.Max(layer.LayerID, max);
            }
            _numOfLayers = max + 1;

            if (_layerModel.CurrentLayerDictionary.Count == 0)
                CreateEntityLayer();
        }

        public void CreateTileLayer() {
            if (_layerModel.IsLayerEmpty())
                _selectedLayerIndex = 0;

            string name = DefaultTileLayerName;
            var tilemapLayer = new TileLayer(name, _numOfLayers, 16);
            
            CreateButtonByTileLayer(tilemapLayer);
            _layerModel.AddLayer(tilemapLayer);

            ++_numOfLayers;
        }
        void CreateButtonByTileLayer(TileLayer tilemapLayer) {
            var tilesetVisual = Instantiate(_tilesetVisualPrefab);

            string name = tilemapLayer.TilesetName;
            Material material = (name != null) ? _tilesetModel.GetMaterialByName(name) : _tilesetModel.GetFirstMaterial();
            tilesetVisual.Initalize(material);
            _tilesetModel.AddTilesetVisual(tilesetVisual);
            tilemapLayer.Visual = tilesetVisual;

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = tilemapLayer.LayerName;
            _buttons.Add(tilemapLayer.LayerID, button);

            int index = tilemapLayer.LayerID;
            System.Action callback = () => { OnTileLayerButtonClick(tilemapLayer); };
            button.onClick.AddListener(() => { 
                _selectedLayerIndex = index; 
                OnLayerButtonClick(button, tilemapLayer, callback);
            });
        }
        void OnTileLayerButtonClick(TileLayer tilemapLayer) {
            _entityLayerWindow.gameObject.SetActive(false);
            _tileLayerWindow.gameObject.SetActive(true);
            _collisionLayerWindow.gameObject.SetActive(false);
            _tileLayerWindow.SetDropdownValueWithIgnoreCallback(tilemapLayer);
            _tileLayerWindow.SetInputFieldTextWithIgnoreCallback(tilemapLayer);
        }

        public void CreateEntityLayer() {
            if (_layerModel.IsLayerEmpty())
                _selectedLayerIndex = 0;

            string name = DefaultEntityLayerName;
            var entityLayer = new EntityLayer(name, _numOfLayers, 16);
            entityLayer.SetEntityModel(_entityModel);

            CreateButtonByEntityLayer(entityLayer);
            _layerModel.AddLayer(entityLayer);

            ++_numOfLayers;
        }

        void CreateButtonByEntityLayer(EntityLayer entityLayer) {
            var entityVisual = Instantiate(_entityVisualPrefab);
            entityVisual.Initalize(_tilesetModel.EntityVisualMaterial);

            entityLayer.SetEntityEditWindow(_entityEditWindow);
            entityLayer.Visual = entityVisual;

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = entityLayer.LayerName;
            _buttons.Add(entityLayer.LayerID, button);

            System.Action callback = () => { _layerModel.SelectedLayerID = entityLayer.LayerID; OnEntityButtonClick(entityLayer); };
            button.onClick.AddListener(() => { 
                _selectedLayerIndex = entityLayer.LayerID;
                OnLayerButtonClick(button, entityLayer, callback);
            });
        }

        void OnEntityButtonClick(EntityLayer entityLayer) {
            _entityLayerWindow.gameObject.SetActive(true);
            _tileLayerWindow.gameObject.SetActive(false);
            _collisionLayerWindow.gameObject.SetActive(false);
            _entityLayerWindow.SetInputFieldTextWithIgnoreCallback(entityLayer);
        }

        public void CreateCollisionLayer() {
            if (_layerModel.IsLayerEmpty())
                _selectedLayerIndex = 0;

            string name = DefaultCollisionLayerName;
            var collisionLayer = new CollisionLayer(name, _numOfLayers, 16, null);

            CreateButtonByCollisionLayer(collisionLayer);
            _layerModel.AddLayer(collisionLayer);

            ++_numOfLayers;
        }

        void CreateButtonByCollisionLayer(CollisionLayer collisionLayer) {
            CollisionVisual visual = Instantiate(_collisionVisualPrefab);
            visual.SetLayerModel(_layerModel);
            visual.LayerID = collisionLayer.LayerID;
            collisionLayer.SetCollisionVisual(visual);

            var button = Instantiate(_layerButtonPrefab, _contentTransform);
            button.GetComponentInChildren<Text>().text = collisionLayer.LayerName;
            _buttons.Add(collisionLayer.LayerID, button);

            System.Action callback = () => { 
                _layerModel.SelectedLayerID = collisionLayer.LayerID;
                OnCollisionButtonClick(collisionLayer);
            };
            button.onClick.AddListener(() => { 
                _selectedLayerIndex = collisionLayer.LayerID;
                OnLayerButtonClick(button, collisionLayer, callback);
            });
        }

        void OnCollisionButtonClick(CollisionLayer collisionLayer) {
             _entityLayerWindow.gameObject.SetActive(false);
            _tileLayerWindow.gameObject.SetActive(false);
            _collisionLayerWindow.gameObject.SetActive(true);
            _collisionLayerWindow.SetInputFieldTextWithIgnoreCallback(collisionLayer);
            _collisionLayerWindow.SetInputFieldTagWithoutCallback(collisionLayer);
        }

        void OnLayerButtonClick(Button button, Layer layer, System.Action callback) {
            if (_selectedButtonImage != null) {
                _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
                _selectedButtonImage.GetComponentInChildren<Text>().color = Color.white;
            }
            _selectedButtonImage = button.image;

            button.image.color = new Color(1f, 0.7626624f, 0.2122642f);
            _selectedButtonImage.GetComponentInChildren<Text>().color = Color.black;

            callback();
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
