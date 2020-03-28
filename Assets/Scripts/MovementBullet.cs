using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBullet : MonoBehaviour
{
    Rigidbody rigBody;

    // Start is called before the first frame update
    private void Awake()
    {
        rigBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// запуск пули вперед
    /// </summary>
    public void StartBullet(float _spdBullet)
    {     
        rigBody.velocity = Vector3.zero;
        rigBody.angularVelocity = Vector3.zero;
        if (transform.rotation.y != 0) //если пуля повернута в другю сторону
        {
            _spdBullet = -_spdBullet;
        }
        rigBody.AddForce(new Vector3(0, 0, _spdBullet), ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("OnCollisionEnter on " + collision.gameObject.name);
        gameObject.SetActive(false);
    }
}
