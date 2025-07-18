using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip rolldownsfx;

    void Start()
    {
        audioSource.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
            audioSource.PlayOneShot(rolldownsfx);
    }
}
