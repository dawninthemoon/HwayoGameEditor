using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshImageRender : MonoBehaviour
{
    [SerializeField] Vector2 _uv;
    MeshRenderer _meshRenderer;

    private void Awake() {
        Vector3[] vertices = new Vector3[] {
            new Vector3(-1f, 1f), new Vector3(1f, 1f),
            new Vector3(1f, -1f), new Vector3(-1f, 1f)
        };
        for (int i=0; i  <vertices.Length; ++i) {
            vertices[i] *= 348f;
        }
        int[] triangles = new int[] { 0, 1, 2,
                                      0, 2, 3 };
        Mesh mesh = new Mesh();
        Vector2[] uvs = new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material) {
        _meshRenderer.material = material;
    }
    
    public void SetUVCoord(Vector2 uv) {
        _uv = uv;
    }
}
