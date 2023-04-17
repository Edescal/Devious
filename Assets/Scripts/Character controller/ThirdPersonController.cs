using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public bool CanMove { get; set; } = true;
    [field:SerializeField]
    public Transform OverrideCamera { get; set; }

    public Vector3 SetPosition 
    { 
        get => transform.position;
        set
        {
            if(CanMove)
            {
                Debug.Log("Tried to override controller's position. That's only possible when CanMove is false.");
                return;
            }
            transform.position = value;
        }
    }

    public Quaternion SetRotation
    {
        get => transform.rotation;
        set
        {
            if (CanMove)
            {
                Debug.Log("Tried to override controller's position. That's only possible when CanMove is false.");
                return;
            }
            transform.rotation = value;
        }
    }

    private Transform mainCamera;
    private InputReader controls;
    private CharacterController controller;
    private Animator animator;

    [Header("General settings")]
    [SerializeField] private float walkingSpeed = 1.7f;
    [SerializeField] private float sprintinSpeed = 3.5f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float rotSpeed = 10f;
    float currentSpeed = 0;
    float inputMultiplier = 0;
    float verticalSpeed = 0;

    [Header("Slope adjustment")]
    [SerializeField] private float rayOffset;
    [SerializeField] private float slopeDistanceCheck;
    float slideDistanceCheck;

    [Header("Falling & Landing")]
    [SerializeField] private float gravityValue = 10f;
    [SerializeField] private float maxFallingSpeed = 10f;
    [SerializeField] private float fallingToleranceTime = 1;
    [SerializeField] private float landingTime = 0.5f;
    [SerializeField] private float minFallingHeight = 0.7f;
    float fallingCounter = 0;
    bool isFalling = false;
    bool isGrounded;

    bool Falling
    {
        get => isFalling;
        set
        {
            if(value && !isFalling)
            {
                inputMultiplier = 0;
                isFalling = value;
                animator.SetBool("falling", true);
            } 
            else if (!value && isFalling)
            {
                animator.SetBool("falling", false);
                IEnumerator OnLanding()
                {
                    var time = new WaitForSeconds(landingTime);
                    yield return time;
                    isFalling = false;
                }
                StartCoroutine(OnLanding());
            }
        }
    }

    void Start()
    {
        mainCamera = Camera.main.transform;
        controls = GetComponent<InputReader>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleGravity();

        var direction = InputToForward(controls.Movement);
        float currentVelocity = 0;

        if (CanMove && controls.Movement != Vector2.zero && direction != Vector3.zero && !Falling) 
        {
            var lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotSpeed * Time.deltaTime);
        }

        direction = SlopeCheck(direction);
        bool sprint = controls.Sprint || controls.Autosprint;
        float speed = sprint ? sprintinSpeed : walkingSpeed;

        bool stop = !CanMove || controls.Movement == Vector2.zero || Falling;
        inputMultiplier = Mathf.SmoothDamp(inputMultiplier, stop ? 0 : 1, ref currentVelocity, acceleration);
        currentSpeed = Mathf.SmoothDamp(currentSpeed, stop ? 0 : speed, ref currentVelocity, 0.1f);

        var movement = (direction * currentSpeed) * inputMultiplier;

        var gravity = Vector3.up * verticalSpeed;
        var velocity = gravity + movement;
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("forward", (currentSpeed * inputMultiplier), 0.1f, Time.deltaTime);
    }

    Vector3 InputToForward(Vector2 input)
    {
        var right = Vector3.ProjectOnPlane(OverrideCamera != null ? OverrideCamera.right : mainCamera.right, Vector3.up).normalized;
        var forward = Vector3.ProjectOnPlane(OverrideCamera != null ? OverrideCamera.forward : mainCamera.forward, Vector3.up).normalized;
        return ((right * input.x) + (forward * input.y)).normalized;
    }

    void HandleGravity()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalSpeed = -1;
            Falling = false;
            fallingCounter = 0;
        }
        else
        {
            fallingCounter += Time.deltaTime;

            verticalSpeed -= gravityValue * Time.deltaTime;
            if (verticalSpeed < -maxFallingSpeed)
            {
                verticalSpeed = -maxFallingSpeed;
            }

            bool tooHeight = !Physics.Raycast(transform.position, Vector3.down, minFallingHeight);
            if (tooHeight && fallingCounter > fallingToleranceTime)
            {
                Falling = true;
            }
        }
    }

    bool SlideCheck(out Vector3 direction)
    {
        var origin = transform.position;
        origin.y += rayOffset;
        if(isGrounded && Physics.Raycast(origin, Vector3.down, out var hit, slideDistanceCheck))
        {
            direction = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.down), hit.normal);
            return Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit;
        }

        direction = Vector3.zero;
        return false;
    }

    Vector3 SlopeCheck(Vector3 current)
    {
        var temp = current;
        var origin = transform.position;
        origin.y += rayOffset;
        var ray = new Ray(origin, Vector3.down);
        if(Physics.Raycast(ray, out var hit, slopeDistanceCheck))
        {
            temp.y = 0;
            Quaternion adjustedRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            temp = adjustedRotation * temp;
            if(temp.y < 0)
            {
                return temp;
            }
        }
        return current;
    }
}
