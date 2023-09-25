using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public delegate void RouletteSelectBaseFood(BaseFood value);
public delegate void RouletteSelectSeasoning(Seasoning value);

public class RouletteSelect : MonoBehaviour
{
    public enum Mode
    {
        BaseFood,
        Seasoning
    }

    public RouletteSelectBaseFood baseFoodResult;
    public RouletteSelectSeasoning seasoningResult;

    [SerializeField] private Button oneButton;
    [SerializeField] private Material outline;
    [SerializeField] private List<GameObject> baseFoodPrefabs;
    [SerializeField] private List<GameObject> seasoningPrefabs;

    private List<GameObject> instantObjects;
    private int selectIndex = 0;
    private bool stopFlag = false;
    private Mode mode;

    private void Start()
    {
        oneButton.onClick.AddListener(() =>
        {
            stopFlag = true;
            if ( mode == Mode.BaseFood )
            {
                var baseFood = instantObjects[selectIndex].GetComponent<BaseFoodObject>();
                baseFoodResult?.Invoke(baseFood.Type);
                Debug.Log($"Select:{selectIndex}, Type:{baseFood.Type}");

            }
            else
            {
                var seasoning = instantObjects[selectIndex].GetComponent<SeasoningObject>();
                seasoningResult?.Invoke(seasoning.Type);
                Debug.Log($"Select:{selectIndex}, Type:{seasoning.Type}");
            }
        });
    }

    public void SetUpTargetMode(Mode mode)
    {
        this.mode = mode;
    }

    private void OnEnable()
    {
        stopFlag = false;
        oneButton.gameObject.SetActive(true);
        var targetPrefabs = mode == Mode.BaseFood ? baseFoodPrefabs : seasoningPrefabs;

        instantObjects = new List<GameObject>();
        for (int i = 0; i < targetPrefabs.Count; i++)
        {
            const int radius = 1;
            var rad = Mathf.Deg2Rad * 360 * i / targetPrefabs.Count;
            var xPos = 0.75f * Mathf.Sin(rad) * radius;
            var zPos = 0.5f * Mathf.Cos(rad) * radius - 1.5f;
            var go = Instantiate(targetPrefabs[i]);
            go.transform.position = new Vector3(xPos, 2.5f, zPos);
            instantObjects.Add(go);
        }

        RandomSelect(instantObjects).Forget();
    }

    private void OnDisable()
    {
        stopFlag = true;
        oneButton.gameObject.SetActive(false);
        foreach (var go in instantObjects)
        {
            Destroy(go);
        }
        instantObjects.Clear();
    }

    private async UniTaskVoid RandomSelect(List<GameObject> instantPrefabs)
    {
        var meshRenderers = instantPrefabs.Select(x => x.GetComponentInChildren<MeshRenderer>()).ToList();
        while (!stopFlag && instantPrefabs.Any(x => x != null))
        {
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                if (i == selectIndex)
                {
                    var materials = meshRenderers[i].materials;
                    Array.Resize(ref materials, materials.Length + 1);
                    materials[materials.Length - 1] = outline;
                    meshRenderers[i].materials = materials;
                }
                else
                {
                    var materials = meshRenderers[i].materials.ToList();
                    var index = materials.FindIndex(x => x.shader == outline.shader);
                    if (index < 0)
                    {
                        continue;
                    }
                    materials.RemoveAt(index);
                    meshRenderers[i].materials = materials.ToArray();
                }
            }
            await UniTask.Delay(500);
            selectIndex++;
            selectIndex %= meshRenderers.Count;
        }
    }
}
