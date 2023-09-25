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

    [SerializeField] private GameObject fishObject;

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
        if (recipe.Cooking == Cooking.Cut)
        {
            cookingObject = Instantiate(fishObject, recipe.cookPosition, Quaternion.identity);
        }
        else
        {
            cookingObject = Instantiate(recipe.Prefab, recipe.cookPosition, Quaternion.identity);
            var renderers = cookingObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var material in renderers.SelectMany(x => x.materials))
            {
                material.SetFloat(Keyword, 0.5f);
            }
        }

        await UniTask.Delay(100);

        var equip = GetEquipment();
        equip.gameObject.SetActive(true);
        konroObject.gameObject.SetActive(recipe.Cooking != Cooking.Cut);
    }

    /// <summary>
    /// 2�b���炢�����āA��Ƃ��ɂ����āA�����̐F���C���W�P�[�^�[�̌��ʂɍ��킹�ĕς���
    /// �؂�ꍇ�́A�f�ނ̋����܂Ȕɍڂ��Đ؂�A�l�������Ύh�g�ɁA�����łȂ���Αf�ނ̂܂�
    /// </summary>
    /// <returns></returns>
    public async UniTask PlayCookingAnimation(float target)
    {
        // todo play cookanimation
        var cookedObject = cookingObject;
        if (recipe.Cooking == Cooking.Cut)
        {
            var animator = GetEquipment().GetComponentInChildren<Animator>();
            animator.Play("CutAnime");
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            if (target >= 0.8f)
            {
                DestroyImmediate(cookedObject);
                cookedObject = Instantiate(recipe.Prefab, recipe.cookPosition, Quaternion.identity);
            }
        }
        else
        {
            var renderers = cookingObject.GetComponentsInChildren<MeshRenderer>();
            var current = renderers[0].material.GetFloat(Keyword);
            while (current > target)
            {
                current -= 0.01f;
                foreach (var material in renderers.SelectMany(x => x.materials))
                {
                    material.SetFloat(Keyword, current);
                }
                await UniTask.Delay(100);
            }
        }

        resultHandler?.Invoke(cookedObject);
        GetEquipment().gameObject.SetActive(false);
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
