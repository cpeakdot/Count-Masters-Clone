using CMC.Player;
using UnityEngine;
using UnityEngine.AI;
using cpeak.cPool;

namespace CMC.Enemy
{
    public class EnemyController : MonoBehaviour, IDamagable
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float movementSpeed = 3.5f;
        [SerializeField] private LayerMask cloneLayerMask;
        [SerializeField] private float cloneScanRadius = 5f;
        [SerializeField] private float attackRange = 1f;
        private Collider[] cloneArray = new Collider[1];
        private CloneController targetClone = null;
        [SerializeField] private EnemyState enemyState;

        private void Update() 
        {
            switch (enemyState)
            {
                case EnemyState.Scanning:
                    
                    if (Physics.OverlapSphereNonAlloc(transform.position, cloneScanRadius, cloneArray, cloneLayerMask) < 1) { return; }

                    StartState(EnemyState.Attacking);

                    break;

                case EnemyState.Attacking:

                    Attack();

                    break;
                default:
                    break;
            }
        }

        private void StartState(EnemyState newEnemyState)
        {
            if (newEnemyState == enemyState) { return; }

            enemyState = newEnemyState;

            switch (enemyState)
            {
                case EnemyState.Scanning:
                    break;
                case EnemyState.Attacking:
                    
                    targetClone = cloneArray[0].GetComponent<CloneController>();
                    
                    break;
                default:
                    break;
            }
        }

        private void Attack()
        {
            if(!targetClone.enabled)
            {
                StartState(EnemyState.Scanning);
                return;
            }

            if(Vector3.Distance(transform.position, targetClone.transform.position) <= attackRange)
            {
                targetClone.Damage();
                this.Damage();
            }
            else
            {
                agent.SetDestination(targetClone.transform.position);
            }
        }

        public void Damage()
        {
            cPool.instance.ReleaseObject("enemy", this.gameObject);
        }

        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, cloneScanRadius);
        }
    }

    [System.Serializable]
    public enum EnemyState
    {
        Scanning,
        Attacking
    }
}

