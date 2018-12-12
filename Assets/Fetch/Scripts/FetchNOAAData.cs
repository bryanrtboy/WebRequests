//Bryan Leister
//12.10.2018
//
//This script uses the built-in networking of Unity to download a text file from NOAA and parse the contents
//into usable values for a Unity application.
//As a stub, this could be used to fetch any web page and grab data if it is in a consistent format.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;

public class FetchNOAAData : MonoBehaviour
{
    [Tooltip("The url address of your data")]
    public string m_url = "https://www.ndbc.noaa.gov/data/latest_obs/latest_obs.txt";
    [Tooltip("Drag a Text UI object here to display results in the scene, leave null if you don't want to show on screen")]
    public Text m_textUI;

    //Not exposed in the editor
    public List<NOAAStations> m_stations { get; private set; }

    public string m_text { get; private set; }



    public void RunNOAAScript()
    {
        StartCoroutine(GetTheData());
    }

    IEnumerator GetTheData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(m_url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                m_text = www.downloadHandler.text;
            }
        }

        if (m_text != null)
        {
            Debug.Log("Got the text!");

            if (m_textUI != null)
                m_textUI.text = "Got everything, click 'Read text' to display results";
        }


    }

    public void ReadTextFile()
    {
        if (m_text == null)
        {
            Debug.LogError("No Text file to read!");
            return;
        }

        string[] lines = m_text.Split('\n');

        m_stations = new List<NOAAStations>();

        //The text file does not seem to use tabs, so use any of these to separate the fields...
        string[] separators = { " ", "  ", "   ", "     " };

        //Skip the first two lines because they are headers
        for (int i = 2; i < lines.Length; i++)
        {
            NOAAStations n = new NOAAStations();
            List<string> data = lines[i].Split(separators, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            if (data.Count < 1)
                continue;
            n.station = data[0];
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

        if (m_textUI != null)
        {
            string s = "Nothing found";

            if (m_stations.Count > 0)
                s = "Station: " + m_stations[0].station + "\nAir Temp: " + ((m_stations[0].airTemperature * 9 / 5) + 32) + "° F\nWater temp: " + ((m_stations[0].waterTemperature * 9 / 5) + 32) + "° F\nLast updated: " + m_stations[0].year + "." +
                m_stations[0].month + "." + m_stations[0].day + " " + m_stations[0].hh + ":" + m_stations[0].mm.ToString("00") + " GMT";

            m_textUI.text = "Found " + m_stations.Count + " bouy stations. \n\n" + s;
        }
    }



}

[System.Serializable]
public class NOAAStations
{
    public string station;
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
}



