using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

namespace Aroma {
    public static class GridUtility {
        public static readonly string DefaultGridSizeKey = "DefaultGridSize";
        public static Vector3 GetWorldPosition(int x, int y, float cellSize, Vector3 originPosition) {
            return new Vector3(x, y) * cellSize + originPosition;
        }
        public static void GetXY(Vector3 worldPosition, out int x, out int y, float cellSize, Vector3 originPosition) {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }

        public static Vector2 ClampPosition(Vector2 position, float cellSize) {
            Vector2 p00 = GridUtility.GetWorldPosition(0, LayerModel.CurrentGridHeight, cellSize, LayerModel.CurrentOriginPosition);
            Vector2 p11 = GridUtility.GetWorldPosition(LayerModel.CurrentGridWidth, 0, cellSize, LayerModel.CurrentOriginPosition);
            
            position.x = Mathf.Clamp(position.x, p00.x, p11.x);
            position.y = Mathf.Clamp(position.y, p11.y, p00.y);
            
            return position;
        }
    }

    public class TileObjectPool : Singleton<TileObjectPool> {
        ObjectPoolWithoutComponent<TileLayer.TileObject> _tileobjectPool;

        public TileObjectPool() {
            int gridSize = PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey);
            _tileobjectPool = new ObjectPoolWithoutComponent<TileLayer.TileObject>(gridSize * gridSize * 2, () => new TileLayer.TileObject());
        }

        public TileLayer.TileObject GetTileObject(CustomTilemap.CustomGrid<TileLayer.TileObject> grid, int x, int y) {
            var tile = _tileobjectPool.GetObject();
            tile.Initalize(grid, x, y);
            return tile;
        }

        public void ReturnObject(TileLayer.TileObject tile) {
            _tileobjectPool.ReturnObject(tile);
        }
    }

    public class LineUtility : Singleton<LineUtility> {
        ObjectPool<LineRenderer> _lineRendererPool;
        LineRenderer[] _rectRenderer;
        Material _gpuInstancing;
        public LineUtility() {
            _gpuInstancing = Resources.Load<Material>("Others/GPUInstancing");
            int gridSize = PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey);
            _lineRendererPool = new ObjectPool<LineRenderer>(gridSize, CreateLineRenderer);
            _rectRenderer = new LineRenderer[4] {
                CreateLineRenderer(),
                CreateLineRenderer(),
                CreateLineRenderer(),
                CreateLineRenderer(),
            };
        }

        public Material GetMaterial() => _gpuInstancing;

        public void ClearAllLines() {
            _lineRendererPool.Clear();
        }

        public void DisableRect() {
            foreach (var lr in _rectRenderer) {
                lr.gameObject.SetActive(false);
            }
        }

        public void DrawRect(Vector2 p00, Vector2 p10, Vector2 p11, Vector2 p01, Color color, float width) {
            foreach (var lr in _rectRenderer) {
                lr.gameObject.SetActive(true);
            }

            DrawLine(_rectRenderer[0], p00, p01, color, width);
            DrawLine(_rectRenderer[1], p01, p11, color, width);
            DrawLine(_rectRenderer[2], p11, p10, color, width);
            DrawLine(_rectRenderer[3], p10, p00, color, width);
        }

        LineRenderer CreateLineRenderer() {
            GameObject obj = new GameObject();
            var lr = obj.AddComponent<LineRenderer>();
            lr.startWidth = lr.endWidth = 0.5f;
            lr.material = _gpuInstancing;
            return lr;
        }

        public void DrawLine(Vector3 start, Vector3 end, Color color, float width = 0.5f) {
            var lr = _lineRendererPool.GetObject();
            DrawLine(lr, start, end, color, width);
        }

        public void DrawLine(LineRenderer lr, Vector3 start, Vector3 end, Color color, float width) {
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.startColor = lr.endColor = color;
            lr.receiveShadows = false;
            lr.startWidth = lr.endWidth = width;
        }
    }

    public static class Utility {
        public static Vector3 GetMouseWorldPosition() {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }

        public static void SortRectanglePoints(ref Vector2 p00, ref Vector2 p10, ref Vector2 p11, ref Vector2 p01) {
            Vector2[] arr = new Vector2[4] { p00, p10, p11, p01 };

            Vector2[] check = new Vector2[4];
            for (int i = 0; i < 4; ++i) {
                check[i] = Vector2.zero;
                for (int j = 0; j < 4; ++j) {
                    if (i == j) continue;
                    if (arr[i].x > arr[j].x) ++check[i].x;
                    else if (arr[i].x < arr[j].x) --check[i].x;
                    if (arr[i].y > arr[j].y) ++check[i].y;
                    else if (arr[i].y < arr[j].y) --check[i].y;
                }
            }

            for (int i = 0; i < 4; ++i) {
                if (check[i].x < -1.5f && check[i].y > 1.5f)
                    p00 = arr[i];
                else if (check[i].x > 1.5f && check[i].y > 1.5f)
                    p10 = arr[i];
                else if (check[i].x < 1.5f && check[i].y < 1.5f)
                    p01 = arr[i];
                else
                    p11 = arr[i];
            }
        }

        public static void Swap<T>(ref T value1, ref T value2) {
            T temp = value1;
            value1 = value2;
            value2 = temp;
        }
    }
}
