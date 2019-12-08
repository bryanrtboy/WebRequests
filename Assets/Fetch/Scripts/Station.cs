using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Station : MonoBehaviour
{

    public NOAA_latest_observations m_stationData;
    public Color m_stationColor = Color.grey;
    public Color m_overColor = Color.red;
    public Color m_clickedColor = Color.blue;

    Renderer rend;
    CameraMovement m_cameraMoveScript;
    StationLabelUI m_ui;
    string m_dataAsString;
    bool m_hasData = false;
    bool m_hasBeenClicked = false;
    Vector3 m_startPosition;
    DateTime m_date;

    void Start()
    {
        rend = GetComponent<Renderer>();
        m_cameraMoveScript = FindObjectOfType<CameraMovement>();
        m_ui = FindObjectOfType<StationLabelUI>();
        float s = m_stationData.waveHeight * .01f;

        if (s < .03f)
            s = .03f;
        else if (s > .3f)
            s = .3f;

        //this.transform.localScale = Vector3.one * s;
        m_startPosition = this.transform.position;

        m_date = DateTime.Parse(m_stationData.year + "-" + m_stationData.month + "-" + m_stationData.day + " " +
        m_stationData.hh + ":" + m_stationData.mm);
        this.transform.localEulerAngles = new Vector3(0, 0, m_stationData.windDirection);
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
        rend.material.color -= new Color(0.01F, 0, 0, 0) * Time.deltaTime;
    }

    // ...and the mesh finally turns white when the mouse moves away.
    void OnMouseExit()
    {
        if (m_hasBeenClicked)
            rend.material.color = m_clickedColor;
        else
            rend.material.color = m_stationColor;

    }

    void OnMouseDown()
    {
        if (m_cameraMoveScript != null)
            m_cameraMoveScript.ZoomToThisPoint(this.transform.position - (Vector3.forward * .7f));
        m_hasBeenClicked = true;
    }

    void MakeStringData()
    {
        m_dataAsString += "Station: " + m_stationData.id;
        m_dataAsString += "\n" + m_date.ToLocalTime().ToString();
        m_dataAsString += "\n\nWind Speed: " + m_stationData.windSpeed;
        m_dataAsString += "\nWind Gust: " + m_stationData.windGust;
        m_dataAsString += "\nWind Direction: " + m_stationData.windDirection;
        m_dataAsString += "\nWave Height: " + m_stationData.waveHeight;

        if (m_stationData.details != null)
        {
            m_dataAsString += "\n\nName: " + m_stationData.details.name;
            m_dataAsString += "\nLocation: " + m_stationData.details.location;
            m_dataAsString += "\nNote: " + m_stationData.details.note;
            m_dataAsString += "\nPayload: " + m_stationData.details.payload;

        }

        m_hasData = true;

    }
}
