using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using  Photon.Pun;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;
    
    public GameObject[] spinnerTopModels;
    

    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;
    public Button next_button;
    public Button previous_button;
    
    public GameObject uI_Selection;
    public GameObject uI_AfterSelection;
    
    #region UNITY Methods

    // Start is called before the first frame update
    void Start()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
        
        playerSelectionNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion



    #region UI Callback Methods

    public void NextPlayer()
    {
        playerSelectionNumber += 1;
        if (playerSelectionNumber >= spinnerTopModels.Length)
        {
            playerSelectionNumber = 0;
        }
        Debug.Log(playerSelectionNumber);
        
        next_button.enabled = false;
        previous_button.enabled = false;
        
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1.0f));
        
        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            //this means the player model type is ATTACK
            playerModelType_Text.text = "Attack";
        }
        else
        {
            //this means the player model type is DEFFEND
            playerModelType_Text.text = "Defend";
        }
    }

    public void PreviousPlayer()
    {
        playerSelectionNumber -= 1;
        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = spinnerTopModels.Length - 1;
        }
        Debug.Log(playerSelectionNumber);
        
        next_button.enabled = false;
        previous_button.enabled = false;
        
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1.0f));
        
        if (playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            //this means the player model type is ATTACK
            playerModelType_Text.text = "Attack";
        }
        else
        {
            //this means the player model type is DEFFEND
            playerModelType_Text.text = "Defend";
        }
    }

    public void OnSelectionButtonClicked()
    {
        uI_Selection.SetActive(false);
        uI_AfterSelection.SetActive(true);
        
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
        
    }

    public void OnReSelectButtonClicked()
    {
        uI_Selection.SetActive(true);
        uI_AfterSelection.SetActive(false);
    }

    public void OnBattleButtonClicked()
    {
        //load the next scene
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void OnBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("scene_Lobby");
    }
    
    #endregion

    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f)
    {
        Quaternion originalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle);

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transformToRotate.rotation = finalRotation;
        next_button.enabled = true;
        previous_button.enabled = true;
    }

    #endregion
}


