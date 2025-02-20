using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ET_UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverGroup;
    [SerializeField] private SimpleTestController player;

    private void Start()
    {
        if (gameOverGroup.activeInHierarchy)
            gameOverGroup.SetActive(false);
    }

    // Subscribe to the event
    private void OnEnable()
    {
        ET_GameManager.OnGameOver += ShowGameOverScreen;
    }

    // Unsubscribe to avoid memory leaks
    private void OnDisable()
    {
        ET_GameManager.OnGameOver -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        gameOverGroup.SetActive(true);
        player.CanMove = false;
    }
}
