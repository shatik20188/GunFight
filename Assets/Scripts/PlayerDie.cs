using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerDie : MonoBehaviour
{
    [SerializeField] float impulseBack = 3;
    [SerializeField] float impulsePower = 1;

    Animator animator;
    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        SetRbKinematicState(true);
        SetColliderState(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            SetContrScriptsState(false);
        }

        animator.enabled = false;
        SetRbKinematicState(false);
        SetColliderState(true);

        MovementCharacters movementCharacter = GetComponent<MovementCharacters>();
        float impusleSide = movementCharacter._movementX;
        Vector3 force = new Vector3(impusleSide*7, 0, movementCharacter.inverse ? impulseBack : -impulseBack) * impulsePower;
        transform.Find("Root").Find("Hips").Find("Spine_01").Find("Spine_02").gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    void SetRbKinematicState(bool state)
    {
        Rigidbody[] rigBodies = GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigBody in rigBodies)
        {
            rigBody.isKinematic = state;
        }
    }

    void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        GetComponent<CharacterController>().enabled = !state;
        GetComponent<BoxCollider>().enabled = !state;
    }

    void SetContrScriptsState(bool state)
    {
        GetComponent<MovementCharacters>().enabled = state;
        GetComponent<Shooting>().enabled = state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MovementBullet>())
        {
            Die();
            Invoke("Respawn", 5f);
        }
    }

    private void Respawn()
    {      
        animator.enabled = true;
        SetRbKinematicState(true);
        SetColliderState(false);

        if (photonView.IsMine)
        {
            SetContrScriptsState(true);
            Vector3 spawnPos;
            Quaternion spawnRot;
            if (PhotonNetwork.IsMasterClient)
            {
                spawnPos = new Vector3(0f, 1f, -7f);
                spawnRot = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                spawnPos = new Vector3(0f, 1f, 7f);
                spawnRot = Quaternion.Euler(0, 180, 0);
            }
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }

    }
}
