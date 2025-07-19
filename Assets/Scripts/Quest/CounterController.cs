using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterController : MonoBehaviour
{
    [SerializeField] public GameObject startDay;
    [SerializeField] public GameObject stamp;
    [SerializeField] public GameObject questFlip;
    [SerializeField] public GameObject dayLevel;
    [SerializeField] public GameObject daySpawns;
    [SerializeField] public GameObject mainLevel;
    [SerializeField] public GameObject mainSpawns;
    [SerializeField] public GameObject buttonOpen;
    [SerializeField] public GameObject instruction1;
    [SerializeField] public GameObject instruction2;
    [SerializeField] public GameObject uiCanvas;

    public void StartDay()
    {
        startDay.SetActive(true);
        stamp.SetActive(true);
        questFlip.SetActive(true);
        dayLevel.SetActive(true);
        daySpawns.SetActive(true);
        mainLevel.SetActive(false);
        mainSpawns.SetActive(false);
        buttonOpen.SetActive(true);
        instruction1.SetActive(false);
        instruction2.SetActive(false);
        uiCanvas.SetActive(true);
    }

    public bool dayActive()
    {
        return startDay.activeInHierarchy;
    }

    public void stopWork()
    {
        startDay.SetActive(false);
        stamp.SetActive(false);
        questFlip.SetActive(false);
        dayLevel.SetActive(false);
        daySpawns.SetActive(false);
        mainLevel.SetActive(true);
        mainSpawns.SetActive(true);
        buttonOpen.SetActive(false);
        instruction1.SetActive(true);
        instruction2.SetActive(true);
    }
}
