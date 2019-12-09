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

    [HideInInspector]
    public bool m_hasGotData = false;

    // Start is called before the first frame update
    void Start()
    {
        FetchRawData(m_latestObservationURL);
    }

    // Update is called once per frame
    void Update()
    {

        if (m_dataFetchingComplete && !m_hasGotData && m_stations != null && m_stations.Count > 1)
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
        foreach (NOAA_latest_observations ns in m_stations)
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

    Vector3 SphericalCoordinate(float longi, float lati, float radius)
    {
        return Quaternion.AngleAxis(longi, -Vector3.up) * Quaternion.AngleAxis(lati, -Vector3.right) * new Vector3(0, 0, radius);

    }
}
