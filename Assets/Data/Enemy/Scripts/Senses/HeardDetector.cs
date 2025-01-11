using System.Collections.Generic;
using UnityEngine;

public class HeardDetector : MonoBehaviour
{
    [SerializeField] private float hearingRange = 15f;  // Rango de escucha de la IA
    [SerializeField] private float soundPersistence = 5f;  // Tiempo que la IA recuerda el sonido
    private List<GameObject> objects = new();
    private Dictionary<GameObject, float > objectsHeared = new();
    private GameObject target;

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
        this.target = target;
        return objects.Contains(target);
    }

    // Método para recibir el sonido
    public void HearSound(Vector3 soundOrigin, GameObject soundSource, bool isFromPlayer)
    {    
        if (isFromPlayer && target != null){soundSource = target;}
        
        float distance = Vector3.Distance(transform.position, soundOrigin);

        if (distance <= hearingRange)
        {
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
