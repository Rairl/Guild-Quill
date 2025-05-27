using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterController : MonoBehaviour
{
    [SerializeField] public GameObject startDay;

    public void StartDay()
    {
        startDay.SetActive(true);
    }

    public bool dayActive()
    {
        return startDay.activeInHierarchy;
    }

    public void stopWork()
    {
        startDay.SetActive(false);
    }
}
