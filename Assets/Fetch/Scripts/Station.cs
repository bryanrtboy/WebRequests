using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{

    public NOAAStationData m_stationData;
    public Color m_stationColor = Color.grey;
    public Color m_overColor = Color.red;
    public Color m_clickedColor = Color.blue;

    Renderer rend;
    CameraMovement m_cameraMoveScript;
    StationLabelUI m_ui;
    string m_dataAsString;
    bool m_hasData = false;
    bool m_hasBeenClicked = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        m_cameraMoveScript = FindObjectOfType<CameraMovement>();
        m_ui = FindObjectOfType<StationLabelUI>();

    }

    void Update()
    {
        if (m_stationData != null)
        {
            rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, Mathf.PingPong(Time.time, m_stationData.waveHeight));
        }
    }

    // The mesh goes red when the mouse is over it...
    void OnMouseEnter()
    {
        if (!m_hasData)
            MakeStringData();

        rend.material.color = Color.red;
        if (m_ui != null)
            m_ui.UpdateUI(m_dataAsString);
    }

    // ...the red fades out to cyan as the mouse is held over...
    void OnMouseOver()
    {
        rend.material.color -= new Color(0.1F, 0, 0) * Time.deltaTime;
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, this.transform.position, Time.deltaTime);
    }

    // ...and the mesh finally turns white when the mouse moves away.
    void OnMouseExit()
    {
        if (m_hasBeenClicked)
            rend.material.color = m_clickedColor;
        else
            rend.material.color = m_stationColor;
        // if (m_cameraMoveScript != null)
        //     m_cameraMoveScript.m_isReturning = true;
    }

    void OnMouseDown()
    {
        if (m_cameraMoveScript != null)
            m_cameraMoveScript.ZoomToThisPoint(this.transform.position - (Vector3.forward * .2f));
        m_hasBeenClicked = true;
    }

    void MakeStringData()
    {
        m_dataAsString = "Station: " + m_stationData.station;
        m_dataAsString += "\nWind Speed: " + m_stationData.windSpeed;
        m_dataAsString += "\nWind Gust: " + m_stationData.windGust;
        m_dataAsString += "\nWind Direction: " + m_stationData.windDirection;
        m_dataAsString += "\nWave Height: " + m_stationData.waveHeight;
        m_dataAsString += "\nDate: " + m_stationData.month + "/" + m_stationData.day + " " + m_stationData.hh + ":" + m_stationData.mm;
        m_hasData = true;

    }
}
