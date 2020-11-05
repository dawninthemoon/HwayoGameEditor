using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class LevelModel : MonoBehaviour {
        [SerializeField] EntityModel _entityModel = null;
        [SerializeField] LayerModel _layerModel = null;
        public int _currentLevel;

        public void SaveLevel() {
            ES3.Save<int>("test", 3, "test");
            //ES3.Save<LayerModel>("layerModel", _layerModel, "layerModel_" + _currentLevel.ToString());
            foreach (var layer in _layerModel.CurrentLayerDictionary) {
            //    ES3.Save<LayerModel>("layerModel", )
            }
        }
    }
}
