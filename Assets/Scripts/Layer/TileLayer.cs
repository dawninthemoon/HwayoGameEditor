using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public class TileLayer : Layer {
        public int DropdownValue { get; set; }
        public string TilesetName { get; set; }
        Grid<TileObject> _grid;
        TilesetVisual _tilesetVisual;
        public TilesetVisual  Visual { 
            get { return _tilesetVisual; }
            set {
                _tilesetVisual = value;
                _tilesetVisual.SetGrid(this, _grid);
            }
        }

        public TileLayer(string layerName, int layerIndex, float cellSize)
        : base(layerName, layerIndex) {
            TilesetName = TilesetModel.DefaultTilesetName;
            _grid = new Grid<TileObject>(cellSize, (Grid<TileObject> g, int x, int y) => TileObjectPool.GetInstance().GetTileObject(g, x, y));
        }

        public override void SetTileIndex(Vector3 worldPosition, int tileIndex) {
            TileObject tilemapObject = _grid.GetGridObject(worldPosition);
            tilemapObject?.SetIndex(tileIndex);
        }

        public override void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta) {
            _grid.ResizeGrid(originPosition, widthDelta, heightDelta);
        }

        public class TileObject : IGridObject {
            Grid<TileObject> _grid;
            int _x;
            int _y;
            int _textureIndex;

            public TileObject() {
                _textureIndex = -1;
            }

            public TileObject(Grid<TileObject> grid, int x, int y) {
                _grid = grid;
                _x = x;
                _y = y;
                _textureIndex = -1;
            }

            public void Initalize(Grid<TileObject> grid, int x, int y) {
                _grid = grid;
                _x = x;
                _y = y;
                _textureIndex = -1;
            }

            public void SetIndex(int tileIndex) {
                _textureIndex = tileIndex;
                _grid.TriggerGridObjectChanged(_x, _y);
            }

            public int GetIndex() => _textureIndex;
        }
    }
}
