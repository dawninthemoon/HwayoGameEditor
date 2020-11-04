using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class TileLayer : Layer {
        public int DropdownValue { get; set; }
        public string TilesetName { get; set; }

        public TileLayer(string layerName, int layerIndex, float cellSize, Vector3 originPosition)
        : base(layerName, layerIndex, cellSize, originPosition) {
            TilesetName = TilesetModel.DefaultTilesetName;
        }
    }
}
