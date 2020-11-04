using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Aroma;

namespace CustomTilemap {
    public class Grid {
        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs {
            public int x;
            public int y;
        }

        private float cellSize;
        private Vector3 _originPosition;
        private Layer.TileObject[,] _gridArray;

        public Grid(float cellSize, Vector3 originPosition) {
            this.cellSize = cellSize;
            this._originPosition = originPosition;

            int width = LayerModel.CurrentGridWidth;
            int height = LayerModel.CurrentGridHeight;
            _gridArray = new Layer.TileObject[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                   _gridArray[x, y] = TileObjectPool.GetInstance().GetTileObject(this, x, y);
                }
            }
        }

        public void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            int prevWidth = LayerModel.CurrentGridWidth - widthDelta;
            int prevHeight = LayerModel.CurrentGridHeight - heightDelta;
            bool wChanged = !(Mathf.Abs(_originPosition.x - originPosition.x) < Mathf.Epsilon);
            bool hChanged = !(Mathf.Abs(_originPosition.y - originPosition.y) < Mathf.Epsilon);

            Layer.TileObject[,] gridArray = new Layer.TileObject[LayerModel.CurrentGridWidth, LayerModel.CurrentGridHeight];
            
            for (int x = 0; x < LayerModel.CurrentGridWidth; ++x) {
                for (int y = 0; y < LayerModel.CurrentGridHeight; ++y) {
                    gridArray[x, y] = TileObjectPool.GetInstance().GetTileObject(this, x, y);
                }
            }
            
            for (int x = 0; x < prevWidth; ++x) {
                for (int y = 0; y < prevHeight; ++y) {
                    int idx = _gridArray[x, y].GetTileIndex();
                    TileObjectPool.GetInstance().ReturnObject(_gridArray[x, y]);
                    if (idx == -1) continue;

                    int alteredX = wChanged ? x + widthDelta : x;
                    int alteredY = hChanged ? y + heightDelta : y;

                    if (alteredX < 0 || alteredY < 0 || alteredX >= LayerModel.CurrentGridWidth || alteredY >= LayerModel.CurrentGridHeight) {
                        continue;
                    }

                    gridArray[alteredX, alteredY].SetTileIndex(idx);
                }
            }

            _originPosition = originPosition;
            _gridArray = gridArray.Clone() as Layer.TileObject[,];
        }

        public int GetWidth() {
            return LayerModel.CurrentGridWidth;
        }

        public int GetHeight() {
            return LayerModel.CurrentGridHeight;
        }

        public float GetCellSize() {
            return cellSize;
        }

        public Vector3 GetWorldPosition(int x, int y) {
            return Aroma.GridUtility.GetWorldPosition(x, y, cellSize, _originPosition);
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y) {
            Aroma.GridUtility.GetXY(worldPosition, out x, out y, cellSize, _originPosition);
        }

        public void SetGridObject(int x, int y, Layer.TileObject value) {
            if (x >= 0 && y >= 0 && x < LayerModel.CurrentGridWidth && y < LayerModel.CurrentGridHeight) {
                _gridArray[x, y] = value;
                OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
            }
        }

        public void TriggerGridObjectChanged(int x, int y) {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        public void SetGridObject(Vector3 worldPosition, Layer.TileObject value) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetGridObject(x, y, value);
        }

        public Layer.TileObject GetGridObject(int x, int y) {
            if (x >= 0 && y >= 0 && x < LayerModel.CurrentGridWidth && y < LayerModel.CurrentGridHeight) {
                return _gridArray[x, y];
            } else {
                return default(Layer.TileObject);
            }
        }

        public Layer.TileObject GetGridObject(Vector3 worldPosition) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetGridObject(x, y);
        }
    }
}
