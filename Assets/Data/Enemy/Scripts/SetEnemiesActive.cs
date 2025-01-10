using UnityEngine;

public class SetEnemiesActive : MonoBehaviour
{
    [SerializeField] private EnemiesGroupManager enemiesGroupManager;
    [SerializeField] private bool active;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.TryGetComponent<CharacterController>(out var characterController))
        {
            if (active)
            {
                enemiesGroupManager.ActiveEnemies();
            }
            else
            {
                enemiesGroupManager.DeactiveEnemies();
            }
        }
    }

}
