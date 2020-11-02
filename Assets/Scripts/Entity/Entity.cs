using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class Entity {
        public string EntityName { get; set; }
        public int TextureIndex { get; set; }
        public Color EntityColor { get; set; }
        List<string> _fields;

        public Entity(string name, int textureIndex, Color color) {
            EntityName = name;
            TextureIndex = textureIndex;
            EntityColor = color;
            _fields = new List<string>();
        }

        public int GetFieldCount() => _fields.Count;
    }
}
