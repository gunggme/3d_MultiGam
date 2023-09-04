using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MiddleSectionPanel : LobbyPanelBase
{
    [Header("MiddleSectionPanel Vars")] 
    [SerializeField] private Button joinRandomRoomBtn;
    [SerializeField] private Button joinRoomByArgBtn;
    [SerializeField] private Button createRoomBtn;

    [SerializeField] private TMP_InputField joinRoomByArgInputField;
    [SerializeField] private TMP_InputField createRoomInputField;

    private NetworkRunnerController networkRunnerController;
    
    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);

        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;
        
        joinRandomRoomBtn.onClick.AddListener(joinRandomRoom);
        joinRoomByArgBtn.onClick.AddListener(() =>  CreateRoom(GameMode.Host, joinRoomByArgInputField.text));
        createRoomBtn.onClick.AddListener(() =>  CreateRoom(GameMode.Host, createRoomInputField.text));
    }

    private void CreateRoom(GameMode mode, string field)
    {
        if (createRoomInputField.text.Length >= 2)
        {
            Debug.Log($"----------------------{mode}-----------------------");
            GlobalManagers.Instance.NetworkRunnerController.StartGame(mode, field);
        }
    }

    private void joinRandomRoom()
    {
        Debug.Log("----------------{JoinRandomRoom!}-------------");
        GlobalManagers.Instance.NetworkRunnerController.StartGame(GameMode.AutoHostOrClient, string.Empty);
    }
}
