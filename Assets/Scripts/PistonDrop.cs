using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public delegate void PistonDropResult(GameObject merged);

public class PistonDrop : MonoBehaviour
{
    public PistonDropResult resultHandler;

    [SerializeField] private Button oneButton;
    [SerializeField] private GameObject dishPrefab;

    private int circleIndex = 0;
    private GameObject dropObject;
    private Rigidbody dropRigid;
    private GameObject dishObject;

    // Start is called before the first frame update
    private void Start()
    {
        oneButton.onClick.AddListener(async() =>
        {
            dropRigid.useGravity = true;
            dropRigid.isKinematic = false;
            await UniTask.Delay(1000);
            // todo: evaluate drop object score
            dropObject.transform.SetParent(dishObject.transform, true);
            resultHandler?.Invoke(dishObject);
        });
    }

    public void SetUpDropObject(GameObject drop)
    {
        dropObject = drop;
        dropObject.transform.position = Vector3.up * 2;
    }

    private void OnEnable()
    {
        oneButton.gameObject.SetActive(true);
        dishObject = Instantiate(dishPrefab, Vector3.zero, Quaternion.identity);
        dropRigid = dropObject.AddComponent<Rigidbody>();
        dropRigid.isKinematic = true;
    }

    private void OnDisable()
    {
        oneButton.gameObject.SetActive(false);
        //Destroy(dropObject);
        //dropObject = null;
        dishObject = null;
        Destroy(dropRigid);
        dropRigid = null;
    }

    private void FixedUpdate()
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
        circleIndex += 3;
    }
}
