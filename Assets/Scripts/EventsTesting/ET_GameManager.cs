using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ET_GameManager : MonoBehaviour
{
    // Defining the event
    public static event Action OnGameOver;
    
    public void EndGame()
    {
        OnGameOver?.Invoke();
    }
}
