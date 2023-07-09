using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float maxX, minX;
    [SerializeField] private float moveSpeed = 2f;

    private bool movingRight = true;

    private void Update() 
    {
        Vector3 currentPos = transform.position;
        
        currentPos += Vector3.right * (Time.deltaTime * moveSpeed) * ((movingRight) ? 1 : -1f);

        if(movingRight && currentPos.x >= maxX)
        {
            movingRight = false;
        }
        else if(!movingRight && currentPos.x <= minX)
        {
            movingRight = true;
        }

        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);

        transform.position = currentPos;
    }
}
