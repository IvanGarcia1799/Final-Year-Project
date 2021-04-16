using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeshW : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xValue = 20;
    public int zValue = 20;
    public float scale = 10;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xValue + 1) * (zValue + 1)];

        int vertix = 0;
        for (int i = 0; i <=zValue; i++)
        {
            for (int j = 0; j <= xValue; j++)
            {
                float y = Mathf.PerlinNoise((j  + this.transform.position.x)/5.0f, (i + this.transform.position.z)/5.0f) * 1.5f;
                vertices[vertix] = new Vector3(j, y, i);
                vertix++;
            }
        }

        triangles = new int[xValue * zValue * 6];
        
        int vertiy = 0;
        int trian = 0;
        for(int j=0; j < zValue; j++)
        {
            for (int i = 0; i < xValue; i++)
            {    
                triangles[trian + 0] = vertiy + 0;
                triangles[trian + 1] = vertiy + xValue + 1;
                triangles[trian + 2] = vertiy + 1;
                triangles[trian + 3] = vertiy + 1;
                triangles[trian + 4] = vertiy + xValue + 1;
                triangles[trian + 5] = vertiy + xValue + 2;

                vertiy++;
                trian += 6;
            }
            vertiy++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    void Update()
    {

    }
}
