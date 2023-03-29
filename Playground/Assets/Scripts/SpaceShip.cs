using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class SpaceShip : MonoBehaviour
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

    [Header("=== Boost Settings ===")]
    [SerializeField]
    private float maxBoostAmount = 2f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostReachargeRate = 0.5f;
    [SerializeField]
    private float boostMultipier = 5f;
    public bool boosting = false;
    public float currentBoostAmount;
    [SerializeField]
    private CinemachineVirtualCamera shipCam;

    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;   
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;        // How fast we slow down
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    float glide, verticalGlide, horizontalGlide = 0f;

    Rigidbody rb;

    //Input Values
    private float thrust1D;
    private float strafe1D;
    private float upDown1D;
    private float roll1D;
    private Vector2 pitchYaw;
    private bool isbrake;

    private bool isOccupied = true;

    private ZeroGMovement player;

    public delegate void OnRequestShipExit();
    public event OnRequestShipExit onRequestShipExit;

    void Start()
    {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount; // player starts with boost
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ZeroGMovement>();
        
        if (player != null) { print("Player found"); }

        player.onRequestShipEntry += PlayerEnteredShip; // assinging the function onRequestEntry to the PlayerEnteredShip method

    }
    private void OnEnable()
    {
       if (shipCam != null)
        {
            CameraSwitcher.Register(shipCam);
        }
        else
        {
            Debug.LogError("Ship Camera Not Assigned");
        } 
    }
    private void OnDisable()
    {
        if(shipCam != null)
        {
            CameraSwitcher.UnRegister(shipCam);
        }
    }

    void FixedUpdate() // because we use physics and we want it to be independent from the frame rate
    {
        if (isOccupied)
        {
            HandleMovement();
            HandleBoosting();
        }
        
    }
    void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f){ // if we have any boost left
            currentBoostAmount -= boostDeprecationRate; // decrease boosting
            if(currentBoostAmount <= 0f)
            {
                boosting = false; // if the tank (with the boost) hits zero we set it to false
            }
        }
        else
        {
            if(currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostReachargeRate; // replenishing the boost in the tank
            }
        }
    }
    void HandleMovement()
    {
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        // THRUST
        if(thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;

            if (boosting)
            {
                currentThrust = thrust * boostMultipier;
            }
            else
            {
                currentThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust; 
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }
        // UP/DOWN
        if (upDown1D > 0.1f || upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.deltaTime);
            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.deltaTime);
            verticalGlide *= upDownGlideReduction;
        }
        //STARFING
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.deltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.deltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
    }
    void PlayerEnteredShip()
    {
        rb.isKinematic = false;
        CameraSwitcher.SwitchCamera(shipCam);
        isOccupied = true;
    }
    void PlayerExitedShip()
    {
        rb.isKinematic = true;
        isOccupied = false;
        if (onRequestShipExit != null) { onRequestShipExit(); }

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
    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }
    public void onInteract(InputAction.CallbackContext context)
    {
        if (isOccupied && context.action.triggered)
        {
            PlayerExitedShip();
        }
    }
    #endregion
}
