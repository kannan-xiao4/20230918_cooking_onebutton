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
    /// 調理前状態の表示アニメーション
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
    /// 2秒ぐらいかけて、鍋とかにかけて、料理の色をインジケーターの結果に合わせて変える
    /// 切る場合は、素材の魚をまな板に載せて切り、値が足りれば刺身に、そうでなければ素材のまま
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
