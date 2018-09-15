using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LidClientBehaviour : MonoBehaviour
{

    public LidClient lidclient;

    private void OnEnable()
    {
        lidclient = new LidClient();
        lidclient.OnNetworkDebugMessage += OnNetworkDebug;
        lidclient.Connected += OnConnected;
        lidclient.Disconnected += OnDisconnected;
        UIManager.instance.btn_Connect.GetComponent<Button>().onClick.AddListener(btn_Connect_Click);
        UIManager.instance.btn_Disconnect.GetComponent<Button>().onClick.AddListener(btn_Disconnect_Click);
        UIManager.instance.btn_Back.GetComponent<Button>().onClick.AddListener(btn_Back_Click);

        UIManager.instance.btn_Disconnect.SetActive(false);
        UIManager.instance.btn_Back.SetActive(true);
    }

    private void OnDisable()
    {
        UIManager.instance.btn_Back.SetActive(false);

        UIManager.instance.btn_Disconnect.GetComponent<Button>().onClick.RemoveListener(btn_Disconnect_Click);
        UIManager.instance.btn_Connect.GetComponent<Button>().onClick.RemoveListener(btn_Connect_Click);
        UIManager.instance.btn_Back.GetComponent<Button>().onClick.RemoveListener(btn_Back_Click);
        lidclient.Connected -= OnConnected;
        lidclient.Disconnected -= OnDisconnected;
        lidclient.OnNetworkDebugMessage -= OnNetworkDebug;
        lidclient = null;
    }

    private void FixedUpdate()
    {
        if (lidclient != null) lidclient.MessagePump();
    }

    private void btn_Connect_Click()
    {
        lidclient.Connect(UIManager.instance.txt_Ipaddress.GetComponent<InputField>().text);
    }

    private void btn_Disconnect_Click()
    {
        lidclient.Disconnect();
    }

    private void btn_Back_Click()
    {
        lidclient.StopClient();
        EnableOrDisableButtonInteractionOClientnUI(false, UIManager.instance.pnl_Client);
        StartCoroutine("Delayed_Back_Click");
    }

    private IEnumerator Delayed_Back_Click()
    {
        yield return new WaitForSeconds(1);
        UIManager.instance.pnl_Client.SetActive(false);
        UIManager.instance.pnl_Menu.SetActive(true);
        EnableOrDisableButtonInteractionOClientnUI(true, UIManager.instance.pnl_Client);
    }

    private void OnNetworkDebug(string debugstring)
    {
        Debug.Log(debugstring);
    }

    private void EnableOrDisableButtonInteractionOClientnUI(bool enableOrDisable, GameObject pnl)
    {
        foreach (Transform child in pnl.transform)
        {
            if (child.gameObject.GetComponent<InputField>() != null)
            {
                child.gameObject.GetComponent<InputField>().interactable = enableOrDisable;
            }
            else
            {
                child.gameObject.GetComponent<Button>().interactable = enableOrDisable;
            }
        }
        UIManager.instance.btn_Back.GetComponent<Button>().interactable = enableOrDisable;
    }

    private void OnConnected()
    {
        UIManager.instance.btn_Back.SetActive(false);
        UIManager.instance.txt_Ipaddress.SetActive(false);
        UIManager.instance.btn_Connect.SetActive(false);
        UIManager.instance.btn_Disconnect.SetActive(true);
    }

    private void OnDisconnected()
    {
        UIManager.instance.btn_Disconnect.SetActive(false);
        UIManager.instance.txt_Ipaddress.SetActive(true);
        UIManager.instance.btn_Connect.SetActive(true);
        UIManager.instance.btn_Back.SetActive(true);
    }
}