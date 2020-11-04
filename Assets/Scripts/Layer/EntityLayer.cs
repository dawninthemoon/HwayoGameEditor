using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class EntityLayer : Layer {
        Dictionary<Vector2, Entity> _entityDictionary;
        EntityModel _entityModel;
        Grid<EntityObject> _grid;
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
            _entityDictionary = new Dictionary<Vector2, Entity>();
            _grid = new Grid<EntityObject>(cellSize, originPosition, (Grid<EntityObject> g, int x, int y) => new EntityObject(g, x, y));
        }

        public void SetEntityModel(EntityModel model) {
            _entityModel = model;
        }

        public override void SetTileIndex(Vector3 worldPosition, int entityID) {
            if (_entityModel.SelectedIndex == -1) return;

            EntityObject entityObject = _grid.GetGridObject(worldPosition);
            if (entityObject == null) return;

            var selectedEntity = _entityModel.GetEntityByID(entityID);
            
            int index = entityObject.GetIndex();
            entityObject?.SetIndex(selectedEntity.TextureIndex);

            int x, y;
            _grid.GetXY(worldPosition, out x, out y);
            Vector2 xy = new Vector2(x, y);
            if (index == -1) {
                AddEntityToDictionary();
            }
            else {
                _entityDictionary.Remove(xy);
                AddEntityToDictionary();
            }
            void AddEntityToDictionary() {
                if (entityID == -1) return;
                var entity = _entityModel.GetEntityByID(entityID);
                _entityDictionary.Add(xy, entity);
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
            List<KeyValuePair<string, string>> _fields;
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
        }
    }
}
