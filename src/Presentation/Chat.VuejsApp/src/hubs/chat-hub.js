import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'

export default {
    install(Vue) {
        // use a new Vue instance as the interface for Vue components to receive/send SignalR events
        // this way every component can listen to events or send new events using this.$chatHub
        const chatHub = new Vue();
        Vue.prototype.$chatHub = chatHub;

        // Provide methods to connect/disconnect from the SignalR hub
        let connection = null;
        let startedPromise = null;
        let manuallyClosed = false;

        Vue.prototype.startSignalR = (jwtToken) => {
            connection = new HubConnectionBuilder()
                .withUrl(
                    `${Vue.prototype.$http.defaults.baseURL}/chat-hub`,
                    jwtToken ? { accessTokenFactory: () => jwtToken } : null
                )
                .configureLogging(LogLevel.Information)
                .build();

            connection.on('ActiveUser', (userId) => {
                chatHub.$emit('active-user', userId);
            });
            connection.on('ActiveUsers', (activeUserIds) => {
                chatHub.$emit('active-users', activeUserIds)
            });
            connection.on('PassiveUser', userId => {
                chatHub.$emit('passive-user', userId);
            });
            connection.on('UserMessageReceived', (senderUserId, messageModel) => {
                chatHub.$emit('user-message-received', { senderUserId, messageModel});
            });
            

            // You need to call connection.start() to establish the connection but the client wont handle reconnecting for you!
            // Docs recommend listening onclose and handling it there.
            // This is the simplest of the strategies
            function start() {
                startedPromise = connection.start()
                    .catch(err => {
                        console.error('Failed to connect with hub', err);
                        return new Promise((resolve, reject) => setTimeout(() => start().then(resolve).catch(reject), 5000));
                    })
                return startedPromise;
            }

            connection.onclose(() => {
                if (!manuallyClosed) start();
            });

            // Start everything
            manuallyClosed = false;
            start();
        };

        Vue.prototype.stopSignalR = () => {
            if (!startedPromise) return;

            manuallyClosed = true;
            return startedPromise
                .then(() => connection.stop())
                .then(() => { startedPromise = null });
        };

        // Provide methods for components to send messages back to server
        // Make sure no invocation happens until the connection is established
        chatHub.joinChatGroup = (groupId) => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke('JoinChatGroup', groupId))
                .catch(console.error);
        };

        chatHub.leaveChatGroup = (groupId) => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke('LeaveChatGroup', groupId))
                .catch(console.error);
        };

        chatHub.sendActiveUser = (userId) => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke('SendActiveUser', userId))
                .catch(console.error);
        };

        chatHub.sendPassiveUser = (userId) => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke('SendPassiveUser', userId))
                .catch(console.error);
        };

        chatHub.sendActiveUsers = () => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke('SendActiveUsers'))
                .catch(console.error);
        };

        chatHub.sendUserMessage = (userId, messageModel) => {
            if (!startedPromise) return;

            return startedPromise
                .then(() => connection.invoke('SendUserMessage', userId, messageModel))
                .catch(console.error);
        };
    }
}
