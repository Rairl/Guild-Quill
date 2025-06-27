using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterController : MonoBehaviour
{
    [SerializeField] public GameObject startDay;
    [SerializeField] public GameObject stamp;

    public void StartDay()
    {
        startDay.SetActive(true);
        stamp.SetActive(true);
    }

    public bool dayActive()
    {
        return startDay.activeInHierarchy;
    }

    public void stopWork()
    {
        startDay.SetActive(false);
        stamp.SetActive(false);
    }
}
