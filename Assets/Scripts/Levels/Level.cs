using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class Level {
        List<Layer> _layerLists;
        public Level() {
            _layerLists = new List<Layer>();
        }
    }
}
