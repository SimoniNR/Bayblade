   using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
   
   

   
   public class LobbyManager : MonoBehaviourPunCallbacks
   {
       [Header("Login UI")] 
       public InputField PlayerNameInputField;
       public GameObject uI_LoginGameobject;
       
       [Header("Lobby UI")] 
       public GameObject uI_LoobyGameobject;
       public GameObject uI_3DGameobject;
       
       [Header("Connection Status UI")] 
       public GameObject uI_ConnectionStatusGameobject;
       public Text connectionStatusText;
       public bool showConnectionStatus = false;
       
    #region UNITY Methods
    // Start is called before the first frame update
    void Start()
    {
        uI_LoobyGameobject.SetActive(false);
        uI_3DGameobject.SetActive(false);
        uI_ConnectionStatusGameobject.SetActive(false);
        
        uI_LoginGameobject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    
    #endregion
    
    #region UI CallBack Methods
    public void OnEnterGameButtonClicked()
    {
        
        string PlayerName = PlayerNameInputField.text;

        if (!string.IsNullOrEmpty(PlayerName))
        {
            uI_LoobyGameobject.SetActive(false);
            uI_3DGameobject.SetActive(false);
            uI_LoginGameobject.SetActive(false);

            showConnectionStatus = true;
            uI_ConnectionStatusGameobject.SetActive(true);
            
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

    public void onQuickMathcButtonClicked()
    {
        SceneManager.LoadScene("Scene_Loading");
    }
    
    #endregion
    
    #region PHOTON Callback Methods

    public override void OnConnected()
    {
        Debug.Log("We connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");
        
        uI_LoobyGameobject.SetActive(true);
        uI_3DGameobject.SetActive(true);
        
        uI_LoginGameobject.SetActive(false);
        uI_ConnectionStatusGameobject.SetActive(false);
        
        
    }
    
    #endregion
}
