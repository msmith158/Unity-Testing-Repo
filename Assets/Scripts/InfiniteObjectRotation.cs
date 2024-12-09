using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteObjectRotation : MonoBehaviour
{
    private GameObject cube;
    [SerializeField] private Vector3 rotationSpeed;
    [SerializeField] private float rotationMagnitude = 1f;
    [SerializeField] private Vector3 primitiveOffset;

    void Start()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = primitiveOffset;
        cube.transform.localScale = new Vector3(3, 1, 1);
    }
    void Update()
    {
        cube.transform.Rotate(rotationSpeed.x * rotationMagnitude, rotationSpeed.y * rotationMagnitude, rotationSpeed.z * rotationMagnitude);
    }
}
