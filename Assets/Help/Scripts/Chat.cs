using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

namespace Help.Scripts
{
	public class ClientMessages
	{
		public short chatMsg = 1000;
	}

	public struct ChatStruct
	{
		public string msg;
		public Color color;
	}


	public class ChatMessage : MessageBase
	{
		public short playerId; 
		public string msg;
		public Color color;
	}

	public class MySyncList : SyncListStruct<ChatStruct>
	{
	
	}

	/// <summary>
	///  if ((base.get_syncVarDirtyBits() & 1u) != 0u)
	/// </summary>
	/// if ((num & 1) != 0)
	public class Chat : NetworkBehaviour
	{
		const short chatMsg = 1000;
		NetworkClient _client;

		[SerializeField]
		private Text chatline;

		[SerializeField] private MySyncList chatLog = new MySyncList();
	
		[SerializeField]
		private InputField input;


		public override void OnStartClient()
		{
			chatLog.Callback = OnChatUpdated;
		}

		public void Start()
		{
			_client = NetworkManager.singleton.client;
			NetworkServer.RegisterHandler(chatMsg, OnServerPostChatMessage);    
			input.onEndEdit.AddListener(delegate { PostChatMessage(input.text); });        
		}

		[ClientCallback]
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				PostChatMessage(input.text);
			}
		}
	
		[Client]
		public void PostChatMessage(string message)
		{
			if (message.Length == 0) return;
			var msg = new ChatMessage();
			msg.color = Color.blue;
			msg.msg = message;

			input.text = "";
			input.ActivateInputField();
			input.Select();
		}

		[Server]
		void OnServerPostChatMessage(NetworkMessage netMsg) 
		{
			ChatMessage message = netMsg.ReadMessage<ChatMessage>();
		
			var chatStuct = new ChatStruct();
			chatStuct.color = message.color;
			chatStuct.msg = message.msg;
		
			//NetworkServer.SendToClient()
		
			chatLog.Add(chatStuct);

		}

		private void OnChatUpdated(SyncListStruct<ChatStruct>.Operation op, int index)
		{
			chatline.text += chatLog[chatLog.Count-1].msg + "\n";
			chatline.color = chatLog[chatLog.Count - 1].color;
		}


	}
}