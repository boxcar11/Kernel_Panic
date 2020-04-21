using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Materials
{
    public materialEnum materialName;
    public int count;
    public Sprite spriteImage;
}

[System.Serializable]
public class Components
{
    public componentEnum componentName;
    public int count;
    public float health = 100;
    public Sprite spriteImage;
}

[System.Serializable]
public class Recipes
{
    public componentEnum componentName;
    public Sprite spriteImage;
    public Materials[] requiredMaterials;
}

public enum materialEnum
{
    Silicon,
    Copper,
    Iron
}

public enum componentEnum
{
    Ram,
    PSU,
    Processor,
    Harddrive,
    SatelliteUplink
}
