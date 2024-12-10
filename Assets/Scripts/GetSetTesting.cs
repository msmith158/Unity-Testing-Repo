using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetSetTesting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private int number1 = 1;
    [SerializeField] private int number2 = 2;
    [SerializeField] private int number3 = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Text.text = "Number 1: " + number1 + "\nNumber 2: " + number2 + "\nNumber 3: " + number3;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_Text.text = "Number 1: " + (firstNumber++) + "\nNumber 2: " + number2 + "\nNumber 3: " + number3;
        }
    }

    private int firstNumber
    {
        get { Debug.Log("Accessing firstNumber");
            return number1;
        }
        set
        {
            Debug.Log("Setting firstNumber"); 
            number1 = value; 
            number2 = value;
            number3 = value;
        }
    }
}
