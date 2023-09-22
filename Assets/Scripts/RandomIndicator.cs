using UnityEngine;
using UnityEngine.UI;

public class RandomIndicator : MonoBehaviour
{
    [SerializeField] private Slider randomIndicator;
    [SerializeField] private GameObject indicateObject;

    private int circleIndex = 0;

    // Start is called before the first frame update
    private void Start()
    {
        randomIndicator.gameObject.SetActive(true);

        var go = Instantiate(indicateObject, Vector3.up, Quaternion.identity);
        randomIndicator.onValueChanged.AddListener(value =>
        {
            var current = go.transform.localScale;
            go.transform.localScale = new Vector3(value * 5, current.y, current.z);
        });
    }

    // Update is called once per frame
    private void Update()
    {
        if (circleIndex >= 360)
        {
            circleIndex = 0;
        }

        randomIndicator.value = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * circleIndex));
        circleIndex++;
    }
}
