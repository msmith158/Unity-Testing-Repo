using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteObjectRotation : MonoBehaviour
{
    [SerializeField] private Transform objectToRotate;
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private float rotationMagnitude = 1f;

    // Update is called once per frame
    void Update()
    {
        objectToRotate.Rotate(rotationSpeed.x * rotationMagnitude, rotationSpeed.y * rotationMagnitude, rotationSpeed.z * rotationMagnitude);
    }
}
