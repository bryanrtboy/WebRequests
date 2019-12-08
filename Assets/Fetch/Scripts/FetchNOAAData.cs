//Bryan Leister
//12.10.2018
//
//This script uses the built-in networking of Unity to download a text file from NOAA and parse the contents
//into usable values for a Unity application.
//As a stub, this could be used to fetch any web page and grab data if it is in a consistent format.

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;

public class FetchNOAAData : MonoBehaviour
{
    [Tooltip("The url address of your data for the latest observations")]
    public string m_latestObservationURL = "https://www.ndbc.noaa.gov/data/latest_obs/latest_obs.txt";
    [Tooltip("Table for stations with additional information")]
    public string m_stationDetailsURL = "https://www.ndbc.noaa.gov/data/stations/station_table.txt";
    public TextAsset m_backupLatestObs;
    public TextAsset m_backupStationData;
    public bool m_getDetails = false;
    [Tooltip("Drag a Text UI object here to display results in the scene, leave null if you don't want to show on screen")]
    public Text m_textUI;

    //Not visible in the Editor
    public List<NOAA_latest_observations> m_stations { get; private set; }
    public Dictionary<string, NOAA_station_table> m_stationDetails { get; private set; }
    public string m_observationRawText { get; private set; }
    public string m_detailsRawText { get; private set; }
    [HideInInspector]
    public bool m_dataFetchingComplete = false;


    ///<summary>
    /// Call this from an inherited class to grab the raw text file
    ///</summary>
    public void FetchRawData(string url) => StartCoroutine(GetLatestObservations());

    public void DisplayRandomStation()
    {
        if (m_stations == null)
        {
            Debug.Log("Data has not been fetched! Nothing to see here...");
            return;
        }

        if (m_textUI != null)
        {

            NOAA_latest_observations m_stationData = m_stations[UnityEngine.Random.Range(0, m_stations.Count)];
            DateTime m_date = DateTime.Parse(m_stationData.year + "-" + m_stationData.month + "-" + m_stationData.day + " " +
m_stationData.hh + ":" + m_stationData.mm);
            string m_dataAsString = "";
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
            m_textUI.text = m_dataAsString;
        }
    }

    IEnumerator GetLatestObservations()
    {
#if UNITY_WEBGL
m_observationRawText = m_backupLatestObs.text;
yield return new WaitForSeconds(1);
#else
        using (UnityWebRequest www = UnityWebRequest.Get(m_latestObservationURL))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                m_observationRawText = www.downloadHandler.text;
                ParseLatestObservations();
            }
        }

#endif

    }

    IEnumerator GetStationDetails()
    {
#if UNITY_WEBGL
m_detailsRawText = m_backupStationData.text;
yield return new WaitForSeconds(1);
#else
        using (UnityWebRequest www = UnityWebRequest.Get(m_stationDetailsURL))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                m_detailsRawText = www.downloadHandler.text;
                ParseStationTable();
            }
        }

#endif

    }

    void ParseLatestObservations()
    {

        string[] lines = m_observationRawText.Split('\n');

        m_stations = new List<NOAA_latest_observations>();

        //The text file does not seem to use tabs, so use any of these to separate the fields...
        string[] separators = { " ", "  ", "   ", "     " };

        //Skip the first two lines because they are headers
        for (int i = 2; i < lines.Length; i++)
        {
            NOAA_latest_observations n = new NOAA_latest_observations();
            List<string> data = lines[i].Split(separators, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            if (data.Count < 1)
                continue;
            n.id = data[0];
            n.latitude = float.Parse(data[1]);
            n.longitude = float.Parse(data[2]);
            n.year = int.Parse(data[3]);
            n.month = int.Parse(data[4]);
            n.day = int.Parse(data[5]);
            n.hh = int.Parse(data[6]);
            n.mm = int.Parse(data[7]);
            int.TryParse(data[8], out n.windDirection);
            float.TryParse(data[9], out n.windSpeed);
            float.TryParse(data[10], out n.windGust);
            float.TryParse(data[11], out n.waveHeight);
            float.TryParse(data[15], out n.pressure);
            float.TryParse(data[17], out n.airTemperature);
            float.TryParse(data[18], out n.waterTemperature);

            m_stations.Add(n);
        }

        Debug.Log("Parsed the data: " + m_stations.Count.ToString() + " stations were found in the data.");

        if (!m_getDetails)
            m_dataFetchingComplete = true;
        else
            StartCoroutine(GetStationDetails());

        if (m_textUI != null)
        {
            string s = "Nothing found";

            if (m_stations.Count > 0)
            {
                int rand = UnityEngine.Random.Range(0, m_stations.Count);
                NOAA_latest_observations _station = m_stations[rand];
                s = "Station: " + _station.id + "\nAir Temp: " + ((_station.airTemperature * 9 / 5) + 32) + "° F\nWater temp: " + ((_station.waterTemperature * 9 / 5) + 32) + "° F\nLast updated: " + _station.year + "." +
                _station.month + "." + _station.day + " " + _station.hh + ":" + _station.mm.ToString("00") + " GMT\nLongitude: " + _station.longitude + "\nLatitude: " + _station.longitude;
            }

            m_textUI.text = "Found " + m_stations.Count + " bouy stations. \n\n" + s;
        }
    }

    void ParseStationTable()
    {

        string[] lines = m_detailsRawText.Split('\n');

        m_stationDetails = new Dictionary<string, NOAA_station_table>();

        string[] separators = { "|" };

        //Skip the first two lines because they are headers
        for (int i = 2; i < lines.Length; i++)
        {
            NOAA_station_table n = new NOAA_station_table();
            List<string> data = lines[i].Split(separators, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            if (data.Count < 1)
                continue;
            n.id = data[0];
            n.owner = data[1];
            n.type = data[2];
            n.hull = data[3];
            n.name = data[4];
            if (data.Count > 5)
                n.payload = data[5];
            if (data.Count > 6)
                n.location = data[6];
            if (data.Count > 7)
                n.timezone = data[7];
            if (data.Count > 8)
                n.forecast = data[8];
            if (data.Count > 9)
                n.note = data[9];

            m_stationDetails.Add(n.id, n);
        }

        Debug.Log("Parsed the station table: " + m_stationDetails.Count.ToString() + " stations were found in the data.");


        //
        if (m_stations.Count > 0 && m_stationDetails.Count > 0)
        {
            foreach (NOAA_latest_observations s in m_stations)
            {
                if (m_stationDetails.ContainsKey(s.id))
                {
                    s.details = m_stationDetails[s.id];
                }

            }
        }

        m_dataFetchingComplete = true;

    }

}

//https://www.ndbc.noaa.gov/data/latest_obs/latest_obs.txt
[System.Serializable]
public class NOAA_latest_observations
{
    public string id;
    public float latitude;
    public float longitude;
    public int year;
    public int month;
    public int day;
    public int hh;
    public int mm;
    public int windDirection;
    public float windSpeed;
    public float windGust;
    public float waveHeight;
    public float pressure;
    public float airTemperature;
    public float waterTemperature;
    public bool airIsEstimated = false;
    public bool waterIsEstimated = false;
    public NOAA_station_table details;
}

//https://www.ndbc.noaa.gov/data/stations/station_table.txt
[System.Serializable]
public class NOAA_station_table
{
    public string id;
    public string owner;
    public string type;
    public string hull;
    public string name;
    public string payload;
    public string location;
    public string timezone;
    public string forecast;
    public string note;
}




