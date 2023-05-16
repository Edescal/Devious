using UnityEngine;

namespace Edescal.AI
{
    public abstract class EnemyState
    {
        protected Enemy machine;
        public EnemyState(Enemy machine)
        {
            this.machine = machine;
        }
        public abstract void OnEnter();
        public abstract void Tick();
        public abstract void OnExit();
    }

    public class IdleState : EnemyState
    {
        public IdleState(Enemy machine) : base(machine) { }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void Tick()
        {
            if (machine.DistanceToPlayer >= machine.maxDistanceToPlayer) return;

            if (!machine.CanSeePlayer) return;

            if (machine.chasePlayer)
            {
                machine.SwitchState(new ChaseState(machine));
            }
            else
            {
                machine.RotateToPlayer();
            }
        }
    }

    public class KnockbackState : EnemyState
    {
        private float exitTime;

        public KnockbackState(Enemy machine) : base(machine) { }

        public override void OnEnter()
        {
            exitTime = Time.time + machine.knockbackDuration;
            machine.Animator.SetFloat("forward", 0);
            machine.Animator.CrossFadeInFixedTime("Knockback", 0.2f);
        }

        public override void OnExit() { }

        public override void Tick()
        {
            machine.RotateToPlayer();

            if (Time.time > exitTime)
            {
                machine.SwitchState(new IdleState(machine));
            }
        }
    }

    public class DeadState : EnemyState
    {
        public DeadState(Enemy machine) : base(machine) { }

        public override void OnEnter()
        {
            machine.enabled = false;
            machine.Animator.CrossFadeInFixedTime("Stunned", 0.25f);
        }

        public override void OnExit()
        {
            machine.enabled = true;
        }

        public override void Tick() { }
    }

    public class ChaseState : EnemyState
    {
        public ChaseState(Enemy machine) : base(machine) { }

        public override void OnEnter() { }

        public override void OnExit()
        {
            machine.agent.ResetPath();
            machine.Animator.SetFloat("forward", 0);
        }

        public override void Tick()
        {
            if (!machine.chasePlayer)
            {
                machine.SwitchState(new IdleState(machine));
                return;
            }

            if (machine.DistanceToPlayer > machine.maxDistanceToPlayer)
            {
                machine.RotateToPlayer();
                machine.agent.ResetPath();
                machine.Animator.SetFloat("forward", 0, 0.4f, Time.deltaTime);
                if (machine.Animator.GetFloat("forward") <= 0.02f)
                {
                    machine.SwitchState(new IdleState(machine));
                }
            }
            else
            {
                Vector3 velocity = machine.agent.velocity;
                var direction = Vector3.ProjectOnPlane(velocity, Vector3.up);
                if (direction != Vector3.zero)
                {
                    var targetRot = Quaternion.LookRotation(direction);
                    machine.transform.rotation = Quaternion.Slerp(machine.transform.rotation, targetRot, 10 * Time.deltaTime);
                }
                float magnitude = velocity.magnitude;
                machine.agent.SetDestination(machine.player.position);
                machine.Animator.SetFloat("forward", magnitude * machine.speedParam, 0.2f, Time.deltaTime);
            }
        }
    }
}