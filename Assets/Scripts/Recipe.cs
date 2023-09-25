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
    [StringValue("�Ȃ�")]
    Null,
    [StringValue("��")]
    Fish,
    [StringValue("�{��")]
    Chicken,
    [StringValue("�ؓ�")]
    Pork,
    [StringValue("����")]
    Beef
}

public enum Seasoning
{
    [StringValue("�Ȃ�")]
    Null,
    [StringValue("������")]
    Flour,
    [StringValue("�p����")]
    BreadCrumbs,
    [StringValue("����")]
    SoySauce,
    [StringValue("�f�ނ̖�")]
    None
}

public enum Cooking
{
    [StringValue("�؂�")]
    Cut,
    [StringValue("�g����")]
    Fry,
    [StringValue("�ς�")]
    Stew,
    [StringValue("�Ă�")]
    Grill
}
