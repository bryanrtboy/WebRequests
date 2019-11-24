using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoints : FetchNOAAData
{
    [Tooltip("Drag a Quad or similar game object with a meshrenderer to scale the mapped coordinates to fit it's bounds.")]
    public MeshRenderer m_map;
    [Tooltip("Game object to create for each point in the dataset")]
    public Transform m_point;
    public bool m_trimZeroHeightWave = false;
    public bool m_sphericalMapping = false;
    public float m_sphereRadius = 1;


    bool m_hasGotText = false;
    [HideInInspector]
    public bool m_hasGotData = false;
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
                CreatePoints();
            else
                Debug.LogError("No map to place points on!");
        }

    }

    void CreatePoints()
    {
        foreach (NOAAStationData ns in m_stations)
        {
            //Don't make a point if we don't want bouys with no wave data
            if (m_trimZeroHeightWave && ns.waveHeight == 0)
                continue;

            Vector3 mapPosition = Vector3.zero;

            if (m_sphericalMapping)
                mapPosition = SphericalCoordinate(ns.longitude, ns.latitude, m_sphereRadius);
            else
                mapPosition = RectangularCoordinates(ns.longitude, ns.latitude, m_map);


            if (m_point != null)
            {
                Transform t = Instantiate(m_point, mapPosition, Quaternion.identity, this.transform);
                t.GetComponent<Station>().m_stationData = ns;
            }
            else
            {
                Debug.LogError("Coordinates were created, but no point object to instantiate!");
            }
        }
    }

    Vector3 RectangularCoordinates(float longi, float lati, MeshRenderer meshRend)
    {

        float x = (meshRend.bounds.size.x * longi) / 360f;
        float y = (meshRend.bounds.size.y * lati) / 180f;

        return new Vector3(x + meshRend.transform.position.x, y + meshRend.transform.position.y, meshRend.transform.position.z);

    }

    // Get the spherical Coordinate
    Vector3 SphericalCoordinate(float longi, float lati, float radius)
    {

        // Method 1: Transfer to Radians from Degrees
        // float theta = longi * Mathf.Deg2Rad;
        // float phi = lati * Mathf.Deg2Rad;

        // float Xpos = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
        // float Ypos = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
        // float Zpos = radius * Mathf.Cos(phi);

        // // Set the X,Y,Z pos from the long and lat
        // return new Vector3(Xpos, Zpos, Ypos);

        return Quaternion.AngleAxis(longi, -Vector3.up) * Quaternion.AngleAxis(lati, -Vector3.right) * new Vector3(0, 0, radius);

    }
}
