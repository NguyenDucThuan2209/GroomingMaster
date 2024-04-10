using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectPoolSO", menuName = "HyrphusQ/ObjectPoolSO/GameObject")]
public class GameObjectPoolSO : ObjectPoolSO<GameObject>
{
    protected override void DestroyMethod(GameObject item)
    {
        Destroy(item);
    }

    protected override GameObject InstantiateMethod()
    {
        var instance = Instantiate(prefabObject, anchoredTransform);
        return instance;
    }
}
