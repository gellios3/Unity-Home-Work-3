using UnityEngine;
using UnityEngine.Networking;

namespace Struct
{
    public class ChatMessage : MessageBase
    {
        public int PlayerId;
        public string Msg;
        public Color Color;
        public StatusMsg Status;
    }

    public enum StatusMsg
    {
        Sending,
        Deleting,
        Printing,
        Editing
    }
}