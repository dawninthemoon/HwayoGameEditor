using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class Entity {
        public string EntityName { get; set; }
        public int TextureIndex { get; set; }
        public Color EntityColor { get; set; }
        public int EntityID { get; set; }
        public int FieldSequence { get; set; }
        List<string> _fields;
        public List<string> Fields { get { return _fields; } }

        public Entity() {
            _fields = new List<string>();
        }

        public Entity(string name, int textureIndex, int id, Color color) {
            EntityName = name;
            TextureIndex = textureIndex;
            EntityColor = color;
            EntityID = id;
            _fields = new List<string>();
        }

        public int GetFieldCount() => _fields.Count;

        public void AddField(string fieldName) {
            _fields.Add(fieldName);
        }

        public void ChangeFieldName(string prevName, string curName) {
            _fields.Remove(prevName);
            _fields.Add(curName);
        }
    }
}
