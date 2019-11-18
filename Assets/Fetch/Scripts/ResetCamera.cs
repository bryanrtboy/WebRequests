using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCamera : MonoBehaviour
{
    CameraMovement m_cameraMoveScript;

    void Start()
    {
        m_cameraMoveScript = FindObjectOfType<CameraMovement>();
    }

    void OnMouseDown()
    {
        if (m_cameraMoveScript != null)
        {
            m_cameraMoveScript.m_zoomingToPoint = false;
            m_cameraMoveScript.m_isReturning = true;
        }
    }
}
