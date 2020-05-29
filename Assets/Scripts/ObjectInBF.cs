using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInBF : MonoBehaviour
{
    [SerializeField] private int hp = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        hp--;
        if (hp <= 0) Destroy(this.gameObject);
    }
}
