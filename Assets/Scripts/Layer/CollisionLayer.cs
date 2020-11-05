using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class CollisionLayer : Layer {
        float _cellSize;
        CollisionVisual _visual;

        public CollisionLayer(string layerName, int layerIndex, float cellSize) 
        : base(layerName, layerIndex) {
            _cellSize = cellSize;
        }

        public void SetCollisionVisual(CollisionVisual visual) {
            _visual = visual;
        }

        public override void SetTileIndex(Vector3 worldPosition, int textureIndex) {
            worldPosition.z = 0f;
            worldPosition = GridUtility.ClampPosition(worldPosition, _cellSize);

            if (Input.GetKey(KeyCode.LeftShift)) {
                int x, y;
                GridUtility.GetXY(worldPosition, out x, out y, _cellSize, LayerModel.CurrentOriginPosition);
                worldPosition = GridUtility.GetWorldPosition(x, y, _cellSize, LayerModel.CurrentOriginPosition);
            }
            if (Input.GetMouseButtonDown(0)) {
                _visual.AddPoint(worldPosition);
            }
        }

        public override void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) { }
    }
}
