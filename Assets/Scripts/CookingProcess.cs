using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

public delegate void CookingResult(GameObject coocked);

public class CookingProcess : MonoBehaviour
{
    public CookingResult resultHandler;

    [SerializeField] private GameObject panObject;
    [SerializeField] private GameObject nabeObject;
    [SerializeField] private GameObject knifeObject;
    [SerializeField] private GameObject konroObject;

    private Recipe recipe;
    private GameObject cookingObject;

    const string Keyword = "_Multiplay";

    public void SetUp(Recipe recipe)
    {
        this.recipe = recipe;
        this.cookingObject = null;
    }

    /// <summary>
    /// �����O��Ԃ̕\���A�j���[�V����
    /// </summary>
    /// <returns></returns>
    public async UniTask PlayPreparedAnimation()
    {
        cookingObject = Instantiate(recipe.Prefab, recipe.cookPosition, Quaternion.identity);
        var renderers = cookingObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var material in renderers.SelectMany(x => x.materials))
        {
            material.SetFloat(Keyword, 0.5f);
        }

        await UniTask.Delay(100);
    }

    /// <summary>
    /// 2�b���炢�����āA��Ƃ��ɂ����āA�����̐F���C���W�P�[�^�[�̌��ʂɍ��킹�ĕς���
    /// �؂�ꍇ�́A�f�ނ̋����܂Ȕɍڂ��Đ؂�A�l�������Ύh�g�ɁA�����łȂ���Αf�ނ̂܂�
    /// </summary>
    /// <returns></returns>
    public async UniTask PlayCookingAnimation(float target)
    {
        var equip = GetEquipment();
        equip.gameObject.SetActive(true);
        // play animation eqio
        konroObject.gameObject.SetActive(recipe.Cooking != Cooking.Cut);

        var renderers = cookingObject.GetComponentsInChildren<MeshRenderer>();
        var current = renderers[0].material.GetFloat(Keyword);
        // ToDo: move to cooking object to equip
        while(current > target)
        {
            current -= 0.01f;
            foreach (var material in renderers.SelectMany(x => x.materials))
            {
                material.SetFloat(Keyword, current);
            }
            await UniTask.Delay(100);
        }

        resultHandler?.Invoke(cookingObject);
        equip.gameObject.SetActive(false);
        konroObject.gameObject.SetActive(false);
    }

    private GameObject GetEquipment()
    {
        return recipe.Cooking switch
        {
            Cooking.Cut => knifeObject,
            Cooking.Grill => panObject,
            _ => nabeObject,
        };
    }

    private void OnDestroy()
    {
        recipe = null;
        cookingObject = null;
    }
}
