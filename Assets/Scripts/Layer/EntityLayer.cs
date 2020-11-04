using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class EntityLayer : Layer {
        EntityModel _entityModel;
        Grid<EntityObject> _grid;
        EntityEditWindow _entityEditWindow;
        EntityVisual _entityVisual;
        public EntityVisual Visual {
            get { return _entityVisual; }
            set {
                _entityVisual = value;
                _entityVisual.SetGrid(this, _grid);
            }
        }

        public EntityLayer(string layerName, int layerIndex, float cellSize, Vector3 originPosition)
        : base(layerName, layerIndex, cellSize, originPosition) { 
            _grid = new Grid<EntityObject>(cellSize, originPosition, (Grid<EntityObject> g, int x, int y) => new EntityObject(g, x, y));
        }

        public void SetEntityModel(EntityModel model) {
            _entityModel = model;
        }
        public void SetEntityEditWindow(EntityEditWindow window) {
            _entityEditWindow = window;
        }

        public override void SetTileIndex(Vector3 worldPosition, int entityID) {
            if (_entityModel.SelectedIndex == -1) return;

            EntityObject entityObject = _grid.GetGridObject(worldPosition);
            if (entityObject == null) return;

            var selectedEntity = _entityModel.GetEntityByID(entityID);
            if (entityObject.GetIndex() == -1) {
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
        }

        public class EntityObject : IGridObject {
            Grid<EntityObject> _grid;
            int _x;
            int _y;
            int _textureIndex;
            public string EntityName { get; set; }
            public int EntityID { get; set; }
            Dictionary<string, string> _fields = new Dictionary<string, string>();
            public Dictionary<string, string> Fields { get { return _fields; } }

            public EntityObject(Grid<EntityObject> grid, int x, int y) {
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

            public void SetFieldNames(List<string> fieldNames) {
                _fields.Clear();
                for (int i = 0; i < fieldNames.Count; ++i) {
                    _fields.Add(fieldNames[i], null);
                }
            }
        }
    }
}
