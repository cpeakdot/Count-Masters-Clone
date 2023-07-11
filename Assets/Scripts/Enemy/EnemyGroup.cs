using UnityEngine;
using CMC.Enemy;

[ExecuteInEditMode]
public class EnemyGroup : MonoBehaviour
{
    [Range(0f,1f), SerializeField] private float groupRadius = .1f;
    [Range(0f,1f), SerializeField] private float distanceBtwEnemies = .1f;
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private EnemyController enemyControllerPrefab;
    [SerializeField] private EnemyController[] enemyControllers;

    [ContextMenu("Init Enemy Group")]
    public void FormatTheShapeOfTheEnemyGroup()
    {
        foreach (EnemyController enemy in enemyControllers)
        {
            if (enemy == null) { continue; }

            DestroyImmediate(enemy.gameObject);
        }

        enemyControllers = new EnemyController[enemyCount];

        for (int i = 0; i < enemyControllers.Length; i++)
        {
            EnemyController enemyControllerInstance = Instantiate(enemyControllerPrefab);

            float x = distanceBtwEnemies * Mathf.Sqrt(i) * Mathf.Cos(i * groupRadius);
            float z = distanceBtwEnemies * Mathf.Sqrt(i) * Mathf.Sin(i * groupRadius);

            Vector3 pos = new Vector3(x, 0f, z) + transform.position;

            enemyControllerInstance.transform.rotation = transform.rotation;
            
            enemyControllerInstance.transform.SetParent(this.transform);

            enemyControllers[i] = enemyControllerInstance;
            enemyControllers[i].transform.position = pos;
        }
    }

}
