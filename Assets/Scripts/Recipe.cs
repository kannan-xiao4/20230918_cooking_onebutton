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
    [StringValue("‚È‚µ")]
    Null,
    [StringValue("‹›")]
    Fish,
    [StringValue("Œ{“÷")]
    Chicken,
    [StringValue("“Ø“÷")]
    Pork,
    [StringValue("‹“÷")]
    Beef
}

public enum Seasoning
{
    [StringValue("‚È‚µ")]
    Null,
    [StringValue("¬”•²")]
    Flour,
    [StringValue("ƒpƒ“•²")]
    BreadCrumbs,
    [StringValue("‚½‚ê")]
    SoySauce,
    [StringValue("‘fŞ‚Ì–¡")]
    None
}

public enum Cooking
{
    [StringValue("Ø‚é")]
    Cut,
    [StringValue("—g‚°‚é")]
    Fry,
    [StringValue("Ï‚é")]
    Stew,
    [StringValue("Ä‚­")]
    Grill
}
