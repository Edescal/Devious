using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edescal
{
    public class ThirdPersonController : MonoBehaviour
    {
        public IEntity player { get; private set; }
        public bool CanMove { get; set; } = true;
        public Transform OverrideCamera { get; set; }

        public Vector3 SetPosition
        {
            get => transform.position;
            set
            {
                if (CanMove)
                {
                    Debug.Log("Tried to override controller's position. That's only possible when CanMove is false.");
                    return;
                }
                controller.enabled = false;
                transform.position = value;
                controller.enabled = true;
                isFalling = false;
                fallingCounter = 0;
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
        private LockOnTarget lockOnTarget;
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
                if (value && !isFalling)
                {
                    inputMultiplier = 0;
                    isFalling = value;
                    animator.SetBool("falling", true);
                }
                else if (!value && isFalling)
                {
                    isFalling = false;
                    CanMove = false;
                    animator.SetBool("falling", false);
                    
                    IEnumerator OnLanding()
                    {
                        if(player != null)
                        {
                            int damage = fallingCounter > 1.5f ? 3
                                : fallingCounter > 1.25f ? 2
                                : fallingCounter > 1 ? 1 : 0;

                            if(damage != 0)
                            {
                                player.ApplyDamage(damage, this);
                            }
                        }
                        var time = new WaitForSeconds(landingTime);
                        yield return time;
                        CanMove = true;
                    }
                    StartCoroutine(OnLanding());
                }
            }
        }

        public void SetTarget(LockOnTarget target) => lockOnTarget = target;
        public void UnsetTarget() => lockOnTarget = null;

        void Start()
        {
            player = GetComponent<IEntity>();
            mainCamera = Camera.main.transform;
            controls = GetComponent<InputReader>();
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (Time.timeScale == 0) return;

            HandleGravity();

            var direction = InputToForward(controls.Movement);
            if (lockOnTarget != null)
            {
                var localVector = transform.InverseTransformDirection(direction);
                localVector.x = Mathf.Round(localVector.x * 100) / 100;
                localVector.z = Mathf.Round(localVector.z * 100) / 100;

                if (Mathf.Abs(localVector.z) >= Mathf.Abs(localVector.x))
                    direction = transform.TransformDirection(new Vector3(0, 0, Mathf.Sign(localVector.z)));
                else direction = transform.TransformDirection(new Vector3(Mathf.Sign(localVector.x), 0, 0));
            }

            direction = SlopeCheck(direction);
            bool sprint = controls.Sprint || controls.Autosprint;
            float speed = sprint ? sprintinSpeed : walkingSpeed;

            float currentVelocity = 0;
            bool stop = !CanMove || controls.Movement == Vector2.zero || Falling;

            inputMultiplier = Mathf.SmoothDamp(inputMultiplier, stop ? 0 : 1, ref currentVelocity, acceleration);
            currentSpeed = Mathf.SmoothDamp(currentSpeed, stop ? 0 : speed, ref currentVelocity, 0.1f);

            var movement = (direction * currentSpeed) * inputMultiplier;

            var gravity = Vector3.up * verticalSpeed;
            var velocity = gravity + movement;
            controller.Move(velocity * Time.deltaTime);

            if (CanMove && !Falling)
            {
                if (lockOnTarget != null)
                {
                    RotateToTarget();
                }
                else
                {
                    if (direction != Vector3.zero && controls.Movement != Vector2.zero)
                    {
                        var lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotSpeed * Time.deltaTime);
                    }
                }
            }

            if (lockOnTarget != null)
            {
                var animVector = transform.InverseTransformDirection(direction);
                var absDirection = Vector2.zero;
                if (controls.Movement != Vector2.zero)
                {
                    animVector.x = Mathf.Round(animVector.x * 100) / 100;
                    animVector.z = Mathf.Round(animVector.z * 100) / 100;

                    if (Mathf.Abs(animVector.z) >= Mathf.Abs(animVector.x))
                        absDirection = new Vector2(0, Mathf.Sign(animVector.z));
                    else absDirection = new Vector2(Mathf.Sign(animVector.x), 0);
                }
                print($"x: {absDirection.x} | y: {absDirection.y}");
                animator.SetFloat("forward", absDirection.y * currentSpeed * inputMultiplier, 0.1f, Time.deltaTime);
                animator.SetFloat("horizontal", absDirection.x * currentSpeed * inputMultiplier, 0.1f, Time.deltaTime);
            }
            else
            {
                animator.SetFloat("forward", (currentSpeed * inputMultiplier), 0.1f, Time.deltaTime);
                animator.SetFloat("horizontal", 0, 0.1f, Time.deltaTime);
            }
        }

        Vector3 InputToForward(Vector2 input)
        {
            var right = Vector3.ProjectOnPlane(OverrideCamera != null ? OverrideCamera.right : mainCamera.right, Vector3.up).normalized;
            var forward = Vector3.ProjectOnPlane(OverrideCamera != null ? OverrideCamera.forward : mainCamera.forward, Vector3.up).normalized;
            return ((right * input.x) + (forward * input.y)).normalized;
        }

        void RotateToTarget()
        {
            if (lockOnTarget == null) return;

            var direction = Vector3.ProjectOnPlane((lockOnTarget.transform.position - transform.position).normalized, Vector3.up);
            if (direction != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotSpeed * Time.deltaTime);
            }

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
            if (isGrounded && Physics.Raycast(origin, Vector3.down, out var hit, slideDistanceCheck))
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
            if (Physics.Raycast(ray, out var hit, slopeDistanceCheck))
            {
                temp.y = 0;
                Quaternion adjustedRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                temp = adjustedRotation * temp;
                if (temp.y < 0)
                {
                    return temp;
                }
            }
            return current;
        }
    }
}