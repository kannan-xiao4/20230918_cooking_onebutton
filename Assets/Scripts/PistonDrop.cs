using UnityEngine;
using UnityEngine.UI;

public delegate void PistonDropResult(int value);

public class PistonDrop : MonoBehaviour
{
    public PistonDropResult resultHandler;

    [SerializeField] private Button oneButton;
    [SerializeField] private GameObject dropPrefab;

    private int circleIndex = 0;
    private GameObject dropObject;
    private Rigidbody dropRigid;

    // Start is called before the first frame update
    private void Start()
    {
        oneButton.onClick.AddListener(() =>
        {
            dropRigid.useGravity = true;
            dropRigid.isKinematic = false;
            // todo: evaluate drop object score
            resultHandler?.Invoke(10);
        });
    }

    private void OnEnable()
    {
        oneButton.gameObject.SetActive(true);
        dropObject = Instantiate(dropPrefab, Vector3.up * 2, Quaternion.identity);
        dropRigid = dropObject.AddComponent<Rigidbody>();
        dropRigid.isKinematic = true;
    }

    private void OnDisable()
    {
        oneButton.gameObject.SetActive(false);
        Destroy(dropObject);
        dropObject = null;
        dropRigid = null;
    }

    // Update is called once per frame
    private void Update()
    {
        if(dropObject == null || dropRigid == null || !dropRigid.isKinematic)
        {
            return;
        }

        if (circleIndex >= 360)
        {
            circleIndex = 0;
        }

        var tempPos = dropObject.transform.position;
        var xPos = Mathf.Sin(Mathf.Deg2Rad * circleIndex) * 2;
        dropObject.transform.position = new Vector3(xPos, tempPos.y, tempPos.z);
        circleIndex++;
    }
}
