using UnityEngine;

public class UIManager : MonoBehaviour
{

    #region instance
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
        Application.runInBackground = true;
    }
    #endregion

    public GameObject pnl_Menu, pnl_Server, pnl_Client, btn_Server, btn_Client, btn_StartStopServer, btn_Connect, btn_Disconnect, btn_Back, txt_Ipaddress;

    private void Start()
    {
        pnl_Menu.SetActive(true);
    }
}