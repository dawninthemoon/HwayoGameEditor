using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTilemap;

public class CollisionLayerWindow : MonoBehaviour {
    [SerializeField] InputField _inputField = null;
    [SerializeField] LayerModel _layerModel = null;
    bool _ignoreCallback;
    ProjectLayerWindow _projectLayerWindow;

    void Start() {
        _projectLayerWindow = GetComponentInParent<ProjectLayerWindow>();
    }

    public void OnLayerNameInputFieldChanged(InputField inputField) {
        if (_ignoreCallback) return;
        var layer = _layerModel.GetLayerByIndex(_projectLayerWindow.SelectedLayerIDInWindow);
        layer.LayerName = inputField.text;
        Button button = _projectLayerWindow.GetButtonByIndex(layer.LayerID);
        button.GetComponentInChildren<Text>().text = layer.LayerName;
    }

    public void SetInputFieldTextWithIgnoreCallback(Layer layer) {
        _ignoreCallback = true;
        _inputField.text = layer.LayerName;
        _ignoreCallback = false;
    }
}
