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
        List<GameObject> itemsToRemove = new();
        Dictionary<GameObject, float> tempObjectsHeared = new(objectsHeared);

        foreach (var kvp in tempObjectsHeared)
        {
            objectsHeared[kvp.Key] -= Time.deltaTime;

            if (objectsHeared[kvp.Key] <= 0)
            {
                itemsToRemove.Add(kvp.Key);
            }
        }

        foreach (GameObject item in itemsToRemove)
        {
            objectsHeared.Remove(item);
            objects.Remove(item);
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
            if (!objects.Contains(soundSource))
            {
                Debug.Log("IA ha escuchado el sonido de " + soundSource + " a una distancia de " + distance);
                objects.Add(soundSource);
                objectsHeared.Add(soundSource, soundPersistence);
            }
            else
            {
                Debug.Log("IA ha vuelto a escuchar el sonido de " + soundSource + "a una distancia de " + distance);
                objectsHeared[soundSource] = soundPersistence;
            }
        }
    }
}
