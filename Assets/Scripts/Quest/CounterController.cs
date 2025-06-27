using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterController : MonoBehaviour
{
    [SerializeField] public GameObject startDay;
    [SerializeField] public GameObject stamp;
    [SerializeField] public GameObject questPaper1;
    [SerializeField] public GameObject questPaper2;
    [SerializeField] public GameObject questPaper3;
    [SerializeField] public GameObject questPaper4;
    [SerializeField] public GameObject questPaper5;
    [SerializeField] public GameObject questPaper6;
    [SerializeField] public GameObject questPaper7;
    [SerializeField] public GameObject questPaper8;
    [SerializeField] public GameObject questPaper9;
    [SerializeField] public GameObject questPaper10;
    [SerializeField] public GameObject questFlip;

    public void StartDay()
    {
        startDay.SetActive(true);
        stamp.SetActive(true);
        questPaper1.SetActive(true);
        questPaper2.SetActive(true);
        questPaper3.SetActive(true);
        questPaper4.SetActive(true);
        questPaper5.SetActive(true);
        questPaper6.SetActive(true);
        questPaper7.SetActive(true);
        questPaper8.SetActive(true);
        questPaper9.SetActive(true);
        questPaper10.SetActive(true);
        questFlip.SetActive(true);
    }

    public bool dayActive()
    {
        return startDay.activeInHierarchy;
    }

    public void stopWork()
    {
        startDay.SetActive(false);
        stamp.SetActive(false);
        questPaper1.SetActive(false);
        questPaper2.SetActive(false);
        questPaper3.SetActive(false);
        questPaper4.SetActive(false);
        questPaper5.SetActive(false);
        questPaper6.SetActive(false);
        questPaper7.SetActive(false);
        questPaper8.SetActive(false);
        questPaper9.SetActive(false);
        questPaper10.SetActive(false);
        questFlip.SetActive(false);
    }
}
