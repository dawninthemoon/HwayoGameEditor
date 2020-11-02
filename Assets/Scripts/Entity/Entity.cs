using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class Entity {
        public class Field {
            public string fieldName;
            public string value;
            public Field(string f) {
                fieldName = f;
            }
            public Field(string f, string v) {
                fieldName = f;
                value = v;
            }
        }
        public string EntityName { get; set; }
        public int TextureIndex { get; set; }
        public Color EntityColor { get; set; }
        List<Field> _fields;

        public Entity(string name, int textureIndex, Color color) {
            EntityName = name;
            TextureIndex = textureIndex;
            EntityColor = color;
            _fields = new List<Field>();
        }

        public int GetFieldCount() => _fields.Count;

        public void AddField(string fieldName) {
            _fields.Add(new Field(fieldName));
        }
    }
}
