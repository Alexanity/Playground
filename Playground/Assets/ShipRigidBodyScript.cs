using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ShipRigidBodyScript : MonoBehaviour
{
    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float yawTorque = 500f; // left and right
    [SerializeField]
    private float pitchTorque = 1000f; // up and down movement / pulling the nose of the ship up and down
    [SerializeField]
    private float rollTorque = 1000f; // just like barrel roll!
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f; // how fast we go up and down
    [SerializeField]
    private float strafeThrust = 50f; // how fast we go left and right
    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;   
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;        // How fast we slow down
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;

    Rigidbody rb;

    //Input Values
    private float thrust1D;
    private float strafe1D;
    private float upDown1D;
    private float roll1D;
    private Vector2 pitchYaw;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() // because we use physics and we want it to be independent from the frame rate
    {
        HandleMovement();   
    }

    void HandleMovement()
    {
        //  Roll             // Vector3.back means it'll roll around the X or in this case the forward axis
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);                              // when not pressed the value is 0
        // Pitch            // it adds Torque around the right Axis
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        // Yawn
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * pitchTorque* Time.deltaTime);
    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context) // context = the button -> so when you press the button it reads the value
    {                                          // Works with both keyboard and controllers
        thrust1D = context.ReadValue<float>(); // if we press W the value will be set to 1, if we press S the value will be set to -1
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }
    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }
    #endregion
}
