using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class MakeLines : MonoBehaviour
{
    public bool m_sortByLongitude = false;

    LineRenderer m_line;
    MapPoints m_map;

    void Start()
    {
        m_line = this.GetComponent<LineRenderer>();
        m_map = FindObjectOfType<MapPoints>();
    }

    void LateUpdate()
    {
        if (m_map != null && m_map.m_hasGotData)
        {
            MakeAndConnectLines();
        }

    }

    void MakeAndConnectLines()
    {
        List<Station> stations = FindObjectsOfType<Station>().ToList();


        if (stations != null)
        {

            if (m_sortByLongitude)
                stations.Sort(SortByLongitude);
            else
                stations.Sort(SortByLatitude);

            List<Vector3> nodes = new List<Vector3>();
            foreach (Station s in stations)
            {
                nodes.Add(s.transform.position);
            }

            m_line.positionCount = nodes.Count;
            m_line.SetPositions(nodes.ToArray());
            Destroy(this);
        }
    }

    static int SortByLongitude(Station p1, Station p2)
    {
        return p1.m_stationData.longitude.CompareTo(p2.m_stationData.longitude);
    }

    static int SortByLatitude(Station p1, Station p2)
    {
        return p1.m_stationData.latitude.CompareTo(p2.m_stationData.latitude);
    }


}
