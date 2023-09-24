using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public delegate void RandomIndicatorResult(float value);

public class RandomIndicator : MonoBehaviour
{
    public RandomIndicatorResult resultHandler;

    [SerializeField] private int multiplyForIndicate;
    [SerializeField] private Button oneButton;
    [SerializeField] private Slider indicateSlider;
    //[SerializeField] private GameObject indicatePrefab;

    private GameObject indicateObject;
    private int circleIndex = 0;
    private bool stopFlag = false;

    private void Start()
    {
        oneButton.onClick.AddListener(async() =>
        {
            stopFlag = true;
            oneButton.interactable = false;
            //var result = indicateObject.transform.localScale.x / multiplyForIndicate;
            var result = indicateSlider.value;
            Debug.Log($"IndicateValue: {result}");

            await UniTask.Delay(50);
            resultHandler?.Invoke(result);
        });
    }

    private void OnEnable()
    {
        stopFlag = false;
        oneButton.gameObject.SetActive(true);
        oneButton.interactable = true;
        indicateSlider.gameObject.SetActive(true);
        //indicateObject = Instantiate(indicatePrefab, Vector3.up, Quaternion.identity);
    }

    private void OnDisable()
    {
        stopFlag = true;
        oneButton.gameObject.SetActive(false);
        indicateSlider.gameObject.SetActive(false);
        //Destroy(indicateObject);
        //indicateObject = null;
    }

    private void FixedUpdate()
    {
        if (stopFlag)
        {
            return;
        }

        if (circleIndex >= 360)
        {
            circleIndex = 0;
        }

        var xScale = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * circleIndex));
        //var current = indicateObject.transform.localScale;
        //indicateObject.transform.localScale = new Vector3(xScale * multiplyForIndicate, current.y, current.z);
        indicateSlider.value = xScale;
        circleIndex += 3;
    }
}
