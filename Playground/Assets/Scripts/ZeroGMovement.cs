using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class ZeroGMovement : MonoBehaviour
{
    [Header("=== Player Movement Settings ===")]
    //[SerializeField]
    //private float yawTorque = 500f; // left and right
    //[SerializeField]
    //private float pitchTorque = 1000f; // up and down movement / pulling the nose of the ship up and down
    [SerializeField]
    private float rollTorque = 1000f; // just like barrel roll!
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f; // how fast we go up and down
    [SerializeField]
    private float strafeThrust = 50f; // how fast we go left and right

    [SerializeField]
    private CinemachineVirtualCamera playerCam;

    private Camera mainCam;

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

    public SpaceShip ShipToEnter;

    public delegate void OnRequestShipEntry();
    public event OnRequestShipEntry onRequestShipEntry;
    void Start()
    {
        mainCam = Camera.main; // moving in relation to the camera
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        currentBoostAmount = maxBoostAmount; // player starts with boost 
        ShipToEnter = null; // making sure player can't enter the ship without being in the interaction zone
        if(playerCam != null)
        {
            CameraSwitcher.Register(playerCam);
        }
        else
        {
            Debug.LogError("Player Camera Not Assigned");
        }
    }
    private void OnDisable()
    {
        if (playerCam != null)
        {
            CameraSwitcher.UnRegister(playerCam);
        }
    }

    void FixedUpdate() // because we use physics and we want it to be independent from the frame rate
    {
        HandleMovement();
        HandleBoosting();
    }

    void EnterShip()
    {
        transform.parent = ShipToEnter.transform;
        this.gameObject.SetActive(false); // hides player

        if(onRequestShipEntry != null) { 
            onRequestShipEntry();
        }
    }
    void ExitShip()
    {
        transform.parent = null;
        this.gameObject.SetActive(true); // shows the player again
        CameraSwitcher.SwitchCamera(playerCam);
    }
    public void AssignShip(SpaceShip ship)
    {
        ShipToEnter = ship;
        if(ShipToEnter != null) { ShipToEnter.onRequestShipExit += ExitShip; } // += subscribe the method
    }
    public void RemoveShip()
    {
        ShipToEnter.onRequestShipExit -= ExitShip; // unsubscribe the method
        ShipToEnter = null;
    }

    void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f)
        { // if we have any boost left
            currentBoostAmount -= boostDeprecationRate; // decrease boosting
            if (currentBoostAmount <= 0f)
            {
                boosting = false; // if the tank (with the boost) hits zero we set it to false
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostReachargeRate; // replenishing the boost in the tank
            }
        }
    }
    void HandleMovement()
    {
        rb.AddRelativeForce(-mainCam.transform.forward * roll1D * rollTorque * Time.deltaTime);

        // THRUST
        if (thrust1D > 0.1f || thrust1D < -0.1f)
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

            rb.AddForce(mainCam.transform.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(mainCam.transform.forward * glide * Time.deltaTime);
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
            rb.AddForce(mainCam.transform.right * strafe1D * upThrust * Time.deltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            rb.AddForce(mainCam.transform.right * horizontalGlide * Time.deltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
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
    //public void OnPitchYaw(InputAction.CallbackContext context)
    //{
    //    pitchYaw = context.ReadValue<Vector2>();
    //}
    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
        //FindObjectOfType<AudioManager>().Play("boost");
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(ShipToEnter!=null && context.action.triggered)
        {
            EnterShip();
        }
        
    }

    #endregion
}
