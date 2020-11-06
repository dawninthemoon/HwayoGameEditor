using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class Level {
        public int LevelID { get; set; }
        public string LevelName { get; set; }
        public Level() {
        }
        public Level(int id, string levelName) {
            LevelID = id;
            LevelName = levelName;
        }
    }
}
