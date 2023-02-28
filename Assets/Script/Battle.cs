using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Battle : MonoBehaviourPun
{
    public Spinner spinnerScript;

    public GameObject uI_3D_Gameobject;
    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobejct;
    
    private Rigidbody rb;
    
    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;


    public float common_Damage_Coefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;
    private bool isDead = false;
    
    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f; //do more damage than defender ADVANTAGE
    public float getDamaged_Coefficient_Attacker = 1.2f; //gets more damage - DISADVANTAGE
    
    public float doDamage_Coefficient_Defender = 0.75f;// do less damage - DISADVANTAGE
    public float getDamaged_Coefficient_Defender = 0.2f; //get less damage - ADVANTAGE

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        
    }

    private void checkPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
            
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isAttacker = false;
            isDefender = true;

            spinnerScript.spinSpeed = 4400;

            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //comparing the speeds of the spinnerTops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My speed: " + mySpeed + "-----Other player speed: " + otherPlayerSpeed);

            if (mySpeed > otherPlayerSpeed)
            {
                Debug.Log("you DAMAGE the other player");
                float default_Damage_Amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600 * common_Damage_Coefficient;

                if (isAttacker)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Attacker;
                }
                else if (isDefender)
                {
                    default_Damage_Amount *= doDamage_Coefficient_Defender;
                }
                
                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    //Apply Damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_Damage_Amount);
                }
            }
        }
    }

   [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamaged_Coefficient_Attacker;

                if (_damageAmount > 1000)
                {
                    _damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damageAmount *= getDamaged_Coefficient_Defender;
            }
        
            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;

            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
              Debug.Log("current speed is low "+ currentSpinSpeed );
                //die
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("set is dead to TRUE");
        GetComponent<MovementController>().enabled = false;
        Debug.Log("disable MOVEMENT");
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Debug.Log("disable ROTATION/VELOCITY");

        spinnerScript.spinSpeed = 0f;
        
        uI_3D_Gameobject.SetActive(false);

        if (photonView.IsMine)
        {
            //countdown for respawn
            StartCoroutine(ReSpawn());
        }
    }

    IEnumerator ReSpawn()
    {
        
        GameObject canvasGameobject = GameObject.Find("Canvas");
        if (deathPanelUIGameobejct == null)
        {
            deathPanelUIGameobejct = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);

        }
        else
        {
            deathPanelUIGameobejct.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameobejct.transform.Find("RespawnTimeText").GetComponent<Text>();
        Debug.Log("call respawn time");
        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");
            Debug.Log("decrease respawn time");

            GetComponent<MovementController>().enabled = false;
        }
        deathPanelUIGameobejct.SetActive(false);

        GetComponent<MovementController>().enabled = true;
        Debug.Log("enable MOVEMENT AGAIN");
        
        photonView.RPC("ReBorn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero); //reset rotation
        
        uI_3D_Gameobject.SetActive(true);
        
        isDead = false;
        Debug.Log(" REBORN set is dead to FALSE AGAIN");

    }

    // Start is called before the first frame update
    void Start()
    {
        checkPlayerType();
        
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
