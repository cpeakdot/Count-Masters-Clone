using UnityEngine;
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
            if (gameState != GameState.Not_Started) { return; }

            if(Input.GetMouseButtonDown(0))
            {
                gameState = GameState.Started;
                
                OnGameStateChange?.Invoke(gameState);
            }
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

