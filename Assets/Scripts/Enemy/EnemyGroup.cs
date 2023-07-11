using UnityEngine;
using CMC.Enemy;
using System.Collections.Generic;
using TMPro;

[ExecuteInEditMode]
public class EnemyGroup : MonoBehaviour
{
    [Range(0f,1f), SerializeField] private float groupRadius = .1f;
    [Range(0f,1f), SerializeField] private float distanceBtwEnemies = .1f;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private EnemyController enemyControllerPrefab;
    [SerializeField] private List<EnemyController> enemyControllers = new List<EnemyController>();
    [SerializeField] private TMP_Text countText;
    [SerializeField] private GameObject countDisplayParent;

    private void Start() 
    {
        ResetEnemyControllersList();

        UpdateCountText();
    }

    [ContextMenu("Init Enemy Group")]
    public void FormatTheShapeOfTheEnemyGroup()
    {
        foreach (EnemyController enemy in enemyControllers)
        {
            if (enemy == null) { continue; }

            DestroyImmediate(enemy.gameObject);
        }

        enemyControllers.Clear();

        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemyControllerInstance = Instantiate(enemyControllerPrefab);

            float x = distanceBtwEnemies * Mathf.Sqrt(i) * Mathf.Cos(i * groupRadius);
            float z = distanceBtwEnemies * Mathf.Sqrt(i) * Mathf.Sin(i * groupRadius);

            Vector3 pos = new Vector3(x, 0f, z) + transform.position;

            enemyControllerInstance.transform.rotation = transform.rotation;
            
            enemyControllerInstance.transform.SetParent(this.transform);

            enemyControllers.Add(enemyControllerInstance);

            enemyControllerInstance.transform.position = pos;
        }
    }

    private void ResetEnemyControllersList()
    {
        enemyControllers.Clear();

        EnemyController[] tempList = GetComponentsInChildren<EnemyController>();

        foreach (EnemyController enemyController in tempList)
        {
            enemyControllers.Add(enemyController);
        }
    }

    public void HandleOnCloneDie(EnemyController clone)
    {        
        enemyControllers.Remove(clone);

        enemyCount--;

        UpdateCountText();
    }

    private void UpdateCountText()
    {
        if(enemyCount == 0)
        {
            countDisplayParent.SetActive(false);
            return;
        }
        countText.text = enemyCount.ToString();
    }

}
