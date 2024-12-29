using System.Collections.Generic;
using UnityEngine;

public class HeardDetector : MonoBehaviour
{
    [SerializeField] private float hearingRange = 15f;  // Rango de escucha de la IA
    [SerializeField] private float soundPersistence = 5f;  // Tiempo que la IA recuerda el sonido
    public List<GameObject> objects = new();
    private Dictionary<GameObject, float > objectsHeared = new();

    private void Update() 
    {
        foreach (var item in objectsHeared)
        {
            objectsHeared[item.Key] -= Time.deltaTime;

            if (soundPersistence <= 0)
            {
                objects.Remove(item.Key);
                objectsHeared.Remove(item.Key);
            }
        }
    }

    public bool IsTargetDetected(GameObject target)
    {
        return objects.Contains(target);
    }

    // Método para recibir el sonido
    public void HearSound(Vector3 soundOrigin, GameObject soundSource)
    {
        float distance = Vector3.Distance(transform.position, soundOrigin);

        if (distance <= hearingRange)
        {
            Debug.Log("IA ha escuchado el sonido desde: " + soundOrigin + " a una distancia de " + distance);
            if (!objects.Contains(soundSource))
            {
                objects.Add(soundSource);
                objectsHeared.Add(soundSource, soundPersistence);
            }
            else
            {
                objectsHeared[soundSource] = soundPersistence;
            }
        }
    }
}
