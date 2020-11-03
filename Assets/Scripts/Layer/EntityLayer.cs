using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class EntityLayer : Layer {
        public EntityLayer(string layerName, int layerIndex, float cellSize, Vector3 originPosition)
        : base(layerName, layerIndex, cellSize, originPosition) { }
    }
}
