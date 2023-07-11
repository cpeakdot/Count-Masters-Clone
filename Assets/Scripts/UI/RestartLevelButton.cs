using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartLevelButton : MonoBehaviour
{
    private Button button;
    private void Awake() 
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(RestartLevel);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
