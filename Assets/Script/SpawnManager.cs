using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPositions;
    
    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Photon Callback Methods

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            /*object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER,out playerSelectionNumber))
            {
                Debug.Log("Player Selection number is " + (int)playerSelectionNumber);

                int randomSpawnPoint = Random.Range(0,spawnPositions.Length-1);
                Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;
                    
                PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name,instantiatePosition, Quaternion.identity);
            }*/
            SpawnPlayer();
        }
        
    }

    #endregion

    #region Private Methods

    private void SpawnPlayer()
    {
        //get player selected data from player properties
        object playerSelectionNumber;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER,out playerSelectionNumber))
        {
            Debug.Log("Player Selection number is " + (int)playerSelectionNumber);

           //pick a random spawn point
            int randomSpawnPoint = Random.Range(0,spawnPositions.Length-1);
            Vector3 instantiatePosition = spawnPositions[randomSpawnPoint].position;

            //instatiate the player locally
            GameObject playerGameobject = Instantiate(playerPrefabs[(int)playerSelectionNumber], instantiatePosition, Quaternion.identity);

            PhotonView _photonView = playerGameobject.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(_photonView))
            {
                object[] data = new object[]
                {
                    playerGameobject.transform.position, playerGameobject.transform.rotation, _photonView.ViewID, playerSelectionNumber
                };
                
                // Raise Events!
                PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.PlayerSpawnEventCode);
            }
            else
            {
                Debug.Log("Failed to allocate a viewID");
                Destroy(playerGameobject);
            }
        }
    }
    

    #endregion
}
