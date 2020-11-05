using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class CollisionVisual : MonoBehaviour {
        List<Vector3> _points = new List<Vector3>();
        public int CellSize { get; set; }
        Vector3 _mousePosition;
        public int LayerID { get; set; }
        public LayerModel _layerModel;

        LineRenderer _lineRenderer;

        void Start() {
            GameObject obj = new GameObject("LineRenderer");
            _lineRenderer = obj.AddComponent<LineRenderer>();
            _lineRenderer.material = Aroma.LineUtility.GetInstance().GetMaterial();
            _lineRenderer.startWidth = _lineRenderer.endWidth = 0.5f;
            _lineRenderer.startColor = _lineRenderer.endColor = Color.green;
        }

        public void SetLayerModel(LayerModel layerModel) {
            _layerModel = layerModel;
        }
        
        public bool IsPointsEmpty() => (_points.Count == 0);

        public void AddPoint(Vector3 point) {
            _points.Add(point);
            _lineRenderer.positionCount = _points.Count + 1;
            _lineRenderer.SetPosition(_points.Count - 1, point);
        }

        void Update() {
            if (_layerModel.SelectedLayerID != LayerID) return;
            _mousePosition = Utility.GetMouseWorldPosition();
            if (Input.GetKey(KeyCode.LeftShift)) {
                int x, y;
                GridUtility.GetXY(_mousePosition, out x, out y, CellSize, LayerModel.CurrentOriginPosition);
                _mousePosition = GridUtility.GetWorldPosition(x, y, CellSize, LayerModel.CurrentOriginPosition);
            }

            if (!IsPointsEmpty()) {
                _lineRenderer.SetPosition(_points.Count, _mousePosition);
            }
        }
    }
}
