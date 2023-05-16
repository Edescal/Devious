using System.Collections;
using UnityEngine.AI;
using UnityEngine;

namespace Edescal.AI
{
    public class Enemy : MonoBehaviour, IEntity
    {
        public NavMeshAgent agent { get; private set; }
        public Inventory Inventory { get; private set; }
        public Health Health { get; private set; }
        public Animator Animator { get; private set; }

        [field: SerializeField]
        public float maxDistanceToPlayer { get; private set; } = 5f;
        [field: SerializeField]
        public float knockbackDuration { get; private set; } = 2f;
        [field: SerializeField]
        public Transform player { get; private set; }
        [field: SerializeField]
        public bool chasePlayer { get; set; }
        [field: SerializeField]
        public float speedParam { get; private set; } = 3f;

        [SerializeField]
        private float fieldOfVision = 60f;
        [SerializeField]
        private LayerMask obstacleMask;
        [SerializeField]
        private float rotationSpeed = 10f;
        
        private EnemyState currentState;

        public void ApplyDamage(int damage, object source)
        {
            if (Health != null)
            {
                if (Health.CurrentHealth == 0) return;

                if ((Health.CurrentHealth -= damage) > 0)
                {
                    SwitchState(new KnockbackState(this));
                }
                else
                {
                    SwitchState(new DeadState(this));
                }
            }
        }

        public void SwitchState(EnemyState state)
        {
            if (state == null) return;

            currentState.OnExit();
            currentState = state;
            currentState.OnEnter();
        }

        public void RotateToPlayer()
        {
            var direction = Vector3.ProjectOnPlane((player.position - transform.position), Vector3.up);
            if (direction != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        public bool CanSeePlayer
        {
            get
            {
                var direction = Vector3.ProjectOnPlane((player.position - transform.position).normalized, Vector3.up);
                float dot = Vector3.Dot(direction, transform.forward);
                print($"{dot}   {Mathf.Cos(Mathf.Deg2Rad * fieldOfVision / 2)}");
                if (dot > Mathf.Cos(Mathf.Deg2Rad * fieldOfVision / 2))
                {
                    var offset = Vector3.up * 1;
                    if (!Physics.Linecast(transform.position + offset, player.position + offset, obstacleMask))
                        return true;
                }

                return false;
            }
        }

        public float DistanceToPlayer
        {
            get
            {
                return Vector3.Distance(transform.position, player.position);
            }
        }

        void Start()
        {
            currentState = new IdleState(this);
            Health = GetComponent<Health>();
            Inventory = GetComponent<Inventory>();
            Animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (currentState != null)
            {
                currentState.Tick();
            }
        }
    }

    
}