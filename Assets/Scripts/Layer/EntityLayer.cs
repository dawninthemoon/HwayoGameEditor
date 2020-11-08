using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class EntityLayer : Layer {
        EntityModel _entityModel;
        CustomGrid<EntityObject> _grid;
        EntityEditWindow _entityEditWindow;
        EntityVisual _entityVisual;
        public EntityVisual Visual {
            get { return _entityVisual; }
            set {
                _entityVisual = value;
                _entityVisual.SetGrid(this, _grid);
            }
        }
        EntityObject[,] _gridArray;
        public EntityLayer() {
            _grid = new CustomGrid<EntityObject>(16, (CustomGrid<EntityObject> g, int x, int y) => new EntityObject(g, x, y));
        }
        public EntityLayer(string layerName, int layerIndex, float cellSize)
        : base(layerName, layerIndex) { 
            _grid = new CustomGrid<EntityObject>(cellSize, (CustomGrid<EntityObject> g, int x, int y) => new EntityObject(g, x, y));
            _gridArray = _grid.GridArray;
        }

        public void SetEntityModel(EntityModel model) {
            _entityModel = model;
        }
        public void SetEntityEditWindow(EntityEditWindow window) {
            _entityEditWindow = window;
        }
        public void LoadGridArray() {
            _grid.GridArray = _gridArray;
        }

        public override void SetTileIndex(Vector3 worldPosition, int entityID) {
            if (_entityModel.SelectedIndex == -1 || (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))) return;

            EntityObject entityObject = _grid.GetGridObject(worldPosition);
            if (entityObject == null) return;

            if ((entityObject.GetIndex() == -1) && (entityID != -1)) {
                var selectedEntity = _entityModel.GetEntityByID(entityID);
                if (selectedEntity == null) return;
                entityObject.EntityID = entityID;
                entityObject.EntityName = selectedEntity.EntityName;
                entityObject.SetIndex(selectedEntity.TextureIndex);
                entityObject.SetFieldNames(selectedEntity.Fields);
            }
            else {
                if (entityID == -1) {
                    entityObject.EntityID = -1;
                    entityObject.SetIndex(-1);
                }
                else {
                    _entityEditWindow.EnableWindow(entityObject);
                }
            }
        }

        public override void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            _grid.ResizeGrid(originPosition, widthDelta, heightDelta);
            _gridArray = _grid.GridArray;
        }

        public class EntityObject : IGridObject {
            CustomGrid<EntityObject> _grid;
            int _x;
            int _y;
            int _textureIndex;
            public string EntityName { get; set; }
            public int EntityID { get; set; }
            Dictionary<string, string> _fields = new Dictionary<string, string>();
            public Dictionary<string, string> Fields { get { return _fields; } }
            public EntityObject() { }

            public EntityObject(CustomGrid<EntityObject> grid, int x, int y) {
                _grid = grid;
                _x = x;
                _y = y;
                _textureIndex = -1;
            }
            public int GetIndex() {
                return _textureIndex;
            }
            public void SetIndex(int index) {
                _textureIndex = index;
                _grid.TriggerGridObjectChanged(_x, _y);
            }
            public void SetGrid(object grid) {
                _grid = grid as CustomGrid<EntityObject>;
            }

            public void SetFieldNames(List<string> fieldNames) {
                _fields.Clear();
                for (int i = 0; i < fieldNames.Count; ++i) {
                    _fields.Add(fieldNames[i], null);
                }
            }
        }
    }
}
