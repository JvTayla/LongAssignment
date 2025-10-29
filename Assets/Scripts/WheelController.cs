using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    //public Rigidbody rb;
    //public Transform boatTransform;

    [SerializeField] private Rigidbody boatRb;

    public float acceleration = 500f;
    public float breakingForce = 300f;
    public float maxTurnAngle = 15f;

    [SerializeField] private float currentAcceleration = 0f;
    [SerializeField] private float currentBreakForce = 0f;
    [SerializeField] private float currentTurnAngle = 0f;

    private void FixedUpdate()
    {
        //Get forward/reverse acceleration from the vertical axis (W and S keys)
        currentAcceleration = acceleration * Input.GetAxis("Vertical");

        if (currentAcceleration == 0)
        {
            boatRb.drag = 0.5f;
            currentBreakForce = breakingForce;
        }
        else
        {
            boatRb.drag = 0f;
        }

        //if we're pressing keycode, give currentBreakingForce a value
        if (Input.GetKey(KeyCode.E))
        {
            currentBreakForce = breakingForce;
        }
        else
        {
            currentBreakForce = 0f;
        }

        //Apply acceleration to back wheels
        backRight.motorTorque = currentAcceleration;
        backLeft.motorTorque = currentAcceleration;

        //Apply braking force to all wheels
        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;

        //take care of the steering
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    // Start is called before the first frame update
    void Start()
    {
        //rb.centerOfMass += new Vector3(0, -1, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
