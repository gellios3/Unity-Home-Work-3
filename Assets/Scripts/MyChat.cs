using System.Collections;
using Struct;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MyChat : NetworkBehaviour
{
    /// <summary>
    /// Chat message connection id
    /// </summary>
    private const short ChatMsgId = 1000;

    /// <summary>
    /// Chat message box
    /// </summary>
    [SerializeField] private Text _chatMsgBox;

    /// <summary>
    /// Chat message box
    /// </summary>
    [SerializeField] private Text _chatMsgStatus;

    /// <summary>
    /// User Input field
    /// </summary>
    [SerializeField] private InputField _inputField;

    /// <summary>
    /// Chat network log
    /// </summary>
    [SerializeField] private MySyncList _chatLog = new MySyncList();

    /// <summary>
    /// Network client
    /// </summary>
    private NetworkClient _client;

    /// <inheritdoc />
    /// <summary>
    /// On start network client
    /// </summary>
    public override void OnStartClient()
    {
        // call on chat box updatyed 
        _chatLog.Callback = OnChatMsgBoxUpdated;
    }

    /// <summary>
    /// On start script
    /// </summary>
    public void Start()
    {
        // init client network
        _client = NetworkManager.singleton.client;
        // register server on send message from client  
        NetworkServer.RegisterHandler(ChatMsgId, OnServerPostChatMessage);
        // add lister on end edit user input. Then send message.
        _inputField.onEndEdit.AddListener(delegate { PostChatMessage(_inputField.text); });
        // add lister on printig message to user input
        _inputField.onValueChanged.AddListener(delegate { OnPrintingMsg(); });
    }

    /// <summary>
    /// On client update
    /// </summary>
    [ClientCallback]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            PostChatMessage(_inputField.text);
        }
    }

    /// <summary>
    /// On pintig chat message
    /// </summary>
    [Client]
    public void OnPrintingMsg()
    {
        var msg = new ChatMessage
        {
            Status = StatusMsg.Printing,
            PlayerId = _client.connection.connectionId
        };
        _client.Send(ChatMsgId, msg);
    }

    /// <summary>
    /// POst chat message
    /// </summary>
    /// <param name="message"></param>
    [Client]
    public void PostChatMessage(string message)
    {
        if (message.Length == 0) return;

        var msg = new ChatMessage
        {
            Color = Color.blue,
            Msg = message,
            Status = StatusMsg.Sending,
            PlayerId = _client.connection.connectionId
        };

        _client.Send(ChatMsgId, msg);

        _inputField.text = "";
        _inputField.ActivateInputField();
        _inputField.Select();
    }

    /// <summary>
    /// On server post chat message
    /// </summary>
    /// <param name="netMsg"></param>
    [Server]
    private void OnServerPostChatMessage(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<ChatMessage>();
        if (message.Status == StatusMsg.Printing)
        {
            RpcOnShowStatusText(message);
        }
        else
        {
            var chatStuct = new ChatStruct
            {
                Color = message.Color,
                Msg = message.Msg,
                PlayerId = message.PlayerId
            };

            _chatLog.Add(chatStuct);
        }
    }

    /// <summary>
    /// On show status message
    /// </summary>
    /// <param name="message"></param>
    [ClientRpc]
    private void RpcOnShowStatusText(ChatMessage message)
    {
        if (message.PlayerId == _client.connection.connectionId) return;
        _chatMsgStatus.text = "Printing User_" + message.PlayerId;
        StartCoroutine(HideStatus());
    }

    /// <summary>
    /// Hide tatus message
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideStatus()
    {
        yield return new WaitForSeconds(1);
        _chatMsgStatus.text = "";
    }

    /// <summary>
    /// Callback on chat log updated
    /// </summary>
    /// <param name="op"></param>
    /// <param name="index"></param>
    private void OnChatMsgBoxUpdated(SyncListStruct<ChatStruct>.Operation op, int index)
    {
        _chatMsgBox.text += "User_" + _chatLog[_chatLog.Count - 1].PlayerId + ": " + _chatLog[_chatLog.Count - 1].Msg +
                            "\n";
        _chatMsgBox.color = _chatLog[_chatLog.Count - 1].Color;
    }
}