using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    //new input
    private PlayerControls controls;
    private PlayerInput playerInput;
    private InputAction steerAction;
    private InputAction accelerateAction;
    private InputAction driftAction;
    private InputAction glideAction;
    private InputAction brakeReverseAction;

    private Vector2 moveInput;
    private bool accelInput;
    private bool brakeInput;
    [SerializeField] private bool driftPressed;
    [SerializeField] private bool driftHeld;
    private Vector2 gliderStick;

    //KartControl controls;
    //public InputActionReference Accelerate;

    private Rigidbody rb;

    [SerializeField] private float CurrentSpeed = 0;
    public float MaxSpeed;
    public float BoostSpeed;
    public float offroadSpeed;
    [SerializeField] private float RealSpeed;
    
    public bool GLIDER_FLY;
    public GameObject glider;

    public Transform frontLeftTire;
    public Transform frontRightTire;
    public Transform backLeftTire;
    public Transform backRightTire;

    [SerializeField] private float steerDirection;
    [SerializeField] private float driftTime;

    //drift 
    [SerializeField] bool driftLeft = false;
    [SerializeField] bool driftRight = false;
    float outwardsDriftForce = 50000;

    public bool isSliding = false;
   
    //private bool isDrifting = false;
    //private bool wasDriftingLastFrame = false;

    [SerializeField] private bool touchingGround;

    [HideInInspector]
    public float BoostTime = 0;

    public LayerMask collisionStuff;

    public bool endGlide;

    public CameraShake camShake;

    //[SerializeField] private Gamepad pad1;


    
        
    private void Awake()
    {
        //controls = new KartControl();
        //controls = new PlayerControls();

        playerInput = GetComponent<PlayerInput>();

        steerAction = playerInput.actions["Steer"];
        accelerateAction = playerInput.actions["Accelerate"];
        driftAction = playerInput.actions["Drift"];
        glideAction = playerInput.actions["GliderSteer"];
        brakeReverseAction = playerInput.actions["BrakeReverse"];


        steerAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        steerAction.canceled += ctx => moveInput = Vector2.zero;

        glideAction.performed += ctx => gliderStick = ctx.ReadValue<Vector2>();
        glideAction.canceled += ctx => gliderStick = Vector2.zero;

        accelerateAction.performed += ctx => accelInput = true;
        accelerateAction.canceled += ctx => accelInput = false;

        brakeReverseAction.performed += ctx => brakeInput = true;
        brakeReverseAction.canceled += ctx => brakeInput = false;

        driftAction.started += ctx => { driftPressed = true; driftHeld = true; };
        driftAction.canceled += ctx => { driftHeld = false; };

        


        //// Acceleration
        //controls.Player.Accelerate.performed += ctx => accelInput = true;
        //controls.Player.Accelerate.canceled += ctx => accelInput = false;

        //// Brake / Reverse
        //controls.Player.BrakeReverse.performed += ctx => brakeInput = true;
        //controls.Player.BrakeReverse.canceled += ctx => brakeInput = false;

        //// Drift
        //controls.Player.Drift.started += ctx => { driftPressed = true; driftHeld = true; };
        //controls.Player.Drift.canceled += ctx => { driftHeld = false; };

        //// Glider movement (for up/down/left/right)
        //controls.Player.GliderSteer.performed += ctx => gliderStick = ctx.ReadValue<Vector2>();
        //controls.Player.GliderSteer.canceled += ctx => gliderStick = Vector2.zero;
    }

    private void OnEnable()
    {

        //Drift.action.started += Drift;
        //controls.Player.Enable();
        steerAction?.Enable();
        accelerateAction?.Enable();
        driftAction?.Enable();
    }

    private void OnDisable()
    {
        steerAction?.Disable();
        accelerateAction?.Disable();
        driftAction?.Disable();
        //controls.Player.Disable();
        //Gamepad.current?.SetMotorSpeeds(0f, 0f); // make sure rumble is off
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        glider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //driftPressed = false;
        // Get the vertical speed
        //float verticalSpeed = rb.velocity.y;
        //Debug.Log("Vertical Speed: " + verticalSpeed);

        //if (verticalSpeed > 2)
        //{
        //    Debug.Log("DROPPED FROM HIGH!!");
        //} 

        //if (Gamepad.current == null)
        //{
        //    Debug.Log("NO CONTROLLER CONNECTED");
        //}
            

        //foreach(Gamepad gamepad in Gamepad.all)
        //{
        //    if (gamepad.buttonSouth.wasPressedThisFrame)
        //    {
        //        Debug.Log($"West button pressed on {gamepad.displayName}!");
        //    }
        //}
        
        Drift();

        //Drift rumble
        if (driftTime > 1.5 && driftTime < 4)
        {
            Debug.Log("DRIFT LEVEL 1");
            RumbleManager.instance.RumblePulse(0.01f, 0.01f, 1000f);
        }
        if (driftTime >= 4 && driftTime < 6.5)
        {
            Debug.Log("DRIFT LEVEL 2");
            RumbleManager.instance.RumblePulse(0.05f, 0.05f, 1000f);
        }
        if (driftTime >= 6.5)
        {
            Debug.Log("DRIFT LEVEL 3");
            RumbleManager.instance.RumblePulse(0.15f, 0.15f, 1000f);
        }
    }

    private void FixedUpdate()
    {
        move();
        tireSteer();
        steer();
        groundNormalRotation();
        //Drift();
        boosts();
    }

    public void move()
    {
        RealSpeed = transform.InverseTransformDirection(rb.velocity).z;

        if (accelInput)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, Time.deltaTime * 0.5f);
        }
        else if (brakeInput)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, -MaxSpeed / 1.75f, 1f * Time.deltaTime);
        }
        else
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0, Time.deltaTime * 1.5f);
        }

        if (!GLIDER_FLY)
        {
            Vector3 vel = transform.forward * CurrentSpeed;
            vel.y = rb.velocity.y;
            rb.velocity = vel;
        }
        else
        {
            Vector3 vel = transform.forward * CurrentSpeed;
            vel.y = rb.velocity.y * 0.6f;
            rb.velocity = vel;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int otherLayer = collision.gameObject.layer;

        if (((1 << otherLayer) & collisionStuff) != 0)
        {
            //Debug.Log("Hit wall or something");
            //CurrentSpeed = Mathf.Lerp(CurrentSpeed, -MaxSpeed / 1f, 1f * Time.deltaTime);
        }

        if (collision.gameObject.tag == "OffRoad")
        {
            MaxSpeed = offroadSpeed;
            //CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, 1 * Time.deltaTime);
            Debug.Log("OFFROAD!");
        }
    }

    public void steer()
    {
        steerDirection = moveInput.x;
        Vector3 steerDirVect;

        float steerAmount;

        if (driftLeft && !driftRight)
        {
            steerDirection = Input.GetAxis("Horizontal") < 0 ? -1.5f : -0.5f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.deltaTime);

            if (isSliding && touchingGround)
                rb.AddForce(transform.right * outwardsDriftForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else if (driftRight && !driftLeft)
        {
            steerDirection = Input.GetAxis("Horizontal") > 0 ? 1.5f : 0.5f;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.deltaTime);

            if (isSliding && touchingGround)
                rb.AddForce(transform.right * -outwardsDriftForce * Time.deltaTime, ForceMode.Acceleration);
        }
        else if (!driftRight && !driftLeft)
        {
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0f, 0), 8f * Time.deltaTime);
        }

        steerAmount = RealSpeed > 30 ? RealSpeed / 4 * steerDirection : steerAmount = RealSpeed / 1.5f * steerDirection;

        if (gliderStick.x < -0.5f && GLIDER_FLY) // left
        {
            //float angle = transform.localEulerAngles.z;
            //angle = (angle > 180) ? angle - 360 : angle;

            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 40), 2 * Time.deltaTime);
        }
        if (gliderStick.x > 0.5f && GLIDER_FLY) // right
        {
            //float angle = transform.localEulerAngles.z;
            //angle = (angle > 180) ? angle - 360 : angle;

            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -40), 2 * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0), 2 * Time.deltaTime);
        }

        if (gliderStick.y > 0.5f && GLIDER_FLY) // up
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(25, transform.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
            rb.AddForce(Vector3.down * 8000 * Time.deltaTime, ForceMode.Acceleration);
        }

        if (gliderStick.y < -0.5f && GLIDER_FLY) // down
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(-25, transform.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
            rb.AddForce(Vector3.up * 4000 * Time.deltaTime, ForceMode.Acceleration);
            rb.AddForce(Vector3.down * 4000 * Time.deltaTime, ForceMode.Acceleration);
            
        }
        else
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
        }




        steerDirVect = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirVect, 3 * Time.deltaTime);
    }

    public void groundNormalRotation()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 5f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }

    public void Drift()
    {
        if (driftPressed && touchingGround && CurrentSpeed > 40)
        {
            driftPressed = false;

            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hop");

            if (steerDirection > 0)
            {
                driftRight = true;
                driftLeft = false;
            }
            else if (steerDirection < 0)
            {
                driftRight = false;
                driftLeft = true;
            }
        }

        if (driftHeld && touchingGround && CurrentSpeed > 40 && Mathf.Abs(steerDirection) != 0 )
        {
            driftTime += Time.deltaTime;
        }

        if (!driftHeld || RealSpeed < 40)
        {
            driftLeft = false;
            driftRight = false;
            isSliding = false;

            if (driftTime > 1.5f && driftTime < 4f) BoostTime = 0.75f;
            if (driftTime >= 4f && driftTime < 6.5f) BoostTime = 1.5f;
            if (driftTime >= 6.5f) BoostTime = 2.5f;

            driftTime = 0;
            RumbleManager.instance.RumblePulse(0f, 0f, 0f);
        }

        //isDrifting = driftHeld && touchingGround && CurrentSpeed > 40f && Mathf.Abs(steerDirection) > 0.1f;

        //// Rumble logic
        //if (isDrifting && !wasDriftingLastFrame)
        //{
        //    // Just started drifting
        //    Gamepad.current?.SetMotorSpeeds(0.1f, 0.1f); // you can tweak strength
        //}
        //else if (!isDrifting && wasDriftingLastFrame)
        //{
        //    // Just stopped drifting
        //    Gamepad.current?.SetMotorSpeeds(0f, 0f);
        //}

        //wasDriftingLastFrame = isDrifting;
    }

    private void boosts()
    {
        BoostTime -= Time.deltaTime;
        if (BoostTime > 0)
        {
            MaxSpeed = BoostSpeed;
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, 1 * Time.deltaTime);
            //RumbleManager.instance.RumblePulse(0.25f, 1f, 0.25f);
        }
        else
        {
            MaxSpeed = BoostSpeed - 20;
        }
    }

    public void tireSteer()
    {
        if (moveInput.x < 0)
        {
            frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), 5 * Time.deltaTime);
            frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 155, 0), 5 * Time.deltaTime);
        }
        if (moveInput.x > 0)
        {
            frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), 5 * Time.deltaTime);
            frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 205, 0), 5 * Time.deltaTime);
        }
        else if (moveInput.x == 0)
        {
            frontLeftTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), 5 * Time.deltaTime);
            frontRightTire.localEulerAngles = Vector3.Lerp(frontLeftTire.localEulerAngles, new Vector3(0, 180, 0), 5 * Time.deltaTime);
        }

        if (CurrentSpeed > 30)
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * CurrentSpeed * 0.5f, 0, 0);
        }
        else
        {
            frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            backLeftTire.Rotate(90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
            backRightTire.Rotate(90 * Time.deltaTime * RealSpeed * 0.5f, 0, 0);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.tag == "Glider Panel")
        {
            GLIDER_FLY = true;
            glider.SetActive(true);
            endGlide = false;
        }

        if (other.gameObject.tag == "BoostPad")
        {
            driftTime = 4f;
            Debug.Log("BOOST PAD!!!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "OffRoad")
        {
            GLIDER_FLY = false;
            glider.SetActive(false);
        }

        

        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    // Get the Rigidbody component of the object that just hit the ground
        //    Rigidbody rb = GetComponent<Rigidbody>();

        //    if (rb != null)
        //    {
        //        // The vertical speed is the absolute value of the y-component of the velocity
        //        float verticalSpeed = Mathf.Abs(rb.velocity.y);
        //        Debug.Log($"Object hit the ground with a vertical speed of: {verticalSpeed} m/s");

        //        if (verticalSpeed > 2f)
        //        {
        //            Debug.Log("DROPPED FROM HIGH!!");
        //        }
        //    }
        //}
    }


}
