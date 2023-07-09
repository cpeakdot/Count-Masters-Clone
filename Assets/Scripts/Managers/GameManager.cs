using UnityEngine;
using UnityEngine.AI;
using System;

namespace CMC
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event Action<GameState> OnGameStateChange;
        private GameState gameState;
        public GameState getGameState => gameState;
        public bool useNavMesh = true;
        [SerializeField] private NavMeshSurface navmeshSurface;
        [SerializeField] private float navMeshUpdateIteration = .2f;
        private float navMeshIterationTimer = 0f;

        private void Awake() 
        {
            if(Instance == null)
            {
                Instance = this;
            }    
            else
            {
                Destroy(this);
            }

            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;
        }

        private void Update() 
        {
            navMeshIterationTimer += Time.deltaTime;

            if(navMeshIterationTimer >= navMeshUpdateIteration)
            {
                navMeshIterationTimer = 0f;
                UpdateNavMesh();
            }

            if (gameState != GameState.Not_Started) { return; }

            if(Input.GetMouseButtonDown(0))
            {
                gameState = GameState.Started;
                
                OnGameStateChange?.Invoke(gameState);
            }
        }
        private void UpdateNavMesh()
        {
            navmeshSurface.BuildNavMesh();
        }

        public void SetGameState(GameState newGameState, bool hasWon = false)
        {
            gameState = newGameState;

            OnGameStateChange?.Invoke(gameState);
        }
    }

    public enum GameState
    {
        Not_Started,
        Started,
        Ended
    }
}

