using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(EnemyGroup))]
public class EnemyGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyGroup enemyGroup = (EnemyGroup)target;

        if(GUILayout.Button("Init Enemy Group"))
        {
            enemyGroup.FormatTheShapeOfTheEnemyGroup();
        }
    }
}
