using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableQuestController : MonoBehaviour
{
    [Header("Animation Names")]
    public string openAnimationName;
    public string closeAnimationName;

    [Header("Animation Durations")]
    public float openAnimationDuration = 1.0f;  // Set this to match your open clip length
    public float closeAnimationDuration = 1.0f; // Set this to match your close clip length

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayOpen()
    {
        gameObject.SetActive(true);

        if (animator != null)
        {
            animator.enabled = true; // Just in case it's been disabled from a previous open
            animator.Play(openAnimationName, 0, 0f);
            StartCoroutine(DisableAnimatorAfterDelay(openAnimationDuration));
        }
    }

    public void PlayClose()
    {
        if (animator != null)
        {
            animator.enabled = true; // Re-enable if previously disabled
            animator.Play(closeAnimationName, 0, 0f);
        }

        StartCoroutine(DeactivateAfterDelay(closeAnimationDuration));
    }

    private IEnumerator DisableAnimatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
            animator.enabled = false; // Prevent it from overriding drag movement
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
