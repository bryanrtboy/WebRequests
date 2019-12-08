using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool m_isReturning = false;
    public bool m_zoomingToPoint = false;

    Vector3 m_targetPoint;
    Vector3 m_startPosition;

    void Start()
    {
        m_startPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isReturning)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, m_startPosition, Time.deltaTime);
            if (Vector3.Distance(this.transform.position, m_startPosition) < .01f)
                m_isReturning = false;
        }

        if (m_zoomingToPoint)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, m_targetPoint, Time.deltaTime);
            if (Vector3.Distance(this.transform.position, m_targetPoint) < .01f)
                m_zoomingToPoint = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_isReturning = true;
            m_zoomingToPoint = false;
        }

    }

    public void ZoomToThisPoint(Vector3 target)
    {
        m_isReturning = false;
        m_targetPoint = target;
        m_zoomingToPoint = true;
    }
}
