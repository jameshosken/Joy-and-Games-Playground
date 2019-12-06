using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    [SerializeField] CameraLookController cameraController;
    [SerializeField] ShipControlsHandler shipControls;

    [SerializeField] Transform rightHandController;
    [SerializeField] Transform controllerSpaceReference;
    [SerializeField] Transform rightJoystick;

    [SerializeField] AnimationCurve pitchRollCurve;
    [SerializeField] float torqueMultiplier = 5f;

    [SerializeField] float accelMultiplier = .1f;

    [SerializeField] bool isVR = true;
    bool isCameraLocked;

    [Tooltip("Controlled Status")]
    [SerializeField] GameObject engaged;
    [SerializeField] GameObject disengaged;


    ShipShakeHandler shipShake;
    void Start()
    {
        shipShake = FindObjectOfType<ShipShakeHandler>();
        //rControllerReference = new GameObject("r Controller Ref");
        //rControllerReference.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote);
        //rControllerReference.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);

        disengaged.SetActive(true);
        engaged.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        if (isVR)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))

        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            cameraController.SetHoming(false);
        }

        if (Input.GetMouseButtonUp(0))
        {
            cameraController.SetHoming(true);
        }

        if (Input.GetMouseButton(0) && !isCameraLocked)
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            cameraController.RotateCamera(new Vector3(x, y, 0));

        }


    }

    private void HandleVRControls()
    {

        

        if (OVRInput.Get(OVRInput.Button.One))
        {
            shipControls.body.angularVelocity *= 0.95f;
        }

        //IF CONTROLLER RELEASED
        if (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            rightHandController.rotation = controllerSpaceReference.rotation;

            if (engaged.activeSelf)
            {
                disengaged.SetActive(true);
                engaged.SetActive(false);
            }
        }
        else
        {
            //(If pressed)
            if (disengaged.activeSelf)
            {
                disengaged.SetActive(false);
                engaged.SetActive(true);
            }
        }

       



        //Quaternion rRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        //Quaternion relativeRot = Quaternion.Inverse(rRot) * rControllerReference.transform.localRotation;

        Vector2 pad1 = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        //Vector3 torque = Quaternion.FromToRotation(rightHandController.forward, controllerSpaceReference.forward).eulerAngles;


        Vector3 pitchPlaneNormal = controllerSpaceReference.right;

        Vector3 rollPlaneNormal = controllerSpaceReference.forward;


        Vector3 pitchProjection = Quaternion.Inverse(controllerSpaceReference.rotation) * Vector3.ProjectOnPlane(rightHandController.up, pitchPlaneNormal);

        Vector3 rollProjection = Quaternion.Inverse(controllerSpaceReference.rotation) * Vector3.ProjectOnPlane(rightHandController.up, rollPlaneNormal); ;

        Vector3 pitch = Vector3.Cross(Vector3.up, pitchProjection);

        Vector3 roll = Vector3.Cross(Vector3.up, rollProjection);

        float pitchPower = pitchRollCurve.Evaluate(pitch.magnitude);
        float rollPower = pitchRollCurve.Evaluate(roll.magnitude);

        pitch *= pitchPower;
        roll *= rollPower;

        Vector3 joystickPosition = pitch + roll;


        rightJoystick.localEulerAngles = joystickPosition * 45;

        Vector3 yawPlaneNormal = rightJoystick.up;
        Vector3 yawProjection = Quaternion.Inverse(rightJoystick.rotation) * Vector3.ProjectOnPlane(rightHandController.forward, yawPlaneNormal); ;
        Vector3 yaw = Vector3.Cross(Vector3.forward, yawProjection);


        float yawPower = pitchRollCurve.Evaluate(yaw.magnitude);
        yaw *= yawPower;

        rightJoystick.Rotate(yaw * 45, Space.Self);

        shipControls.ApplyRelativeTorque(pitch * torqueMultiplier);

        shipControls.ApplyRelativeTorque(roll * torqueMultiplier);

        shipControls.ApplyRelativeTorque(yaw * torqueMultiplier);

        

        float fwdThrust = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) ? 1 : 0;

        if (fwdThrust == 1)
        {
            shipShake.SetIsShaking(true);
        }else { 
            shipShake.SetIsShaking(false);
        }

        Vector3 thrust = new Vector3(pad1.x, pad1.y, fwdThrust * 2);
        shipControls.ApplyRelativeThrust(thrust * accelMultiplier);


        if (Time.frameCount % 120 == 0)
        {
            OVRInput.SetControllerVibration(.5f, joystickPosition.magnitude, OVRInput.Controller.RTrackedRemote);
        }

        Debug.Log("AMOUNT: " + pitchPower);
        Debug.Log("ROTATION: " + joystickPosition);

    }

    void HandlePCControls()
    {


        if (Input.GetMouseButtonUp(1))
        {

        }


        //float roll = Input.GetAxis("Mouse X") * -1;
        //float pitch = Input.GetAxis("Mouse Y");



        float yaw = Input.GetAxisRaw("Vertical");
        float pitch = Input.GetAxisRaw("Horizontal");

        float roll = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            roll = 1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            roll = -1;
        }

        shipControls.ApplyRelativeTorque(new Vector3(yaw, pitch, roll));


        float up = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
        float fwd = Input.GetKey(KeyCode.Space) ? 5 : 0;

        shipControls.ApplyRelativeThrust(new Vector3(0, up, fwd));

    }


    private void FixedUpdate()
    {

        if (isVR)
        {
            HandleVRControls();
        }
        else
        {
            HandlePCControls();
        }

        
    }
}
