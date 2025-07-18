using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    public RectTransform creditsText;
    public float scrollSpeed = 30f;

    public float pauseAfterSeconds = 5f;
    public float waitBeforeMainMenu = 5f;    //  Delay before loading main menu
    public bool isScrolling = true;

    private CanvasGroup canvasGroup;

    void Start()
    {

        StartCoroutine(PauseAfterTime());
    }

    void Update()
    {
        if (!isScrolling) return;

        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        float currentY = creditsText.anchoredPosition.y;


    }

    private System.Collections.IEnumerator PauseAfterTime()
    {
        yield return new WaitForSeconds(pauseAfterSeconds);
        isScrolling = false;

        yield return new WaitForSeconds(waitBeforeMainMenu);
        SceneManager.LoadScene("MenuScene");
    }
}
