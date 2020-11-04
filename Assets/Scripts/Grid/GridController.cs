using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public delegate void OnGridResized();
    public class GridController : MonoBehaviour {
        [SerializeField] GridScaler _gridScalerPrefab = null;
        [SerializeField] float _rectWidth = 1f;
        [SerializeField] LayerModel _layerModel = null;
        float _cellSize = 16;
        Vector3 _originPosition;
        Vector3[] _vertexes = new Vector3[8];
        Vector3[] _units;
        static readonly Vector2 DefaultColliderSize = new Vector2(16f, 16f);
        GridScaler[] _gridScalers;

        void Start() {
            _gridScalers = new GridScaler[8];
            for (int i = 0; i < _gridScalers.Length; ++i) {
                _gridScalers[i] = Instantiate(_gridScalerPrefab, _vertexes[i], Quaternion.identity).GetComponent<GridScaler>();
                _gridScalers[i].SetCallbackFunc(OnScalerChanging, OnScalerChanged);
            }
            DrawGridLine();
            UpdateGridScalers();

            _layerModel.SetOnGridResized(UpdateGridLine);
        }

        void UpdateGridScalers() {
            int width = LayerModel.CurrentGridWidth;
            int height = LayerModel.CurrentGridHeight;

            _vertexes[0] = GridUtility.GetWorldPosition(0, height, _cellSize, _originPosition);
            _vertexes[1] = GridUtility.GetWorldPosition(width, height, _cellSize, _originPosition);
            _vertexes[2] = GridUtility.GetWorldPosition(width, 0, _cellSize, _originPosition);
            _vertexes[3] = GridUtility.GetWorldPosition(0, 0, _cellSize, _originPosition);
            
            for (int i = 0; i < _vertexes.Length / 2; ++i) {
                _vertexes[i + 4] = (_vertexes[(i + 1) % 4] - _vertexes[i]) * 0.5f + _vertexes[i];
            }
            
            for (int i = 0; i < _vertexes.Length; ++i) {
                _gridScalers[i].transform.position = _vertexes[i];
                _gridScalers[i].ScalerIndex = i;
            }
        }

        void OnScalerChanging(GridScaler scaler) {
            if (_layerModel.IsLayerEmpty()) return;
            if (!CanResizeGrid(scaler.ScalerIndex)) {
                Aroma.LineUtility.GetInstance().DisableRect();
                return;
            }

            Vector2 p00, p10, p11, p01;
            GetRectPoints(scaler.ScalerIndex, out p00, out p10, out p11, out p01);

            Aroma.LineUtility.GetInstance().DrawRect(p00, p10, p11, p01, Color.yellow, _rectWidth);
        }

        void OnScalerChanged(GridScaler scaler) {
            if (_layerModel.IsLayerEmpty()) return;
            Aroma.LineUtility.GetInstance().DisableRect();
            if (!CanResizeGrid(scaler.ScalerIndex)) return;
            
            Vector2 p00, p10, p11, p01;
            GetRectPoints(scaler.ScalerIndex, out p00, out p10, out p11, out p01);
            
            Utility.SortRectanglePoints(ref p00, ref p10, ref p11, ref p01);

            int curWidth = Mathf.FloorToInt(Mathf.Abs((p10 - p01).x) / 16f);
            int curHeight = Mathf.FloorToInt(Mathf.Abs((p10 - p01).y) / 16f);

            _originPosition = p01;
            _layerModel.ResizeAllLayers(p01, curWidth - LayerModel.CurrentGridWidth, curHeight - LayerModel.CurrentGridHeight);
        }

        bool CanResizeGrid(int index) {
            Vector2 cur = Utility.GetMouseWorldPosition();
            int width = LayerModel.CurrentGridWidth, height = LayerModel.CurrentGridHeight;

            int x, y;
            GridUtility.GetXY(cur, out x, out y, _cellSize, _originPosition);

            bool[] canResizeGridArr = new bool[8] {
                (x < width - 1) && (y > 0),
                (x > 0) && (y > 0),
                (x > 0) && (y < height - 1),
                (x < width - 1) && (y < height - 1),
                y > 0,
                x > 0,
                y < height - 1,
                x < width - 1
            };

            return canResizeGridArr[index];
        }

        void GetRectPoints(int index, out Vector2 p00, out Vector2 p10, out Vector2 p11, out Vector2 p01) {
            Vector3 cur = Utility.GetMouseWorldPosition();
            if (index >= 4) {
                if (index % 2 == 0) cur.x = _vertexes[index].x;
                else cur.y = _vertexes[index].y;
            }
            int x, y;
            GridUtility.GetXY(cur, out x, out y, _cellSize, _originPosition);
            Vector2 cursorPos = GridUtility.GetWorldPosition(x, y, _cellSize, _originPosition);

            if (index < 4) {
                int diagonalIndex = (index + 2) % 4;
                p00 = _vertexes[diagonalIndex];
                p10 = new Vector2(cursorPos.x, p00.y);
                p11 = new Vector2(cursorPos.x, cursorPos.y);
                p01 = new Vector2(p00.x, cursorPos.y);
            }
            else {
                int parallelIndex = (index + 2) % 4 + 4;
                Vector2 p = _vertexes[parallelIndex];
                float halfWidth = 
                    (GridUtility.GetWorldPosition(LayerModel.CurrentGridWidth, 0, _cellSize, _originPosition).x - 
                    GridUtility.GetWorldPosition(0, 0, _cellSize, _originPosition).x) * 0.5f;
                float halfHeight = 
                    (GridUtility.GetWorldPosition(0, LayerModel.CurrentGridHeight, _cellSize, _originPosition).y -
                    GridUtility.GetWorldPosition(0, 0, _cellSize, _originPosition).y) * 0.5f;

                Debug.Log(halfWidth);

                if (index % 2 == 0) {
                    p00 = new Vector2(p.x - halfWidth, p.y);
                    p10 = new Vector2(p.x + halfWidth, p.y);
                    p11 = new Vector2(p.x + halfWidth, cursorPos.y);
                    p01 = new Vector2(p.x - halfWidth, cursorPos.y);
                }
                else {
                    p00 = new Vector2(p.x, p.y + halfHeight);
                    p10 = new Vector2(cursorPos.x, p.y + halfHeight);
                    p11 = new Vector2(cursorPos.x, p.y - halfHeight);
                    p01 = new Vector2(p.x, p.y - halfHeight);
                }
            }
        }

        public void UpdateGridLine() {
            Aroma.LineUtility.GetInstance().ClearAllLines();
            DrawGridLine();
            UpdateGridScalers();
        }

        public void DrawGridLine() {
            var lineUtility = Aroma.LineUtility.GetInstance();
            int width = LayerModel.CurrentGridWidth;
            int height = LayerModel.CurrentGridHeight;
            
            for (int i = 0; i <= height; ++i) {
                Vector2 p1 = GridUtility.GetWorldPosition(0, i, _cellSize, _originPosition);
                Vector2 p2 = GridUtility.GetWorldPosition(width, i, _cellSize, _originPosition);
                lineUtility.DrawLine(p1, p2, Color.white);
            }
            for (int i = 0; i <= width; ++i) {
                Vector2 p1 = GridUtility.GetWorldPosition(i, 0, _cellSize, _originPosition);
                Vector2 p2 = GridUtility.GetWorldPosition(i, height, _cellSize, _originPosition);
                lineUtility.DrawLine(p1, p2, Color.white);
            }
        }
    }
}
