   using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;  
   public class LobbyManager : MonoBehaviourPunCallbacks
   {
       [Header("Login UI")] 
       InputField PlayerNameInputField;
       
    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    #endregion
    
    #region UI CallBack Methods
    public void OnEnterGameButtonClicked()
    {
        string PlayerName = PlayerNameInputField.text;

        if (!string.IsNullOrEmpty(PlayerName))
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = PlayerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player name is invalid or empty");
        }
    }
    #endregion
    
    #region PHOTON Callback Methods
    
    
    #endregion
}
