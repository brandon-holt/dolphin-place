using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
    public Mesh planetMesh;
    public Material waterMaterial;
    public MeshRenderer waterRenderer;
    public Vector3 amplitude, frequency;
    public Gradient surfaceGradient;
    public Color waterColor;

    private void Start()
    {
        GenerateTerrain();

        SetWaterColor();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        GenerateTerrain();

        SetWaterColor();
    }
#endif

    private void GenerateTerrain()
    {
        if (planetMesh == null) return;

        Mesh newMesh = new Mesh();

        Vector3[] vertices = planetMesh.vertices;

        float[] heights = new float[vertices.Length];

        float minHeight = float.PositiveInfinity; float maxHeight = float.NegativeInfinity;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];

            float phi = Mathf.Atan(Mathf.Sqrt((v.x * v.x) + (v.y * v.y)) / v.z);

            float theta = Mathf.Atan2(v.y, v.x);

            float x = phi / Mathf.PI;

            float z = theta / (2 * Mathf.PI);

            float noise = amplitude.x * Mathf.PerlinNoise(x * frequency.x, z * frequency.x)
                        + amplitude.y * Mathf.PerlinNoise(x * frequency.y, z * frequency.y)
                        + amplitude.z * Mathf.PerlinNoise(x * frequency.z, z * frequency.z);

            vertices[i] *= 1f + (.1f * noise);

            heights[i] = vertices[i].magnitude;

            if (heights[i] < minHeight) minHeight = heights[i];

            if (heights[i] > maxHeight) maxHeight = heights[i];
        }

        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, heights[i]);

            colors[i] = surfaceGradient.Evaluate(normalizedHeight);
        }

        newMesh.vertices = vertices;

        newMesh.triangles = planetMesh.triangles;

        newMesh.colors = colors;

        newMesh.RecalculateNormals();

        newMesh.RecalculateBounds();

        newMesh.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = newMesh;
    }

    private void SetWaterColor()
    {
        if (waterMaterial == null || waterRenderer == null) return;

        Material waterMaterialInstance = new Material(waterMaterial);

        waterMaterialInstance.SetColor("_Water_Color", waterColor);

        waterMaterialInstance.SetFloat("_Alpha", waterColor.a);

        waterRenderer.material = waterMaterialInstance;
    }
}