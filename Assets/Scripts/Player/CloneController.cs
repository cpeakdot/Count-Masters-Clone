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
        private PlayerController playerController;
        private CloneStates cloneState;

        [SerializeField] private LayerMask enemyLayerMask;
        [Header("Values")]
        [SerializeField] private float enemyScanRadius = 5f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float gatherAroundPlayerObjIteration = .5f;
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
                case CloneStates.Scanning:
                    
                    if (!canMove) { return; }

                    //agent.SetDestination(transform.position + Vector3.forward);
                    //agent.SetDestination(playerController.transform.position);

                    ScanEnemies();

                    GatherAroundPlayer();

                    break;
                case CloneStates.Attacking:

                    if (targetEnemy.isActiveAndEnabled) { return; }

                    StartState(CloneStates.Scanning);

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
            if(other.TryGetComponent(out Gate gate))
            {
                playerController.HandleOnTriggerGate(gate);
            }
            else if(other.TryGetComponent(out Obstacle obstacle))
            {
                KillThisObject();
            }
        }

        private void KillThisObject()
        {
            this.transform.SetParent(null);

            playerController.HandleCloneDie(this);

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
                StartState(CloneStates.Scanning);
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
            gatherAroundPlayerTimer += Time.deltaTime;
            if(gatherAroundPlayerTimer >= gatherAroundPlayerObjIteration)
            {
                gatherAroundPlayerTimer = 0f;
                agent.SetDestination(playerController.transform.position);
            }
        }

        private void ScanEnemies()
        {
            if (Physics.OverlapSphereNonAlloc(transform.position, enemyScanRadius, enemyArray, enemyLayerMask) < 1) { return; }

            StartState(CloneStates.Attacking);
        }

        private void StartState(CloneStates newCloneState)
        {
            if (newCloneState == cloneState) { return; }

            cloneState = newCloneState;

            switch (cloneState)
            {
                case CloneStates.Scanning:
                    break;
                case CloneStates.Attacking:

                    targetEnemy = enemyArray[0].GetComponent<EnemyController>();

                    break;
                default:
                    break;
            }
        }
    }
}

