using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;

namespace CMC
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event Action<GameState> OnGameStateChange;
        private GameState gameState;
        public GameState getGameState => gameState;
        public bool useNavMesh = true;
        [SerializeField] private GameObject finishDisplay;
        [SerializeField] private TMP_Text finishOverlayText;
        [SerializeField] private UnityEvent<bool> OnGameEnd;

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
            if (gameState == newGameState) { return; }

            gameState = newGameState;

            OnGameStateChange?.Invoke(gameState);

            if(gameState == GameState.Ended)
            {
                finishDisplay.SetActive(true);
                finishOverlayText.text = hasWon ? "You WON!" : "You LOST!";
                OnGameEnd?.Invoke(hasWon);
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

