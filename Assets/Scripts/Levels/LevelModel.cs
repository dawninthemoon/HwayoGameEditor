using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class LevelModel : MonoBehaviour {
        [SerializeField] EntityModel _entityModel = null;
        [SerializeField] LayerModel _layerModel = null;
        public static int CurrentLevelID;
        Dictionary<int, Level> _levelDictionary;
        public Dictionary<int, Level> LevelDictionary { get { return _levelDictionary; } }
        static string LevelDictionaryKey = "Key_LevelModel";

        void Awake() {
            _levelDictionary = ES3.Load(LevelDictionaryKey, new Dictionary<int, Level>());
            if (_levelDictionary.Count == 0) {
                _levelDictionary.Add(0, new Level(0, "Default Level 0"));
                CurrentLevelID = 0;
            }
            else {
                foreach (var levelID in _levelDictionary.Keys) {
                    CurrentLevelID = levelID;
                    break;
                }
            }
        }

        public void AddLevel(Level level) {
            _levelDictionary.Add(level.LevelID, level);
            SaveLevel();
        }

        public void SaveLevel() {
            ES3.Save(LevelDictionaryKey, _levelDictionary);
        }

        public void ChangeLevel(int id) {
            if (id == CurrentLevelID) return;

            _entityModel.SaveEntites();
            _layerModel.SaveLayer();

            CurrentLevelID = id;
            if (_levelDictionary.TryGetValue(id, out Level level)) {
                _entityModel.LoadEntities();
                _layerModel.LoadLayers();
            }
        }

        public Level GetLevelByID(int id) {
            if (!_levelDictionary.TryGetValue(id, out Level level)) {
                Debug.LogError("key not exist");
            }
            return level;
        }

        public void RemoveLevelByID(int id) {
            if (_levelDictionary.TryGetValue(id, out Level level)) {
                _levelDictionary.Remove(id);
                SaveLevel();
            }
            else {
                Debug.Log("Level ID not exist");
            }
        }
    }
}
