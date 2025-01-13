using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [SerializeField] private BossController bossController;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private CanvasGroup bossCanvas;
    [SerializeField] private AudioSource introMusic;
    private Animator animator;
    private bool bossActive = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!bossActive)
        {
            bossController.Activate();
            animator.SetTrigger("CloseDoor");
            bossActive = true;
            musicSource.enabled = true;
            bossCanvas.gameObject.SetActive(true);
            introMusic.Stop();
        }
    }
}
