using UnityEngine;

public class PingPongPaddle : MonoBehaviour
{
    [Header("Control Point Movement")]
    public Transform controlPoint;
    public float cpHorizontalMoveRate;
    public float cpVerticalMoveRate;

    [Header("Auto Paddle Rotation")]
    public Vector2 autoRotateMaxAngle; //x is the rotation when moving left to right, and y is the rotation moving front to back
    public float autoRotateMaxDistance;
    private Vector2 normalizedAutoRotateVal; //two numbers from 0-1 that represents the percent along the maxDistance we've moved the paddle on each axis

    [Header("Paddle Flick")]
    public HingeJoint handleHinge;
    public float handleRestingPosition;
    public float handleFlexedPosition;

    private void Start()
    {
        JointSpring spring = handleHinge.spring;
        spring.targetPosition = handleRestingPosition;
        handleHinge.spring = spring;
    }

    void Update()
    {
        UpdatePaddleFlick();
        UpdatePaddlePosition();
        UpdatePaddleRotation();
    }

    //move the paddle up and down based on input
    private void UpdatePaddleFlick()
    {
        if (Input.GetMouseButton(0))
            SetHandleAngle(handleFlexedPosition);
        else
            SetHandleAngle(handleRestingPosition);
    }

    private void UpdatePaddlePosition()
    {
        //move the paddle side to side
        controlPoint.position += new Vector3(-Input.mousePositionDelta.x * cpHorizontalMoveRate, 0, 0);

        //move the paddle forward and back
        controlPoint.position += new Vector3(0, 0, -Input.mousePositionDelta.y * cpVerticalMoveRate);
    }

    //rotate the paddle as we move side to side
    private void UpdatePaddleRotation()
    {
        normalizedAutoRotateVal.x = Mathf.Clamp(this.transform.position.x / autoRotateMaxDistance, -1, 1);
        normalizedAutoRotateVal.y = Mathf.Clamp(this.transform.position.z / autoRotateMaxDistance, -1, 1);

        float leftRightRot = normalizedAutoRotateVal.x * autoRotateMaxAngle.x;
        float forwardBackRot = -normalizedAutoRotateVal.y * autoRotateMaxAngle.y;

        controlPoint.transform.rotation = Quaternion.Euler(forwardBackRot, 0, leftRightRot);
    } 

    private void SetHandleAngle(float angle)
    {
        JointSpring spring = handleHinge.spring;
        spring.targetPosition = angle;
        handleHinge.spring = spring;
    }
}
