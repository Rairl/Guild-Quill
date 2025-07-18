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
    public GameObject gameObject;
    public string name;
    public List<Trait> traits = new();
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
        int oldMood = (int)mood;
        int newMood = Mathf.Clamp(oldMood + delta, 0, 3);
        mood = (Mood)newMood;

        Debug.Log($"[Mood Change] {name}'s mood changed from {(Mood)oldMood} to {mood} (delta: {delta})");
    }

    public bool IsActive() => daysToSkip <= 0;
}
