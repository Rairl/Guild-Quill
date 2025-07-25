using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawners : MonoBehaviour
{
    public GameObject textObject;
    public GameObject button1;
    public GameObject button2;
    public GameObject StampSprite;
    public float delay = 2f; // Seconds to wait

    void Start()
    {
        StartCoroutine(ShowUI());  
    }

    IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(delay);

        textObject.SetActive(true);
        button1.SetActive(true);
        button2.SetActive(true);
        StampSprite.SetActive(true);
    }
}
