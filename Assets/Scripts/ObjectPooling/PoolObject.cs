using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// это просто скрипт, который должен быть на всех пул-обьектах. 
/// </summary>
public class PoolObject : MonoBehaviour
{
    /// <summary>
    /// возврат обьекта в пул
    /// </summary>
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
