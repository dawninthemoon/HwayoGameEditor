using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Aroma;

namespace CustomTilemap {
    public class CollisionVisual : MonoBehaviour {
        List<Vector2> _points;
        int _cellSize = 16;
        public int LayerID { get; set; }
        public LayerModel _layerModel;
        LineRenderer _lineRenderer;
        Transform _collisionRect;
        float _rectSize;
        public bool Complete { get; private set; }

        void Awake() {
            _rectSize = _cellSize / 4f;
            _lineRenderer = CreateLineRenderer();
            _collisionRect = transform.GetChild(0);
        }
        public void SetPointsList(List<Vector2> list) {
            _points = list;
            if (_points.Count > 0) Complete = true;
        }

        LineRenderer CreateLineRenderer() {
            GameObject obj = new GameObject("LineRenderer");
            var lr = obj.AddComponent<LineRenderer>();
            lr.transform.SetParent(transform);
            lr.material = Aroma.LineUtility.GetInstance().GetMaterial();
            lr.startColor = lr.endColor = Color.green;
            lr.sortingLayerName = "CollisionLine";
            return lr;
        }

        public void SetLayerModel(LayerModel layerModel) {
            _layerModel = layerModel;
        }

        void CompleteCollider() {
            _points.Add(_points[0]);
            Complete = true;
        }

        void Update() {
            DrawCollisionRect();

            if (_points == null) return;
            _lineRenderer.positionCount = _points.Count;
            for (int i = 0; i < _points.Count; ++i) {
                _lineRenderer.SetPosition(i, _points[i]);
            }

            if (Input.GetMouseButtonDown(1) && (_points.Count > 0)) {
                _points.RemoveAt(_points.Count - 1);
                Complete = false;
            }

            if (_layerModel.SelectedLayerID != LayerID) {
                if (_points.Count <= 2) {
                    _points.Clear();
                }
                else if (!Complete)
                    CompleteCollider();
                return;
            }

            if (Complete) return;

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

            if (_points.Count > 0) {
                ++_lineRenderer.positionCount;
                _lineRenderer.SetPosition(_points.Count, mousePosition);
            }
        }

        void DrawCollisionRect() {
            if (_layerModel.SelectedLayerID != LayerID || 
                EventSystem.current.IsPointerOverGameObject() ||
                Complete) {
                _collisionRect.gameObject.SetActive(false);
                return;
            }

            Vector2 mousePosition = Utility.GetMouseWorldPosition();
            mousePosition = GridUtility.ClampPosition(mousePosition, _cellSize);
            if (Input.GetKey(KeyCode.LeftShift)) {
                int x, y;
                GridUtility.GetXY(mousePosition, out x, out y, _cellSize, LayerModel.CurrentOriginPosition);
                mousePosition = GridUtility.GetWorldPosition(x, y, _cellSize, LayerModel.CurrentOriginPosition);
            }
            _collisionRect.gameObject.SetActive(true);
            _collisionRect.position = mousePosition;
        }
    }
}
