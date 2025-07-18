using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableQuestManager : MonoBehaviour
{
    public List<TableQuestController> tableQuests;
    public float closeAnimationDuration = 1.0f;

    public void OpenAll()
    {
        foreach (var quest in tableQuests)
        {
            quest.PlayOpen();
        }
    }

    public void CloseAll()
    {
        StartCoroutine(CloseAndDeactivate());
    }

    private IEnumerator CloseAndDeactivate()
    {
        foreach (var quest in tableQuests)
        {
            quest.PlayClose();
        }

        yield return new WaitForSeconds(closeAnimationDuration);

        foreach (var quest in tableQuests)
        {
            quest.gameObject.SetActive(false);
        }
    }
}
