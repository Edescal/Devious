using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Quit game...");
        Application.Quit();
    }

    public void Options()
    {
        Debug.Log("Options...");
    }

    public void Credits()
    {
        Debug.Log("Credits...");
    }
}