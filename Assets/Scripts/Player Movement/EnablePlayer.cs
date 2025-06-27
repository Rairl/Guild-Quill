using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayer : MonoBehaviour
{

    public GameObject movingPlayer;
    public GameObject staticPlayer;

    public CounterController activeCounter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            activeCounter.stopWork();

            staticPlayer.SetActive(false);
            movingPlayer.SetActive(true);
        }
    }
}
