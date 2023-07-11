using UnityEngine;
using cpeak.cPool;
using UnityEngine.AI;
using CMC.Enemy;

namespace CMC.Player
{
    public class CloneController : MonoBehaviour, IDamagable
    {
        [Header("Refs")]
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Collider coll;
        private PlayerController playerController;
        private CloneStates cloneState;
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("Values")]
        [SerializeField] private float enemyScanRadius = 5f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float gatherAroundPlayerObjIteration = .5f;
        [SerializeField] private float runningStateAgentSpeed = 1f;
        [SerializeField] private float attackingStateAgentSpeed = 3f;
        [SerializeField] private float runningStateAgentAcceleration = 1f;
        [SerializeField] private float attackingStateAgentAcceleration = 200f;
        private float gatherAroundPlayerTimer = 0f;
        private Collider[] enemyArray = new Collider[1];
        private EnemyController targetEnemy = null;
        private bool canMove = false;
        private cPool cpool;

        public NavMeshAgent GetAgent => agent;

        private void Start() 
        {
            GetPlayerController();

            cpool = cPool.instance;
        }

        private void Update() 
        {
            switch (cloneState)
            {
                case CloneStates.Running:
                    
                    if (!canMove) { return; }

                    //agent.SetDestination(transform.position + Vector3.forward);
                    //agent.SetDestination(playerController.transform.position);

                    GatherAroundPlayer();

                    break;
                case CloneStates.Attacking:

                    if (!targetEnemy.isActiveAndEnabled) { return; }

                    agent.SetDestination(targetEnemy.transform.position);

                    Attack();

                    break;
                case CloneStates.Falling:
                    if(transform.position.y <= -1f)
                    {
                        KillThisObject();
                    }
                    break;
                default:
                    break;
            }
           
        }

        private void OnEnable() 
        {
            if(!playerController)
            {
                GetPlayerController();
            }
            if(playerController)
            {
                playerController.OnPlayerMovement += HandleOnPlayerMovement;
            }
        }

        private void OnDisable() 
        {
            if (!playerController) { return; }
            playerController.OnPlayerMovement -= HandleOnPlayerMovement;
        }

        private void GetPlayerController()
        {
            playerController = GetComponentInParent<PlayerController>();

            if (playerController == null) { return; }

            if(playerController.GetCanMoveForward)
            {
                animator.SetTrigger("Run");
                canMove = true;
            }
            else
            {
                animator.SetTrigger("Idle");
                canMove = false;
            }
        }

        private void HandleOnPlayerMovement(bool isMoving)
        {
            animator.ResetTrigger("Run");
            animator.ResetTrigger("Idle");
            animator.SetTrigger((isMoving) ? "Run" : "Idle");

            canMove = isMoving;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.CompareTag("MovingPlatform"))
            {
                Fall();
                return;
            }
            if(other.TryGetComponent(out Gate gate))
            {
                playerController.HandleOnTriggerGate(gate);
            }
            else if(other.TryGetComponent(out Obstacle obstacle))
            {
                KillThisObject();
            }
        }

        // private void OnCollisionEnter(Collision other) 
        // {
        //     if (!other.transform.TryGetComponent(out EnemyController enemy)) { return; }

        //     enemy.Damage();
        //     KillThisObject();
        // }

        private void Fall()
        {
            StartState(CloneStates.Falling);
        }

        private void KillThisObject()
        {
            this.transform.SetParent(null);

            playerController.HandleCloneDie(this);

            cpool.GetPoolObject("deathParticleClone", transform.position, Quaternion.identity, true, 2f);

            cpool.ReleaseObject("clone", this.gameObject);
        }

        public void Damage()
        {
            KillThisObject();
        }

        private void Attack()
        {
            if(!targetEnemy.enabled)
            {
                StartState(CloneStates.Running);
                return;
            }

            if(Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange)
            {
                targetEnemy.Damage();
                this.Damage();
            }
            else
            {
                agent.SetDestination(targetEnemy.transform.position);
            }
        }

        private void GatherAroundPlayer()
        {
            if (cloneState == CloneStates.Falling) { return; }

            gatherAroundPlayerTimer += Time.deltaTime;

            if(gatherAroundPlayerTimer >= gatherAroundPlayerObjIteration)
            {
                gatherAroundPlayerTimer = 0f;

                agent.SetDestination(playerController.transform.position);
            }
        }

        public void AttackEnemies(EnemyController enemyController)
        {
            if (cloneState == CloneStates.Falling) { return; }

            targetEnemy = enemyController;

            StartState(CloneStates.Attacking);
        }

        public void StartState(CloneStates newCloneState)
        {
            if (cloneState == CloneStates.Falling) { return; }

            if (newCloneState == cloneState) { return; }

            cloneState = newCloneState;

            switch (cloneState)
            {
                case CloneStates.Running:

                    agent.SetDestination(playerController.transform.position);

                    agent.acceleration = runningStateAgentAcceleration;

                    agent.speed = runningStateAgentSpeed;

                    break;

                case CloneStates.Attacking:

                    agent.acceleration = attackingStateAgentAcceleration;

                    agent.speed = attackingStateAgentSpeed;

                    break;

                case CloneStates.Falling:

                    transform.SetParent(null);

                    coll.isTrigger = true;

                    rigidBody.isKinematic = false;

                    agent.enabled = false;

                    break;

                default:
                    break;
            }
        }
    }
}

