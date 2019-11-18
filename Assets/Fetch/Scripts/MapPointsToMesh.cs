using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointsToMesh : FetchNOAAData
{
    [Tooltip("Drag a Quad or similar game object with a meshrenderer to scale the mapped coordinates to fit it's bounds.")]
    public MeshRenderer m_map;

    bool m_hasGotText = false;
    bool m_hasGotData = false;
    // Start is called before the first frame update
    void Start()
    {
        RunNOAAScript();
    }

    // Update is called once per frame
    void Update()
    {

        if (!m_hasGotText && m_text != null)
        {
            m_hasGotText = true;
            ReadTextFile();

        }

        if (!m_hasGotData && m_stations != null && m_stations.Count > 1)
        {
            m_hasGotData = true;

            if (m_map != null)
                CreateMesh();
            else
                Debug.LogError("No map to place points on!");
        }

    }

    void CreateMesh()
    {
        //Create the vertices
        List<Vector2> mapVectors = new List<Vector2>();

        foreach (NOAAStationData ns in m_stations)
        {
            float x = (m_map.bounds.size.x * ns.longitude) / 360f;
            float y = (m_map.bounds.size.y * ns.latitude) / 180f;

            mapVectors.Add(new Vector2(x, y));
        }

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(mapVectors.ToArray());
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[mapVectors.Count];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(mapVectors[i].x, mapVectors[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        GameObject g = new GameObject();
        // Set up game object with mesh;
        g.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = g.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
        //Move the new game object to the m_map's position
        g.transform.position = m_map.transform.position;
        //Normals are pointing the wrong direction, so rotate it so we can see the resulting mesh
        //TODO:  Fix this so the normals are pointing the correct direction!
        g.transform.Rotate(Vector3.up, 180f);
        //Move the transform so it's not exactly on top of the map
        g.transform.Translate(Vector3.forward * .1f);
    }
}
