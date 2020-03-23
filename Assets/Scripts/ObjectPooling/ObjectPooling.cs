using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// если хз что такое пул - https://pikabu.ru/story/tutorial_prostoy_i_udobnyiy_pul_obektov_dlya_unity3d_5613202
// описание кода этого пула - https://habr.com/ru/post/275091/
/// <summary>
/// класс-пул обьектов, который хранит в себе определенное кол-во экземпляров префаба
/// </summary>
public class ObjectPooling
{
    List<PoolObject> objects;
    Transform objectsParent;

    public void Initialize(int count, PoolObject sample, Transform _objectsParent) //инициализация пула 
    {
        objects = new List<PoolObject>();
        objectsParent = _objectsParent;
        for (int i = 0; i < count; i++)
        {
            AddObject(sample, _objectsParent);
        }
    }

    void AddObject(PoolObject sample, Transform _objectsParent) //добавляем обьект в пул 
    {
        GameObject temp = GameObject.Instantiate(sample.gameObject);
        temp.name = sample.name;
        temp.transform.SetParent(_objectsParent);
        objects.Add(temp.GetComponent<PoolObject>());
        /*if (temp.GetComponent<Animator>())
            temp.GetComponent<Animator>().StartPlayback();*/ //мб нужно будет включить, если будет лагать при вклчении большого колва пул-обьектов
        temp.SetActive(false);
    }

    public PoolObject GetObject() //получение обьекта из пула
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].gameObject.activeInHierarchy == false)
            {
                return objects[i];
            }
        }
        AddObject(objects[0], objectsParent);
        return objects[objects.Count - 1];
    }
}
