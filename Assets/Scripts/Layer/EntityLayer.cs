using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class EntityLayer : Layer {
        Dictionary<Vector2, Entity> _entityDictionary;
        EntityModel _entityModel;

        public EntityLayer(string layerName, int layerIndex, float cellSize, Vector3 originPosition)
        : base(layerName, layerIndex, cellSize, originPosition) { 
            _entityDictionary = new Dictionary<Vector2, Entity>();
        }

        public void SetEntityModel(EntityModel model) {
            _entityModel = model;
        }

        public override void SetTileIndex(Vector3 worldPosition, int tileIndex) {
            TileObject tilemapObject = _grid.GetGridObject(worldPosition);
            if (tilemapObject == null) return;
            
            int index = tilemapObject.GetTileIndex();
            tilemapObject.SetTileIndex(tileIndex);
            
            if (index == -1) {
                int x, y;
                _grid.GetXY(worldPosition, out x, out y);

                Vector2 xy = new Vector2(x, y);
                var entity = _entityModel.GetEntityByID(tileIndex);
                _entityDictionary.Add(xy, entity);
            }
        }
    }
}
