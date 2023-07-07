using UnityEngine;
using System.Collections.Generic;
using cpeak.cPool;
using DG.Tweening;
using System;

namespace CMC.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private cPool cpool;
        [SerializeField] private SwerveInput swerveInput;
        [SerializeField] private Transform cloneParentTransform;

        [Header("Values")]
        [SerializeField] private float runSpeed = 2f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float swerveSpeed = 1f;
        [Range(0f, 1f),SerializeField] private float distanceBtwClones = 1f;
        [Range(0f, 3f),SerializeField] private float radiusOfClones = 1f;
        [SerializeField] private float cloneReplacementDuration = .2f;
        private bool canMoveForward = false;
        private CloneController mainClone = null;
        [SerializeField] private bool useNavMesh = true;

        public bool GetCanMoveForward => canMoveForward;
        private List<CloneController> cloneControllerList = new List<CloneController>();
        public event Action<bool> OnPlayerMovement;

        private void Start()
        {
            GetClones();
        }

        private void Update() 
        {
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
            if (useNavMesh) { return; }
            transform.position += Vector3.forward * (Time.deltaTime * runSpeed);
        }

        private void Swerve()
        {
            transform.position += (Vector3.right * swerveInput.changeOnX) * (Time.deltaTime * swerveSpeed);
        }

        private bool TryAddCloneToList(GameObject cloneInstance)
        {
            if(cloneInstance.TryGetComponent(out CloneController clone))
            {
                cloneControllerList.Add(clone);

                FormatTheShapeOfTheClones();

                return true;
            }
            Debug.Log("Clone controller not found", cloneInstance.gameObject);
            cpool.ReleaseObject("clone", cloneInstance);
            return false;
        }

        public void HandleCloneDie(CloneController clone)
        {
            cloneControllerList.Remove(clone);

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

        private void FormatTheShapeOfTheClones()
        {
            if(useNavMesh)
            {
                // Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * radiusOfClones;
                // randomPosition.y = 0f;
                // cloneControllerList[cloneControllerList.Count - 1].transform.DOLocalMove(randomPosition, cloneReplacementDuration).SetEase(Ease.OutBack);
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
                GameObject cloneInstance = cpool.GetPoolObject("clone", mainClone.transform.position, Quaternion.identity);

                cloneInstance.transform.SetParent(cloneParentTransform);

                if (TryAddCloneToList(cloneInstance)) { continue; }

                Debug.LogWarning($"<color=yellow>Failed to add the clone instance to the list</color>", cloneInstance);
            }

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
    }
}

