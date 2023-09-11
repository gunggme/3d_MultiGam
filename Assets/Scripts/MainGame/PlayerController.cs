using Fusion;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private TextMeshProUGUI playerNameText;  
    [SerializeField] private GameObject cam;
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float jumpForce = 1000;

    [Networked(OnChanged = nameof(OnNicknameChanged))] private NetworkString<_8> playerName { get; set; }
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    private float horizontal;
    private Rigidbody2D rigid;
    
    private enum PlayerInputButtons
    {
        None,
        Jump
    }

    public override void Spawned()
    {
        rigid = GetComponent<Rigidbody2D>();
        SetLocalObjects();
    }
    
    private void SetLocalObjects()
    {
        if (Runner.LocalPlayer == Object.HasInputAuthority)
        {
            cam.SetActive(true);
            var nickName = GlobalManagers.Instance.NetworkRunnerController.LocalPlayerNickname;
            RpcSetNickName(nickName);
        }
    }

    // Sends RPC to HOST(from a client)
    // "sources" define which PEER can send the rpc
    // The RpcTargets defines on which it is executed!
    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickName(NetworkString<_8> nickName)
    {
        playerName = nickName;
    }

    //For example -
    // if i set on spawned method a name called "banana"
    // and then on fun i change another name which is again "banana"
    private static void OnNicknameChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.SetPlayerNickname(changed.Behaviour.playerName);
    }

    private void SetPlayerNickname(NetworkString<_8> nickname)
    {
        playerNameText.text = nickname + " " + Object.InputAuthority.PlayerId;
    }

    //Happens before anything else Fusion does, network application, reconlation etc
    //Called at the start of the Fusion Update loop, before the Fusion simuation loop.
    //It fires before Fusion does ANY work, every screen refresh
    public void BeforeUpdate()
    {
        if (Runner.LocalPlayer == Object.HasInputAuthority)
        {
            const string HORIZONTAL = "Horizontal";
            horizontal = Input.GetAxisRaw(HORIZONTAL);
        }
    }

    //FUN
    public override void FixedUpdateNetwork()
    {
        // will return false if:
        // the client does not have State Authority or Input Authority
        // the requested type of input does not exist in the simuation  
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            rigid.velocity = new Vector2(input.HorizontalInput * moveSpeed, rigid.velocity.y);
            CheckJumpInput(input);
        }    
    }

    private void CheckJumpInput(PlayerData input)
    {
        var pressed = input.NetworkButtons.GetPressed(buttonsPrev);
        if (pressed.WasPressed(buttonsPrev, PlayerInputButtons.Jump))
        {
            rigid.AddForce(Vector2.up * jumpForce);
        }

        buttonsPrev = input.NetworkButtons;
    }

    public PlayerData GetPlayerNetworkInput()
    {
        PlayerData data = new PlayerData();
        data.HorizontalInput = horizontal;
        data.NetworkButtons.Set(PlayerInputButtons.Jump, Input.GetKey(KeyCode.Space));
        return data;
    }

}
