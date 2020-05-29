using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class ManagerGame : MonoBehaviourPunCallbacks
{
    GameObject _camera;
    GameObject player;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] float cameraMoveBound = 2;

    float playerMoveBound;
    

    private void Start()
    {
        _camera = GameObject.Find("Main camera");

        Vector3 spawnPos;
        Quaternion spawnRot;
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPos = new Vector3(0f, 1f, -7f);
            spawnRot = Quaternion.Euler(0, 0, 0);
            //Camera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            spawnPos = new Vector3(0f, 1f, 7f);
            spawnRot = Quaternion.Euler(0, 180, 0);
            _camera.transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot, 0);

        playerMoveBound = player.GetComponent<MovementCharacters>()._moveBound;
    }

    public void Update()
    {
        Vector3 oldPosCam = _camera.transform.position;
        float newX = player.transform.position.x / playerMoveBound * cameraMoveBound;
        _camera.transform.position = new Vector3( newX, oldPosCam.y, oldPosCam.z );
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


}
