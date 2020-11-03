using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public enum LayerType { Tile, Entity }
    public class Layer {
        public string LayerName { get; set; }
        public int LayerID { get; private set; }
        Grid _grid;

        public Layer(string layerName, int layerIndex, float cellSize, Vector3 originPosition) {
            LayerName = layerName;
            LayerID = layerIndex;
            _grid = new Grid(cellSize, originPosition, (Grid g, int x, int y) => new TileObject(g, x, y));
        }

        public void SetTileIndex(Vector3 worldPosition, int tileIndex) {
            TileObject tilemapObject = _grid.GetGridObject(worldPosition);
            tilemapObject?.SetTileIndex(tileIndex);
        }

        public void SetTilemapVisual(TilemapVisual tilemapVisual) {
            tilemapVisual.SetGrid(this, _grid);
        }

        public void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            _grid.ResizeGrid(originPosition, widthDelta, heightDelta);
        }

        public class TileObject {
            Grid _grid;
            int _x;
            int _y;
            int _tileIndex;

            public TileObject(Grid grid, int x, int y) {
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