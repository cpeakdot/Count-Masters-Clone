using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelButton : MonoBehaviour
{
    private Button button;

    private void Awake() 
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NextLevel);
    }

    private void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = (SceneManager.sceneCountInBuildSettings >  currentSceneIndex + 1)
        ? currentSceneIndex + 1
        : 0;
        SceneManager.LoadScene(nextScene);
    }
}
