using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ET_TriggerForEvent : MonoBehaviour
{
    [SerializeField] private ET_GameManager gameManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SimpleTestController>())
        {
            gameManager.EndGame();
        }
    }
}
