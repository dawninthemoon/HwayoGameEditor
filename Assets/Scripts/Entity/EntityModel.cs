using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public delegate void OnEntityAdded(Entity entity);
    public class EntityModel : MonoBehaviour {
        EntityPickerWindow _pickerWindow;
        Dictionary<int, Entity> _entityDictionary;
        public Dictionary<int, Entity> EntityDictionary { get { return _entityDictionary;} }
        public int SelectedIndex { get; set; } = -1;
        static string DictionarySaveKey = "Key_EntityModel_Dictionary";
        ProjectEntityWindow _projectEntityWindow;

        void Awake() {
            _pickerWindow = GameObject.Find("EntityPickerRect").GetComponent<EntityPickerWindow>();
            _pickerWindow.gameObject.SetActive(false);
            _projectEntityWindow = GameObject.Find("Project Entities").GetComponent<ProjectEntityWindow>();
            LoadEntities();
        }

        public void LoadEntities() {
            string keyName = DictionarySaveKey + "_" + LevelModel.CurrentLevelID;
            _entityDictionary = ES3.Load(keyName , new Dictionary<int, Entity>());
            _pickerWindow.DeleteAllButtons();
            foreach (var entity in _entityDictionary.Values) {
                _pickerWindow.AddButton(entity);
            }
            _projectEntityWindow.LoadEntites();
        }

        public void DeleteEntitySaves() {
            string keyName = DictionarySaveKey + "_" + LevelModel.CurrentLevelID;
            ES3.DeleteKey(keyName);
        }

        public void SaveEntites() {
            string keyName = DictionarySaveKey + "_" + LevelModel.CurrentLevelID;
            ES3.Save(keyName, _entityDictionary);
        }

        public bool IsEntityEmpty() => (_entityDictionary.Count == 0);

        public void AddEntity(Entity entity, int id) {
            _entityDictionary.Add(id, entity);
            _pickerWindow.AddButton(entity);
            SaveEntites();
        }

        public Entity GetEntityByID(int id) {
            if (_entityDictionary.TryGetValue(id, out Entity entity)) {
                return entity;
            }
            return null;
        }

        public void DeleteEntityByID(int entityID) {
            _entityDictionary.Remove(entityID);
            _pickerWindow.DeleteButton(entityID);
            SaveEntites();
        }
    }
}
