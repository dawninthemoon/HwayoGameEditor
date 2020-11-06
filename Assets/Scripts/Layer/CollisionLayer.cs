using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class CollisionLayer : Layer {
        float _cellSize;
        List<Vector2> _points;
        CollisionVisual _visual;
        public CollisionVisual Visual { get { return _visual; } }
        public CollisionLayer() {
            _points = new List<Vector2>();
        }
        public CollisionLayer(string layerName, int layerIndex, float cellSize) 
        : base(layerName, layerIndex) {
            _cellSize = cellSize;
            _points = new List<Vector2>();
        }

        public void SetCollisionVisual(CollisionVisual visual) {
            _visual = visual;
            _visual.SetPointsList(_points);
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
                AddPoint(worldPosition);
            }
        }

        void AddPoint(Vector2 point) {
            if (_visual.Complete) return;
            _points.Add(point);
        }

        public override void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) { }
    }
}
