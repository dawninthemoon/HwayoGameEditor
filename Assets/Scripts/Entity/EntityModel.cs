using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class EntityModel : MonoBehaviour {
        Dictionary<int, Entity> _entityDictionary = new Dictionary<int, Entity>();

        public bool IsEntityEmpty() => (_entityDictionary.Count == 0);

        public void AddEntity(Entity entity, int id) {
            _entityDictionary.Add(id, entity);
        }

        public Entity GetEntityByID(int id) {
            return _entityDictionary[id];
        }

        public void DeleteEntityByID(int entityID) {
            _entityDictionary.Remove(entityID);
        }
    }
}
