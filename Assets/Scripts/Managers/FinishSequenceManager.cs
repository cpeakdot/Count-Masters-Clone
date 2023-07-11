using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CMC.Player;
using Cinemachine;
using DG.Tweening;

namespace CMC
{
    public class FinishSequenceManager : MonoBehaviour
    {
        public static FinishSequenceManager Instance { get; private set; }
        [SerializeField] private CinemachineVirtualCamera finishVirtualCamera;
        [SerializeField] private float ySpacing = 1f;
        [SerializeField] private float xSpacing = .7f;
        [SerializeField] private float sequenceIteration = .2f;
        private int maxCloneByIteration = 4;
        private List<CloneController> cloneControllers = new List<CloneController>();
        [SerializeField] private UnityEvent OnFinishSequenceStart;
        private PlayerController playerController;
        private bool sequenceInitialized = false;

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

        [ContextMenu("Test Sequence")]
        public void InitSequence()
        {
            if (sequenceInitialized) { return; }
            sequenceInitialized = true;
            playerController = FindObjectOfType<PlayerController>();

            playerController.LockSwerve();

            float playerMoveMiddleDuration = .4f;
            playerController.transform.DOMoveX(0, playerMoveMiddleDuration);

            OnFinishSequenceStart?.Invoke();
            cloneControllers = playerController.GetCloneControllerList;
            StartCoroutine(FinishSequence());
        }

        IEnumerator FinishSequence()
        {
            int iterationCounter = 0;
            int stairCounter = 0;


            for (int i = 0; i < cloneControllers.Count; i++)
            {
                if (!cloneControllers[i]) { continue; }

                finishVirtualCamera.m_Follow = cloneControllers[i].transform;

                cloneControllers[i].StartState(CloneStates.FinishSequence);

                cloneControllers[i].DisableAgentMovement();

                cloneControllers[i].transform.localPosition = new Vector3(
                    GetXPosition(iterationCounter),
                    stairCounter * ySpacing,
                    0
                );

                iterationCounter++;

                if(iterationCounter == maxCloneByIteration)
                {
                    iterationCounter = 0;
                    stairCounter++;
                    yield return new WaitForSeconds(sequenceIteration);
                }
            }

            GameManager.Instance.SetGameState(GameState.Ended, true);
        }

        private float GetXPosition(int iterationCounter)
        {
            switch (iterationCounter)
            {
                case 0:
                    return -2 * xSpacing;
                case 1:
                    return -1 * xSpacing;
                case 2:
                    return xSpacing;
                case 3:
                    return 2 * xSpacing;
                default:
                    return 0;
            }
        }
    }
}

