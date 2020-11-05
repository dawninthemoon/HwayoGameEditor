using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public delegate void OnEntityAdded(int id, Entity entity);
    public class EntityModel : MonoBehaviour {
        Dictionary<int, Entity> _entityDictionary;
        public Dictionary<int, Entity> EntityDictionary { get { return _entityDictionary;} }
        OnEntityAdded _onEntityAdded;
        public int SelectedIndex { get; set; } = -1;
        static string DictionarySaveKey = "Key_EntityModel_Dictionary";

        void Awake() {
            _entityDictionary = ES3.Load(DictionarySaveKey, new Dictionary<int, Entity>());
        }
        
        public void SetOnEntityAdded(OnEntityAdded callback) {
            _onEntityAdded = callback;
        }

        public bool IsEntityEmpty() => (_entityDictionary.Count == 0);

        public void AddEntity(Entity entity, int id) {
            _entityDictionary.Add(id, entity);
            _onEntityAdded(id, entity);
            ES3.Save(DictionarySaveKey, _entityDictionary);
        }

        public Entity GetEntityByID(int id) {
            if (id < 0) return null;
            return _entityDictionary[id];
        }

        public void DeleteEntityByID(int entityID) {
            _entityDictionary.Remove(entityID);
            ES3.Save(DictionarySaveKey, _entityDictionary);
        }

        public void SaveDictionary() {
            ES3.Save(DictionarySaveKey, _entityDictionary);
        }
    }
}
