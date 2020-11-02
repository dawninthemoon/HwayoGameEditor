using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectLevelWindow : SlideableUI {
    public override void Initalize() {
        base.Initalize();
        gameObject.SetActive(false);
    }
}
