using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSettingWindow : SlideableUI {
    static readonly string DefaultGridSizeKey = "DefaultGridSize";

    public override void Initalize() {
        base.Initalize();
        if (!PlayerPrefs.HasKey(DefaultGridSizeKey)) {
             PlayerPrefs.SetInt(DefaultGridSizeKey, 16);
        }
        gameObject.SetActive(false);
    }

    public void OnValueChanged(InputField inputField) {
        int value;
        string text = inputField.text;
        if (int.TryParse(text, out value)) {
            PlayerPrefs.SetInt(DefaultGridSizeKey, value);
        }
    }
}
