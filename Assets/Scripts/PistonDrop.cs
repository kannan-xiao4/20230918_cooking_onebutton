using UnityEngine;
using UnityEngine.UI;

public class PistonDrop : MonoBehaviour
{
    [SerializeField] private Button oneButton;
    [SerializeField] private GameObject dropPrefab;

    private int circleIndex = 0;
    private GameObject dropObject;
    private Rigidbody dropRigid;

    // Start is called before the first frame update
    private void Start()
    {
        oneButton.gameObject.SetActive(true);
        oneButton.onClick.AddListener(() =>
        {
            dropRigid.useGravity = true;
            dropRigid.isKinematic = false;
        });

        dropObject = Instantiate(dropPrefab, Vector3.up * 2, Quaternion.identity);
        dropRigid = dropObject.AddComponent<Rigidbody>();
        dropRigid.isKinematic = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!dropRigid.isKinematic)
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
