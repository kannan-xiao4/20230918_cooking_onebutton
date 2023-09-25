using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipes;

    [SerializeField] private Canvas titleCanvas;
    [SerializeField] private Canvas inGameCanvas;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button gotoTitleButton;
    [SerializeField] private Button restartGameButton;

    [SerializeField] private TMP_Text recipeObjectiveText;
    [SerializeField] private TMP_Text processObjectiveText;
    [SerializeField] private TMP_Text processingText;
    [SerializeField] private TMP_Text resultText;

    [SerializeField] private TMP_Text okSelectResultImage;
    [SerializeField] private TMP_Text ngSelectResultImage;

    [SerializeField] private RouletteSelect rouletteSelect;
    [SerializeField] private RandomIndicator randomIndicator;
    [SerializeField] private CookingProcess cookingProcess;
    [SerializeField] private PistonDrop pistonDrop;

    private GameObject cookedObject;

    private void Awake()
    {
        startGameButton.onClick.AddListener(() => { GameLoop().Forget(); });
        restartGameButton.onClick.AddListener(() => { GameLoop().Forget(); });
        gotoTitleButton.onClick.AddListener(() =>
        {
            titleCanvas.gameObject.SetActive(true);
            inGameCanvas.gameObject.SetActive(false);
        });
    }

    private async UniTaskVoid GameLoop()
    {
        if (cookedObject != null)
        {
            DestroyImmediate(cookedObject);
        }

        titleCanvas.gameObject.SetActive(false);
        inGameCanvas.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);
        gotoTitleButton.gameObject.SetActive(false);
        restartGameButton.gameObject.SetActive(false);

        // 今回作るレシピの決定
        var target = recipes[Random.Range(0, recipes.Count - 1)];
        // var target = recipes.First(x => x.Name == "刺身");

        await PlayTextAnimation($"「{target.Name}」をつくれ！", recipeObjectiveText);
        await PlayTextAnimation($"「{target.BaseFood.GetStringValue()}」を選べ！", processObjectiveText);

        // 素材選択ルーレットのセットアップ
        var rouletteResult1 = BaseFood.Null;
        void RouletteResultHandler1(BaseFood value)
        {
            rouletteResult1 = value;
            rouletteSelect.baseFoodResult -= RouletteResultHandler1;
        }
        rouletteSelect.SetUpTargetMode(RouletteSelect.Mode.BaseFood);
        rouletteSelect.baseFoodResult += RouletteResultHandler1;
        rouletteSelect.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => rouletteResult1 != BaseFood.Null);
        rouletteSelect.gameObject.SetActive(false);

        if (rouletteResult1 != target.BaseFood)
        {
            ngSelectResultImage.gameObject.SetActive(true);
            await UniTask.Delay(500);
            PlayResult(null).Forget();
            return;
        }

        okSelectResultImage.gameObject.SetActive(true);
        await UniTask.Delay(500);
        okSelectResultImage.gameObject.SetActive(false);

        await PlayTextAnimation($"「{target.Seasoning.GetStringValue()}」を選べ！", processObjectiveText);

        // 調味料選択ルーレットのセットアップ
        var rouletteResult2 = Seasoning.Null;
        void RouletteResultHandler2(Seasoning value)
        {
            rouletteResult2 = value;
            rouletteSelect.seasoningResult -= RouletteResultHandler2;
        }
        rouletteSelect.SetUpTargetMode(RouletteSelect.Mode.Seasoning);
        rouletteSelect.seasoningResult += RouletteResultHandler2;
        rouletteSelect.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => rouletteResult2 != Seasoning.Null);
        rouletteSelect.gameObject.SetActive(false);

        if (rouletteResult2 != target.Seasoning)
        {
            ngSelectResultImage.gameObject.SetActive(true);
            await UniTask.Delay(500);
            PlayResult(null).Forget();
            return;
        }

        okSelectResultImage.gameObject.SetActive(true);
        await UniTask.Delay(500);
        okSelectResultImage.gameObject.SetActive(false);

        // 調理前状態の表示アニメーション
        cookingProcess.SetUp(target);
        processObjectiveText.gameObject.SetActive(false);
        await PlayTextAnimation($"下準備中...", processingText, playZoom: false, playMove: true);
        processingText.gameObject.SetActive(false);
        await cookingProcess.PlayPreparedAnimation();

        await PlayTextAnimation($"「{target.Cooking.GetStringValue()}」するぞ！加減を決めろ！", processObjectiveText);

        // 調理工程の加減インジケータのセットアップ
        var indicatorResult = 0f;
        void IndicatorResultHandler(float value)
        {
            indicatorResult = value;
            randomIndicator.resultHandler -= IndicatorResultHandler;
        }
        randomIndicator.resultHandler += IndicatorResultHandler;
        randomIndicator.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => indicatorResult > 0);
        randomIndicator.gameObject.SetActive(false);

        await PlayTextAnimation("調理開始！", processingText, playZoom: true, playMove: false);
        processingText.gameObject.SetActive(false);

        // 調理アニメーション
        GameObject cookedResult = null;
        void CookedResultHandler(GameObject value)
        {
            cookedResult = value;
            cookingProcess.resultHandler -= CookedResultHandler;
        }
        cookingProcess.resultHandler += CookedResultHandler;
        await cookingProcess.PlayCookingAnimation(indicatorResult);
        await UniTask.WaitUntil(() => cookedResult != null);

        pistonDrop.SetUpDropObject(cookedResult);
        await PlayTextAnimation($"落として盛り付けろ！", processObjectiveText);

        GameObject dropResult = null;
        void DropResultHandler(GameObject value)
        {
            dropResult = value;
            pistonDrop.resultHandler -= DropResultHandler;
        }
        pistonDrop.resultHandler += DropResultHandler;
        pistonDrop.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => dropResult != null);
        pistonDrop.gameObject.SetActive(false);

        PlayResult(dropResult).Forget();
    }

    private async UniTask PlayTextAnimation(string prcessText, TMP_Text target, bool playZoom = true, bool playMove = true)
    {
        target.text = prcessText;
        target.gameObject.SetActive(true);
        var animator = target.GetComponent<Animator>();
        animator.Play("Default");

        if (playZoom)
        {
            animator.Play("TextZoom");
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        if (playMove)
        {
            animator.Play("MoveToLeft");
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
    }

    /// <summary>
    /// 盛り付け結果アニメーションさせてリザルト画面にする。
    /// </summary>
    /// <param name="cooked"></param>
    /// <returns></returns>
    private async UniTaskVoid PlayResult(GameObject cooked)
    {
        await UniTask.Delay(100);

        ngSelectResultImage.gameObject.SetActive(false);
        recipeObjectiveText.gameObject.SetActive(false);
        processObjectiveText.gameObject.SetActive(false);
        resultText.gameObject.SetActive(true);
        gotoTitleButton.gameObject.SetActive(true);
        restartGameButton.gameObject.SetActive(true);

        if (cooked == null)
        {
            resultText.text = "何も生み出せなかった";
            return;
        }

        resultText.text = "お料理できたね♪";
        cookedObject = cooked;
    }
}