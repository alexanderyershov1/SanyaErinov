using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketManager;

namespace WebApplication2
{
    public class ChatHandler : WebSocketHandler
    {
        private readonly ChatManager _chatManager;
        public ChatHandler(WebSocketConnectionManager webSocketConnectionManager, ChatManager chatManager) : base(webSocketConnectionManager)
        {
            _chatManager = chatManager;
        }

        public async Task SendMessage(string message, string userName)
        {
            dynamic dynamicMessage = new ExpandoObject();
            dynamicMessage.UserName = userName;
            dynamicMessage.Message = message;
            _chatManager.Messages.Add(dynamicMessage);
            await InvokeClientMethodToAllAsync("pingMessage", message, userName);
        }

        public async Task SendMessageTo(string socketId, string message, object[] arguments)
        {
            await InvokeClientMethodAsync(socketId, "sendMessageTo", arguments);
        }
    }
}
