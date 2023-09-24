using System;

[Serializable]
public class Recipe
{
    public string Name;
    public BaseFood BaseFood;
    public Seasoning Seasoning;
    public Cooking Cooking;
    public UnityEngine.GameObject Prefab;
    public UnityEngine.Vector3 cookPosition;
}

public enum BaseFood
{
    Null,
    Fish,
    Chicken,
    Pork,
    Beef
}

public enum Seasoning
{
    Null,
    Flour,
    BreadCrumbs,
    SoySauce,
    None
}

public enum Cooking
{
    Cut,
    Fry,
    Stew,
    Grill
}
