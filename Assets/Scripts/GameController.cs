using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button oneButton;
    [SerializeField] private Material outline;
    [SerializeField] private List<GameObject> prefabs;

    private int selectIndex = 0;
    private bool stopFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        oneButton.onClick.AddListener(() =>
        {
            stopFlag = true;
            Debug.Log(selectIndex);
        });

        var instantPrefabs = new List<GameObject>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            const int radius = 2;
            var rad = Mathf.Deg2Rad * 360 * i / prefabs.Count;
            var xPos = Mathf.Sin(rad) * radius;
            var zPos = Mathf.Cos(rad) * radius;
            var go = Instantiate(prefabs[i]);
            go.transform.position = new Vector3(xPos, 1, zPos);
            instantPrefabs.Add(go);
        }

        RandomSelect(instantPrefabs).Forget();
    }

    private async UniTaskVoid RandomSelect(List<GameObject> instantPrefabs)
    {
        var meshRenderers = instantPrefabs.Select(x => x.GetComponentInChildren<MeshRenderer>()).ToList();
        while (!stopFlag)
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
            await UniTask.Delay(50);
            selectIndex++;
            selectIndex %= meshRenderers.Count;
        }
    }
}
