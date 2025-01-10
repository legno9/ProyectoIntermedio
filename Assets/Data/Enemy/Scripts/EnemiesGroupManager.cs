using System.Collections.Generic;
using UnityEngine;

public class EnemiesGroupManager : MonoBehaviour
{
    private List<GameObject> enemies = new List<GameObject>();

    void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<EnemyController>(out var enemyController)){ enemies.Add(child.gameObject); }
        }

        DeactiveEnemies();
    }

    public void ActiveEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy?.SetActive(true);
        }
    }

    public void DeactiveEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy?.SetActive(false);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    
}
