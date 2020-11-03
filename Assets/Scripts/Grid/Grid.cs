using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        Func<Grid, int, int, Layer.TileObject> _createGridObjectCallback;

        public Grid(float cellSize, Vector3 originPosition, Func<Grid, int, int, Layer.TileObject> createGridObject) {
            this.cellSize = cellSize;
            this._originPosition = originPosition;

            int width = LayerModel.CurrentGridWidth;
            int height = LayerModel.CurrentGridHeight;
            _gridArray = new Layer.TileObject[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }
            _createGridObjectCallback = createGridObject;

            //OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => { };
        }

        public void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            int width = LayerModel.CurrentGridHeight + widthDelta;
            int height = LayerModel.CurrentGridHeight + heightDelta;
            bool wChanged = !_originPosition.x.Equals(originPosition.x);
            bool hChanged = !_originPosition.y.Equals(originPosition.y);

            Layer.TileObject[,] gridArray = new Layer.TileObject[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    gridArray[x, y] = _createGridObjectCallback(this, x, y);
                }
            }

            if ((wChanged || hChanged)) {
                for (int x = 0; x < LayerModel.CurrentGridWidth; ++x) {
                    for (int y = 0; y < LayerModel.CurrentGridHeight; ++y) {
                        int idx = _gridArray[x, y].GetTileIndex();
                        if (idx == 0) continue;

                        int alteredX = wChanged ? x : x + widthDelta;
                        int alteredY = hChanged ? y : y + heightDelta;
                        if (alteredX < 0 || alteredY < 0 || alteredX >= width || alteredY >= height)
                            continue;

                        gridArray[alteredX, alteredY].SetTileIndex(idx);
                    }
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

        private void GetXY(Vector3 worldPosition, out int x, out int y) {
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
