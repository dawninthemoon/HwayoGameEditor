using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class CollisionVisual : MonoBehaviour {
        List<Vector2> _points = new List<Vector2>();
        int _cellSize = 16;
        public int LayerID { get; set; }
        public LayerModel _layerModel;
        LineRenderer _lineRenderer;
        float _rectSize;
        bool _complete;

        void Start() {
            _rectSize = _cellSize / 4f;
            _lineRenderer = CreateLineRenderer();
        }

        LineRenderer CreateLineRenderer() {
            GameObject obj = new GameObject("LineRenderer");
            var lr = obj.AddComponent<LineRenderer>();
            lr.material = Aroma.LineUtility.GetInstance().GetMaterial();
            lr.startColor = lr.endColor = Color.green;
            lr.sortingLayerName = "CollisionLine";
            return lr;
        }

        public void SetLayerModel(LayerModel layerModel) {
            _layerModel = layerModel;
        }
        
        public bool IsPointsEmpty() => (_points.Count == 0);

        public void AddPoint(Vector3 point) {
            if (_complete) return;
            _points.Add(point);
            _lineRenderer.positionCount = _points.Count + 1;
            _lineRenderer.SetPosition(_points.Count - 1, point);
        }

        void CompleteCollider() {
            _points.Add(_points[0]);
            _lineRenderer.SetPosition(_points.Count - 1, _points[0]);
            _complete = true;
        }

        void Update() {
            if (Input.GetMouseButtonDown(1) && (_points.Count > 0)) {
                _points.RemoveAt(_points.Count - 1);
                _lineRenderer.positionCount = _points.Count + 1;
                _complete = false;
            }

            if (_complete) return;
            if (_layerModel.SelectedLayerID != LayerID) {
                if (_lineRenderer.positionCount == _points.Count + 1)
                    CompleteCollider();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space) && (_points.Count > 0)) {
                CompleteCollider();
                return;
            }

            Vector2 mousePosition = Utility.GetMouseWorldPosition();
            mousePosition = GridUtility.ClampPosition(mousePosition, _cellSize);

            if (Input.GetKey(KeyCode.LeftShift)) {
                int x, y;
                GridUtility.GetXY(mousePosition, out x, out y, _cellSize, LayerModel.CurrentOriginPosition);
                mousePosition = GridUtility.GetWorldPosition(x, y, _cellSize, LayerModel.CurrentOriginPosition);
            }

            if (!IsPointsEmpty()) {
                _lineRenderer.SetPosition(_points.Count, mousePosition);
            }
        }

        void OnDrawGizmos() {
            if (_layerModel.SelectedLayerID != LayerID) return;

            Vector2 mousePosition = Utility.GetMouseWorldPosition();
            mousePosition = GridUtility.ClampPosition(mousePosition, _cellSize);
            if (Input.GetKey(KeyCode.LeftShift)) {
                int x, y;
                GridUtility.GetXY(mousePosition, out x, out y, _cellSize, LayerModel.CurrentOriginPosition);
                mousePosition = GridUtility.GetWorldPosition(x, y, _cellSize, LayerModel.CurrentOriginPosition);
            }
            Gizmos.color = Color.green;
            Gizmos.DrawCube(mousePosition, Vector3.one * _rectSize);
        }
    }
}
