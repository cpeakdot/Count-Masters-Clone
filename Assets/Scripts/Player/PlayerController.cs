using UnityEngine;
using System.Collections.Generic;
using cpeak.cPool;
using DG.Tweening;
using System;
using Cinemachine;
using TMPro;
using CMC.Enemy;

namespace CMC.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private cPool cpool;
        [SerializeField] private SwerveInput swerveInput;
        [SerializeField] private Transform cloneParentTransform;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("Values")]
        [SerializeField] private float runSpeed = 2f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float swerveSpeed = 1f;
        [SerializeField] private float enemyScanRadius = 5f;
        [Range(0f, 1f),SerializeField] private float distanceBtwClones = 1f;
        [Range(0f, 3f),SerializeField] private float radiusOfClones = 1f;
        [SerializeField] private float cloneReplacementDuration = .2f;
        private Collider[] enemyArray = new Collider[1];
        private bool areClonesOnAttackState = false;
        private float enemyScanRadiusCalculated = 5f;
        private bool canMoveForward = false;
        private CloneController mainClone = null;
        private bool useNavMesh = true;
        /// <summary>
        /// Used to count the calls from clones
        /// </summary>
        private int cloneCounterOnStartMovement = 0;

        public bool GetCanMoveForward => canMoveForward;
        private List<CloneController> cloneControllerList = new List<CloneController>();
        public event Action<bool> OnPlayerMovement;

        private void Start()
        {
            useNavMesh = GameManager.Instance.useNavMesh;

            GetClones();

            enemyScanRadiusCalculated = GetAdditionalEnemyScanRadius(cloneControllerList.Count) + enemyScanRadius;
        }

        private void Update() 
        {
            ScanEnemies();

            if (!canMoveForward) { return; }
            
            MoveForward();

            Swerve();
        }

        private void OnEnable() 
        {
            GameManager.OnGameStateChange += HandleOnGameStateChange; 
        }

        private void OnDisable() 
        {
            GameManager.OnGameStateChange -= HandleOnGameStateChange;
        }

        private void GetClones()
        {
            CloneController[] clones = GetComponentsInChildren<CloneController>();

            for (int i = 0; i < clones.Length; i++)
            {
                cloneControllerList.Add(clones[i]);
            }

            mainClone = cloneControllerList[0];
        }

        private void MoveForward()
        {
            transform.position += Vector3.forward * (Time.deltaTime * runSpeed);
        }

        private void Swerve()
        {
            transform.position += (Vector3.right * swerveInput.changeOnX) * (Time.deltaTime * swerveSpeed);
        }

        private void ScanEnemies()
        {
            if (Physics.OverlapSphereNonAlloc(transform.position, enemyScanRadiusCalculated, enemyArray, enemyLayerMask) < 1) 
            {
                if(areClonesOnAttackState)
                {
                    for (int i = 0; i < cloneControllerList.Count; i++)
                    {
                        cloneControllerList[i].StartState(CloneStates.Running);
                    }
                    StartMovement();
                }
                return;
            }

            if (!enemyArray[0].TryGetComponent(out EnemyController enemyController)) { return; }

            StopMovement();

            areClonesOnAttackState = true;

            for (int i = 0; i < cloneControllerList.Count; i++)
            {
                cloneControllerList[i].AttackEnemies(enemyController);
            }
        }

        private bool TryAddCloneToList(GameObject cloneInstance)
        {
            if(cloneInstance.TryGetComponent(out CloneController clone))
            {
                cloneControllerList.Add(clone);

                FormatTheShapeOfTheClones();

                enemyScanRadiusCalculated = GetAdditionalEnemyScanRadius(cloneControllerList.Count) + enemyScanRadius;

                UpdateCountText();

                return true;
            }
            Debug.Log("Clone controller not found", cloneInstance.gameObject);
            cpool.ReleaseObject("clone", cloneInstance);
            return false;
        }

        public void HandleCloneDie(CloneController clone)
        {
            cloneControllerList.Remove(clone);

            UpdateCountText();

            enemyScanRadiusCalculated = GetAdditionalEnemyScanRadius(cloneControllerList.Count) + enemyScanRadius;

            int currentCloneCount = cloneControllerList.Count;

            if (clone == mainClone && currentCloneCount > 0)
            {
                mainClone = cloneControllerList[0];
            }
            else if (currentCloneCount == 0)
            {
                GameManager.Instance.SetGameState(GameState.Ended, false);
                return;
            }

            if (useNavMesh) { return; }
            
            FormatTheShapeOfTheClones();
        }

        private float GetAdditionalEnemyScanRadius(int cloneCount)
        {
            if (cloneCount < 10) { return 0; }

            float additionalRadius = cloneCount / 30;

            return additionalRadius;
        }

        private void UpdateCountText()
        {
            countText.text = cloneControllerList.Count.ToString();
        }

        private void FormatTheShapeOfTheClones()
        {
            if(useNavMesh)
            {
                return;
            }
            for (int i = 0; i < cloneControllerList.Count; i++)
            {
                    float x = distanceBtwClones * Mathf.Sqrt(i) * Mathf.Cos(i * radiusOfClones);
                    float z = distanceBtwClones * Mathf.Sqrt(i) * Mathf.Sin(i * radiusOfClones);

                    Vector3 pos = new Vector3(x, 0f, z);

                    cloneControllerList[i].transform.DOLocalMove(pos, cloneReplacementDuration).SetEase(Ease.OutBack);
            }
        }

        public void HandleOnTriggerGate(Gate gate)
        {
            gate.HandleOnTriggered();

            int gateValue = gate.GetValue;
            int amountToBeSpawned = 0;
            
            switch (gate.GetGateType)
            {
                case GateType.Multiply:
                    amountToBeSpawned = (cloneControllerList.Count * gateValue) - cloneControllerList.Count;
                    break;
                case GateType.Sum:
                    amountToBeSpawned = gateValue;
                    break;
                default:
                    break;
            }
            
            for (int i = 0; i < amountToBeSpawned; i++)
            {
                float randomPositionRadius = 1f;

                Vector3 randomPos = UnityEngine.Random.insideUnitSphere * randomPositionRadius + transform.position;

                randomPos.y = transform.position.y;

                GameObject cloneInstance = cpool.GetPoolObject("clone", randomPos, Quaternion.identity);

                cloneInstance.transform.SetParent(cloneParentTransform);

                if (TryAddCloneToList(cloneInstance)) { continue; }

                Debug.LogWarning($"<color=yellow>Failed to add the clone instance to the list</color>", cloneInstance);
            }

        }

        public void StopMovement()
        {
            canMoveForward = false;
        }

        public void StartMovement()
        {
            canMoveForward = true;
        }

        private void HandleOnGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Started:
                    canMoveForward = true;
                    OnPlayerMovement?.Invoke(true);
                    break;
                case GameState.Ended:
                    canMoveForward = false;
                    OnPlayerMovement?.Invoke(false);
                    break;
                default:
                    break;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, (Application.isPlaying) ? enemyScanRadiusCalculated : enemyScanRadius);
        }
#endif
    }
}

