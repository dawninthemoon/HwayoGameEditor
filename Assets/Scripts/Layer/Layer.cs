using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aroma;

namespace CustomTilemap {
    public enum LayerType { Tile, Entity }
    public abstract class Layer {
        public string LayerName { get; set; }
        public int LayerID { get; private set; }

        public Layer(string layerName, int layerIndex) {
            LayerName = layerName;
            LayerID = layerIndex;
        }

        public abstract void SetTileIndex(Vector3 worldPosition, int textureIndex);

        public abstract void ResizeGrid(Vector3 originPosition, int widthDelta, int heightDelta);
    }
}