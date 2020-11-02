using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public enum LayerType { Tile, Entity }
    public class Layer {
        public string LayerName { get; set; }
        public int LayerIndex { get; private set; }
        Grid<TileObject> _grid;
        public Layer(string layerName, int layerIndex, int width, int height, float cellSize, Vector3 originPosition) {
            LayerName = layerName;
            LayerIndex = layerIndex;
            _grid = new Grid<TileObject>(width, height, cellSize, originPosition, (Grid<TileObject> g, int x, int y) => new TileObject(g, x, y));
        }

        public void SetTileIndex(Vector3 worldPosition, int tileIndex) {
            TileObject tilemapObject = _grid.GetGridObject(worldPosition);
            tilemapObject?.SetTileIndex(tileIndex);
        }

        public void SetTilemapVisual(TilemapVisual tilemapVisual) {
            tilemapVisual.SetGrid(this, _grid);
        }

         public class TileObject {
            Grid<TileObject> _grid;
            int _x;
            int _y;
            int _tileIndex;

            public TileObject(Grid<TileObject> grid, int x, int y) {
                _grid = grid;
                _x = x;
                _y = y;
                _tileIndex = -1;
            }

            public void SetTileIndex(int tileIndex) {
                _tileIndex = tileIndex;
                _grid.TriggerGridObjectChanged(_x, _y);
            }

            public int GetTileIndex() => _tileIndex;
        }
    }
}