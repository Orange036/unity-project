
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    private float force = 1000f;
    private float speedCap = 9f;
    private float maxDistanceToGround = 500f;
    [SerializeField]private float normalDistanceToGround = 50f;
    private float sensibility = 10f;
    private float xRotation = 0;
    private float yRotation = 0;
    [SerializeField]private float springForce = 10f;

    [SerializeField] private float kp, ki, kd;

    private Vector3 gravityVector = Vector3.down * 100f;

    private Rigidbody rb;

    public Controls actions;

    private PIDController controllerPID;
    
    private void Start()
    {
        controllerPID = new PIDController(kp, ki, kd);
        rb = GetComponent<Rigidbody>();
        actions.Player.Jump.performed += OnJump;
    }

    void Update()
    {
        UpdateCameraView();
    }

    private void FixedUpdate()
    {
        controllerPID._Kp = kp;
        controllerPID._Ki = ki;
        controllerPID._Kd = kd;

        Ray r = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(r, out RaycastHit rayHit, maxDistanceToGround)){
            Debug.Log(rayHit.distance);

            float x = rayHit.distance - normalDistanceToGround;


            float controllerParametr = controllerPID.GetOutput(x, Time.fixedDeltaTime);

            rb.AddForce(Vector3.down * springForce * controllerParametr);           
            
        }


        Vector2 readValue = actions.Player.Move.ReadValue<Vector2>();
        Vector3 movementVector = readValue.y * transform.forward + readValue.x * transform.right;
        rb.AddForce(movementVector * force);

        Vector3 cappedVelocityVector = rb.velocity;

        if (rb.velocity.magnitude > speedCap)
        {
            cappedVelocityVector.Normalize();
            cappedVelocityVector *= speedCap;            
        }
        if (readValue.magnitude == 0)
        {
            cappedVelocityVector.z = 0;
            cappedVelocityVector.x = 0;
        }
        rb.velocity = cappedVelocityVector;
    }

    private void UpdateCameraView()
    {
        Vector2 mouseDelta = actions.Player.Look.ReadValue<Vector2>() * sensibility * Time.deltaTime;
        yRotation += mouseDelta.x;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        rb.AddForce(transform.up * force);
    }

    /*private void OnJump(InputAction.CallbackContext contex) 
    {
        if (isOnGround)
        {
            StartCoroutine(JumpAction(jumpDuration, jumpCoefficient));
        }
              
    }

    private IEnumerator MoveActionPercent(float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float timePercentage = time / duration;
            accelerationPercent = speedCurve.Evaluate(timePercentage);
            yield return null;
            time += Time.deltaTime;
        }
        accelerationPercent = 1f;
    }
    
    private IEnumerator JumpAction(float duration, float force)
    {
        float time = 0f;
        while(time < duration)
        {
            float timePercentage = time/duration;
            float valYpositive = jumpCurve.Evaluate(timePercentage);
            jumpVector = new Vector3(0, valYpositive, 0) * force;
            yield return null;
            time += Time.deltaTime;
        }
        jumpVector = Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        isOnGround = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        isOnGround = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isOnGround = false;
    }*/

    private void OnEnable()
    {
        actions = new Controls();
        actions.Enable();
    }
}

