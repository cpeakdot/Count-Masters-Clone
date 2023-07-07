using UnityEngine;
using cpeak.cPool;
using UnityEngine.AI;

namespace CMC.Player
{
    public class CloneController : MonoBehaviour, IDamagable
    {
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        private PlayerController playerController;
        private bool canMove = false;
        private cPool cpool;

        private void Start() 
        {
            GetPlayerController();

            cpool = cPool.instance;
        }

        private void FixedUpdate() 
        {
            if (!canMove) { return; }

            agent.SetDestination(transform.position + Vector3.forward);
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
    }
}

