using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SpinningTopManager : MonoBehaviourPunCallbacks
{
    [Header("UI")] 
    public GameObject uI_InformPanelGameobject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGameButtonGameobject;
    
    // Start is called before the first frame update
    void Start()
    {
        uI_InformPanelGameobject.SetActive(true);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        uI_InformText.text = "Searching for available rooms...";
        PhotonNetwork.JoinRandomRoom();
        
        searchForGameButtonGameobject.SetActive(false);
        
    }

    public void OnQuickMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
        
    }
    
    #endregion
    
    #region PHOTON Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        uI_InformText.text = message;
        CreateAndJoinRoom();
        
    }

    public override void OnJoinedRoom()
    {
        //if there are more than 2 players per room remember to change here 
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "joined to " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";
        }
        else
        {
            uI_InformText.text = "joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));
        }
        Debug.Log(" joined to " + PhotonNetwork.CurrentRoom.Name);
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName+ " joined to " + PhotonNetwork.CurrentRoom.Name+ " Player count " + PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text =  newPlayer.NickName+ " joined to " + PhotonNetwork.CurrentRoom.Name+ " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;
        
        //deactivate message panel after 2 seconds
        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    
    }

    #endregion

    #region PRIVATE Methods

    void CreateAndJoinRoom()
    {
        //setting a random name for the room
        string randomRoomName = "Room" + Random.Range(0, 10000);

        //setting maximum players per room
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
            
        //creating room
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
         yield return new WaitForSeconds(_seconds);
         _gameObject.SetActive(false);
    }
    
    #endregion
}
