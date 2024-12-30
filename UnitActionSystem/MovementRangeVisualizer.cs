using UnityEngine;

public class MovementRangeVisualizer : MonoBehaviour
{
    [SerializeField] private MeshRenderer rangeMeshRenderer;
    [SerializeField] private float rangeVisualizerHeight = 0.1f;
    private MoveAction moveAction;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        CreateRangeMesh();
    }

    private void CreateRangeMesh()
    {
        GameObject rangeObject = new GameObject("MovementRange");
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = Vector3.zero;

        MeshFilter meshFilter = rangeObject.AddComponent<MeshFilter>();
        rangeMeshRenderer = rangeObject.AddComponent<MeshRenderer>();

        // Basit bir disk mesh'i oluştur
        Mesh mesh = new Mesh();
        int segments = 32;
        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % segments + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        // Material ayarları
        Material material = new Material(Shader.Find("Transparent/Diffuse"));
        material.color = new Color(0, 0.5f, 1f, 0.3f);
        rangeMeshRenderer.material = material;
        
        rangeObject.transform.localPosition = new Vector3(0, rangeVisualizerHeight, 0);
        rangeObject.SetActive(false);
    }

    public void ShowRange(float radius)
    {
        rangeMeshRenderer.transform.localScale = Vector3.one * radius * 2;
        rangeMeshRenderer.gameObject.SetActive(true);
    }

    public void HideRange()
    {
        rangeMeshRenderer.gameObject.SetActive(false);
    }
} 