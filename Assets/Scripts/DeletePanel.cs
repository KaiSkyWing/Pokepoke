using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePanel : MonoBehaviour
{
    [SerializeField] private GameObject youCanOpenPanel;

    public void HidePanel()
    {
        Debug.Log("a");
        youCanOpenPanel.gameObject.SetActive(false);
    }
}
