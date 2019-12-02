using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

[RequireComponent(typeof(MeshRenderer))]
public class FetchImage : MonoBehaviour
{
    //Bouy cams can be found on the NOAA bouycam map - https://www.ndbc.noaa.gov/buoycams.shtml
    public string m_baseUrl = "https://www.ndbc.noaa.gov/buoycam.php?station=";
    string m_fetchUrl;
    string m_stationId = "46059";
    List<string> m_bouyCamIDs;

    MeshRenderer m_renderer;

    void Awake()
    {
        m_renderer = this.GetComponent<MeshRenderer>();

        //Initialize the list so it exists
        m_bouyCamIDs = new List<string>();

        //Populate the list with id numbers that work
        m_bouyCamIDs.Add(m_stationId);
        m_bouyCamIDs.Add("51002");

        //Run a function to grab a URL
        FetchANewID();

    }

    void Start()
    {
        StartCoroutine(GetTexture());
    }


    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(m_fetchUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            m_renderer.material.mainTexture = myTexture;
            this.transform.localScale = new Vector3(myTexture.width / myTexture.height, 1, 1);

        }
    }

    //This function runs at the start up of the level, and it can be hooked up to a Button.
    public void FetchANewID()
    {
        //Right now, it will always select only the first ID string in the list... 
        //Need to make it pick a random id
        m_fetchUrl = m_baseUrl + m_bouyCamIDs[0];

    }

}
