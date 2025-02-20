using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTestController : MonoBehaviour
{
    [Header("Values")] 
    public bool CanMove = true;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float sprintModifier = 2f;
    private float currentSpeed;
    
    [Header("Object References")]
    private GameObject playerCube;
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCube = this.gameObject;
        rb = GetComponent<Rigidbody>();
        currentSpeed = playerSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            // Holding down the right button
            if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
                else rb.velocity = new Vector3(currentSpeed, rb.velocity.y, 0);
            }

            // Holding down the left button
            if (Input.GetKey(KeyCode.A))
            {
                if (Input.GetKey(KeyCode.D))
                {
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
                else rb.velocity = new Vector3(-currentSpeed, rb.velocity.y, 0);
            }

            // Holding down the Shift key to sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = playerSpeed * sprintModifier;
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = playerSpeed;
            }

            // Letting go of either key
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
        else if (!CanMove && rb.velocity.x != 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}
