using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoints : FetchNOAAData
{
    public MeshRenderer m_map;
    public Transform m_point;

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

        if (!m_hasGotData && m_stations.Count > 1)
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
        foreach (NOAAStations ns in m_stations)
        {
            float x = (m_map.bounds.size.x * ns.longitude) / 360f;
            float y = (m_map.bounds.size.y * ns.latitude) / 180f;

            if (m_point != null)
                Instantiate(m_point, new Vector3(x + m_map.transform.position.x, y + m_map.transform.position.y, m_map.transform.position.z), Quaternion.identity, this.transform);
            else
                Debug.LogError("Coordinates were created, but no point object to instantiate!");
        }
    }
}
