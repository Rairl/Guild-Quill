using System.Collections.Generic;
using UnityEngine;

public enum TraitType { Positive, Negative }
public enum Mood { Good, Neutral, Bad, VeryBad }

[System.Serializable]
public class Trait
{
    public string name;
    public TraitType type;
}

public class Adventurer
{
    //Traits
    public GameObject gameObject;
    public string name;
    public List<Trait> traits = new();
    //Moods
    public Mood mood = Mood.Good;
    public bool wantsToChangeQuest = false;
    public int daysToSkip = 0;
    public Adventurer(GameObject obj, string name)
    {
        this.gameObject = obj;
        this.name = name;
    }

    public void ChangeMood(int delta)
    {
        int moodInt = Mathf.Clamp((int)mood + delta, 0, 3);
        mood = (Mood)moodInt;
    }

    public bool IsActive() => daysToSkip <= 0;
}
