using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 20f, scrollSpeed = 20f;
    [SerializeField]
    float maxZoom = 250f, minZoom = 45f;
    [SerializeField]
    Vector3 _prevMouseScreenPos;
    bool _isNotOnUI;

    void Update() {
        Vector3 pos = transform.position;
        
        if (Input.GetKeyDown(KeyCode.Mouse2)) {
            _prevMouseScreenPos = Input.mousePosition;
            _isNotOnUI = !EventSystem.current.IsPointerOverGameObject();
        }
        if (_isNotOnUI && Input.GetKey(KeyCode.Mouse2)) {
            Vector3 cur = Input.mousePosition;
            Vector3 delta = _prevMouseScreenPos - cur;
            
            pos += delta.normalized * moveSpeed;

            _prevMouseScreenPos = cur;
        }

        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        //zoom in out
        cam.orthographicSize += -scroll * scrollSpeed * Time.deltaTime * 10f;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        //camera boundery
        pos.z = -10;
        transform.position = pos;
    }
}