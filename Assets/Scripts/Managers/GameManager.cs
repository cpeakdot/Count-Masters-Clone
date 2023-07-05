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
    }

    public enum GameState
    {
        Not_Started,
        Started,
        Ended
    }
}

