$(document).ready(function () {
    var connection = new WebSocketManager.Connection("ws://localhost:5000/chat");
    connection.enableLogging = true;

    connection.connectionMethods.onConnected = () => {

    }

    connection.connectionMethods.onDisconnected = () => {

    }
    
    connection.clientMethods["pingMessage"] = (message, userName) => {
        var messageText = userName + ' ' + connection.connectionId + ' said: ' + message;
        $('#messages').append('<li>' + messageText + '</li>');
        $('#messages').scrollTop($('#messages').prop('scrollHeight'));
    }

    connection.clientMethods["sendMessageTo"] = (to, message, arguments) => {
        var messageText = arguments[2] + ' said: ' + arguments[1]
        $('#messages').append('<li>' + messageText + '</li>');
        $('#messages').scrollTop($('#messages').prop('scrollHeight'));
    }

    connection.start();

    var $messagecontent = $('#message-content');
    $messagecontent.keyup(function (e) {
        if (e.keyCode == 13) {
            var message = $messagecontent.val().trim();
            var userName = $('#user-name').val().trim();
            if (message.length == 0) {
                return false;
            }
            var to = $('#to').val();

            if (to == '') {
                
                connection.invoke("SendMessage", message, userName);
            }
            else {

                connection.invoke("SendMessageTo", to, message, userName);
            }

            $messagecontent.val('');
        }
    });
    $('#messages').scrollTop($('#messages').prop('scrollHeight'));
});
