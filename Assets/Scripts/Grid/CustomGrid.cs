using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Aroma;
using CustomTilemap;

public interface IGridObject {
    int GetIndex();
    void SetIndex(int index);
    void SetGrid(object grid);
}

namespace CustomTilemap {
    public class CustomGrid<T> where T : IGridObject {
        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        public class OnGridObjectChangedEventArgs : EventArgs {
            public int x;
            public int y;
        }

        private float cellSize;
        private T[,] _gridArray;
        public T[,] GridArray { 
            get { return _gridArray; }
            set { 
                _gridArray = value; 
                foreach (var obj in _gridArray) {
                    obj.SetGrid(this);
                }
            } 
        }
        Func<CustomGrid<T>, int, int, T> _createObjectCallback;
        Action<T> _returnObjectCallback;

        public CustomGrid(float cellSize, Func<CustomGrid<T>, int, int, T> createGridObject, Action<T> returnObject = null) {
            this.cellSize = cellSize;

            int width = LayerModel.CurrentGridWidth;
            int height = LayerModel.CurrentGridHeight;
            _gridArray = new T[width, height];

            _createObjectCallback = createGridObject;
            _returnObjectCallback = returnObject;

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                   _gridArray[x, y] = _createObjectCallback(this, x, y);
                }
            }
        }

        public void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            int prevWidth = LayerModel.CurrentGridWidth - widthDelta;
            int prevHeight = LayerModel.CurrentGridHeight - heightDelta;
            bool wChanged = !(Mathf.Abs(LayerModel.CurrentOriginPosition.x - originPosition.x) < Mathf.Epsilon);
            bool hChanged = !(Mathf.Abs(LayerModel.CurrentOriginPosition.y - originPosition.y) < Mathf.Epsilon);

            T[,] gridArray = new T[LayerModel.CurrentGridWidth, LayerModel.CurrentGridHeight];
            
            for (int x = 0; x < LayerModel.CurrentGridWidth; ++x) {
                for (int y = 0; y < LayerModel.CurrentGridHeight; ++y) {
                    gridArray[x, y] = _createObjectCallback(this, x, y);
                }
            }
            
            for (int x = 0; x < prevWidth; ++x) {
                for (int y = 0; y < prevHeight; ++y) {
                    int idx = _gridArray[x, y].GetIndex();
                    _returnObjectCallback?.Invoke(_gridArray[x, y]);
                    if (idx == -1) continue;

                    int alteredX = wChanged ? x + widthDelta : x;
                    int alteredY = hChanged ? y + heightDelta : y;

                    if (alteredX < 0 || alteredY < 0 || alteredX >= LayerModel.CurrentGridWidth || alteredY >= LayerModel.CurrentGridHeight) {
                        continue;
                    }

                    gridArray[alteredX, alteredY].SetIndex(idx);
                }
            }

            _gridArray = gridArray.Clone() as T[,];
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
            return Aroma.GridUtility.GetWorldPosition(x, y, cellSize, LayerModel.CurrentOriginPosition);
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y) {
            Aroma.GridUtility.GetXY(worldPosition, out x, out y, cellSize, LayerModel.CurrentOriginPosition);
        }

        public void SetGridObject(int x, int y, T value) {
            if (x >= 0 && y >= 0 && x < LayerModel.CurrentGridWidth && y < LayerModel.CurrentGridHeight) {
                _gridArray[x, y] = value;
                OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
            }
        }

        public void TriggerGridObjectChanged(int x, int y) {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        public void SetGridObject(Vector3 worldPosition, T value) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetGridObject(x, y, value);
        }

        public T GetGridObject(int x, int y) {
            if (x >= 0 && y >= 0 && x < LayerModel.CurrentGridWidth && y < LayerModel.CurrentGridHeight) {
                return _gridArray[x, y];
            } else {
                return default(T);
            }
        }

        public T GetGridObject(Vector3 worldPosition) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetGridObject(x, y);
        }
    }
}
