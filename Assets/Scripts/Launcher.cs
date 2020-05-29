using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] [Tooltip("Активируйте чтобы показывать логи PUN")]
    bool isEnabledLog = true;

    [SerializeField] byte playersPerRoom = 2;

    [SerializeField] GameObject controlPanel;
    [SerializeField] GameObject progressPanel;

    string gameVersion = "1";
    static bool isConnecting = true;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        controlPanel.SetActive(true);
        progressPanel.SetActive(false);
    }

    public void Connect()
    {
        controlPanel.SetActive(false);
        progressPanel.SetActive(true);

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isEnabledLog) Debug.Log("Connected to master");
        if (isConnecting)
        {
            Debug.Log("isConnecting will be false"); 
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        controlPanel.SetActive(true);
        progressPanel.SetActive(false);
        if (isEnabledLog) Debug.Log("Disconected");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (isEnabledLog) Debug.Log("нету доступных комнат, создаем новую");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = playersPerRoom });
    }

    public override void OnJoinedRoom()
    {     
        if (isEnabledLog) Debug.Log("подключение к комнате");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (isEnabledLog) Debug.Log("мы подключились к созданой комнате");
            

        } else if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if (isEnabledLog) Debug.Log("мы создали комнату, ждем второго игрока");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (isEnabledLog) Debug.Log("новый игрок подключился к комнате");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (isEnabledLog) Debug.Log("в комнате 2 игрока");
            if (PhotonNetwork.IsMasterClient)
            {
                if (isEnabledLog) Debug.Log("мы мастер клиент, запускаем сцену");
                PhotonNetwork.LoadLevel("DemoBattlefield");
            }
        }
    }
}
