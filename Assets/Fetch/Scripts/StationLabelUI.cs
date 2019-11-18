using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationLabelUI : MonoBehaviour
{
    public Text m_textUI;


    public void UpdateUI(string txt)
    {
        if (m_textUI != null)
            m_textUI.text = txt;
    }
}
