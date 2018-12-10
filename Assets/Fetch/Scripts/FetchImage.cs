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
    public string m_url = "https://www.ndbc.noaa.gov/buoycam.php?station=46059";

    MeshRenderer m_renderer;

    void Awake()
    {
        m_renderer = this.GetComponent<MeshRenderer>();
    }

    void Start()
    {
        StartCoroutine(GetTexture());
    }


    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(m_url);
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

}
