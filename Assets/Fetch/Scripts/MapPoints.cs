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

            float x = (m_map.bounds.size.x * ns.longitude) / 360f;
            float y = (m_map.bounds.size.y * ns.latitude) / 180f;

            if (m_point != null)
            {
                Transform t = Instantiate(m_point, new Vector3(x + m_map.transform.position.x, y + m_map.transform.position.y, m_map.transform.position.z), Quaternion.identity, this.transform);
                t.GetComponent<Station>().m_stationData = ns;
            }
            else
            {
                Debug.LogError("Coordinates were created, but no point object to instantiate!");
            }
        }
    }
}
