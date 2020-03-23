using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// если хз что такое пул - https://pikabu.ru/story/tutorial_prostoy_i_udobnyiy_pul_obektov_dlya_unity3d_5613202
// описание кода этого пула - https://habr.com/ru/post/275091/
/// <summary>
/// манагер, управляющий всеми пулами (ObjectPooling) на сцене
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null; //это синглтон, окгугл)

    [SerializeField]
    PoolPart[] pools;

    GameObject objectsParant;

    [System.Serializable]
    struct PoolPart //описывает определенный пулл
    {
        [HideInInspector] public string name; 
        public PoolObject poolPref; 
        public int count;
        [HideInInspector] public ObjectPooling objPooling;
    }

    void Awake()
    {
        instance = this;
        Initialize();
    }

    void Initialize()
    {
        objectsParant = new GameObject();
        objectsParant.name = "Pool";
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].poolPref != null)
            {
                pools[i].name = pools[i].poolPref.name;
                pools[i].objPooling = new ObjectPooling();
                pools[i].objPooling.Initialize(pools[i].count, pools[i].poolPref, objectsParant.transform);
            }
        }
    }

    /// <summary>
    /// выдергивает обьект из пула _name
    /// </summary>
    public GameObject GetObject(string _name, Vector3 _position, Quaternion _rotation)
    {
        GameObject result = null;
        if (pools != null)
        {
            for (int i = 0; i < pools.Length; i++)
            {
                if (string.Compare(pools[i].name, _name) == 0)
                {
                    result = pools[i].objPooling.GetObject().gameObject;
                    result.transform.position = _position;
                    result.transform.rotation = _rotation;
                    result.SetActive(true);
                    return result;
                }
            }
        }
        return result;
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
