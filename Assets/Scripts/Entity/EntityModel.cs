using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public delegate void OnEntityAdded(int id, Entity entity);
    public class EntityModel : MonoBehaviour {
        Dictionary<int, Entity> _entityDictionary = new Dictionary<int, Entity>();
        public Dictionary<int, Entity> EntityDictionary { get { return _entityDictionary;} }
        OnEntityAdded _onEntityAdded;
        public int SelectedIndex { get; set; } = -1;
        
        public void SetOnEntityAdded(OnEntityAdded callback) {
            _onEntityAdded = callback;
        }

        public bool IsEntityEmpty() => (_entityDictionary.Count == 0);

        public void AddEntity(Entity entity, int id) {
            _entityDictionary.Add(id, entity);
            _onEntityAdded(id, entity);
        }

        public Entity GetEntityByID(int id) {
            return _entityDictionary[id];
        }

        public void DeleteEntityByID(int entityID) {
            _entityDictionary.Remove(entityID);
        }
    }
}
