using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private bool rotationON = false;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private Vector3 rotationVector;

    private void Update() 
    {
        if (!rotationON) { return; }

        transform.Rotate(rotationVector * (rotationSpeed * Time.deltaTime));
    }
}
