using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTilemap;

public class EntityPickerWindow : MonoBehaviour {
    
    public OnPicked _onPicked;

    public void SetOnEntityPicked(OnPicked callback) {
        _onPicked = callback;
    }
}
