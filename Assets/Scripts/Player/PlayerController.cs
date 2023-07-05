using UnityEngine;
using System.Collections.Generic;
using cpeak.cPool;

namespace CMC.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private cPool cpool;
        [SerializeField] private SwerveInput swerveInput;

        [Header("Values")]
        [SerializeField] private float runSpeed = 2f;
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float swerveSpeed = 1f;

        private List<CloneController> cloneControllerList = new List<CloneController>();
        private bool canMoveForward = false;

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
        }

        private void MoveForward()
        {
            transform.position += Vector3.forward * (Time.deltaTime * runSpeed);
        }

        private void Swerve()
        {
            transform.position += (Vector3.right * swerveInput.changeOnX) * (Time.deltaTime * swerveSpeed);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.TryGetComponent(out Gate gate))
            {

            }    
        }

        private void HandleOnGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Started:
                    canMoveForward = true;
                    break;
                case GameState.Ended:
                    canMoveForward = false;
                    break;
                default:
                    break;
            }
        }
    }
}

