using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class EntityLayer : Layer {
        public EntityLayer(string layerName, int layerIndex, int width, int height, float cellSize, Vector3 originPosition)
        : base(layerName, layerIndex, width, height, cellSize, originPosition) {
                
        }
    }
}
