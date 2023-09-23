using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] Canvas titleCanvas;
    [SerializeField] Canvas inGameCanvas;
    [SerializeField] Canvas resultCanvas;

    [SerializeField] private Button startGameButton;
    [SerializeField] private Button gotoTitleButton;
    [SerializeField] private Button restartGameButton;

    [SerializeField] private RouletteSelect rouletteSelect;
    [SerializeField] private RandomIndicator randomIndicator;
    [SerializeField] private PistonDrop pistonDrop;

    private void Awake()
    {
        startGameButton.onClick.AddListener(() => { GameLoop().Forget(); });
        restartGameButton.onClick.AddListener(() => { GameLoop().Forget(); });
        gotoTitleButton.onClick.AddListener(() =>
        {
            titleCanvas.gameObject.SetActive(true);
            resultCanvas.gameObject.SetActive(false);
        });
    }

    private async UniTaskVoid GameLoop()
    {
        titleCanvas.gameObject.SetActive(false);
        resultCanvas.gameObject.SetActive(false);
        inGameCanvas.gameObject.SetActive(true);

        var rouletteResult = 0;
        void RouletteResultHandler(int value)
        {
            rouletteResult = value;
            rouletteSelect.resultHandler -= RouletteResultHandler;
        }
        rouletteSelect.resultHandler += RouletteResultHandler;
        rouletteSelect.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => rouletteResult > 0);
        rouletteSelect.gameObject.SetActive(false);

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

        var dropResult = 0;
        void DropResultHandler(int value)
        {
            dropResult = value;
            pistonDrop.resultHandler -= DropResultHandler;
        }
        pistonDrop.resultHandler += DropResultHandler;
        pistonDrop.gameObject.SetActive(true);
        await UniTask.WaitUntil(() => dropResult > 0);
        pistonDrop.gameObject.SetActive(false);

        Debug.Log($"result roulette:{rouletteResult}, indicator:{indicatorResult:f2}, drop:{dropResult}");
        inGameCanvas.gameObject.SetActive(false);
        resultCanvas.gameObject.SetActive(true);
    }

}