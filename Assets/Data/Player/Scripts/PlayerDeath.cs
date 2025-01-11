using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    // [SerializeField] private float delay = 5f;

    private void OnEnable()
    {
        GetComponent<EntityHealth>().OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        GetComponent<EntityHealth>().OnDeath.RemoveListener(OnDeath);
    }

    private void OnDeath()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponentInChildren<Animator>().enabled = false;

        foreach (var component in gameObject.GetComponents<Component>())
        {
            if (component is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
        }

        foreach (HitCollider hit in gameObject.GetComponentsInChildren<HitCollider>(true))
        {
            hit.gameObject.SetActive(false);
        }

        foreach (HurtCollider hurt in gameObject.GetComponentsInChildren<HurtCollider>(true))
        {
            hurt.enabled = false;
        }

        GetComponentInChildren<Ragdollizer>().Ragdollize();

        Invoke("LoadDefeatScene", 3f);
    }

    private void LoadDefeatScene()
    {
        SceneManager.LoadScene("DefeatScene");
    }
}
