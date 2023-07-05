using UnityEngine;

public class SwerveInput : MonoBehaviour
{
    float lastFingerPosX;
    private float moveOnX;
    [SerializeField] private float maxMoveOnXValue = 10f;
    [HideInInspector] public float changeOnX { get { return moveOnX; } }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {

            lastFingerPosX = Input.mousePosition.x;

        }
        else if (Input.GetMouseButton(0))
        {
            moveOnX = Input.mousePosition.x - lastFingerPosX;
            lastFingerPosX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {

            moveOnX = 0f;

        }

        moveOnX = Mathf.Clamp(moveOnX, -maxMoveOnXValue, maxMoveOnXValue);

    }
}
