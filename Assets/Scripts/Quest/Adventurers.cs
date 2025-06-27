using System.Collections.Generic;
using UnityEngine;

public enum TraitType { Positive, Negative }

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

    public Adventurer(GameObject obj, string name)
    {
        this.gameObject = obj;
        this.name = name;
    }
}
