using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aroma {
    public class LineUtility : Singleton<LineUtility> {
        List<LineRenderer> _activeLines = new List<LineRenderer>();

        LineRenderer CreateLineRenderer() {
            GameObject obj = new GameObject();
            var lr = obj.AddComponent<LineRenderer>();
            lr.startWidth = lr.endWidth = 0.5f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            return lr;
        }

        public void DrawLine(Vector3 start, Vector3 end, Color color) {
            var lr = CreateLineRenderer();
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.startColor = lr.endColor = color;
            _activeLines.Add(lr);
        }
    }

    public static class Utility {
        public static Vector3 GetMouseWorldPosition() {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPosition;
        }
    }
}
