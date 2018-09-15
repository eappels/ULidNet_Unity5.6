using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{

    private void OnEnable()
    {
        UIManager.instance.btn_Server.GetComponent<Button>().onClick.AddListener(btn_Server_Click);
        UIManager.instance.btn_Client.GetComponent<Button>().onClick.AddListener(btn_Client_Click);
    }

    private void OnDisable()
    {
        UIManager.instance.btn_Server.GetComponent<Button>().onClick.RemoveListener(btn_Server_Click);
        UIManager.instance.btn_Client.GetComponent<Button>().onClick.RemoveListener(btn_Client_Click);
    }

    private void btn_Server_Click()
    {
        UIManager.instance.pnl_Menu.SetActive(false);
        UIManager.instance.pnl_Server.SetActive(true);
    }

    private void btn_Client_Click()
    {
        UIManager.instance.pnl_Menu.SetActive(false);
        UIManager.instance.pnl_Client.SetActive(true);
    }
}