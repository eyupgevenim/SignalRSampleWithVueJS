<template>
    <b-container class="user-card">
        <b-row class="text-center">
            <b-col>
                <h4>Chat page</h4>
            </b-col>
        </b-row>
        <b-row class="text-center">
            <b-col>
                <hr />
            </b-col>
        </b-row>
        <b-row>
            <b-col cols="4" md="4" sm="5" class="text-center">
                <div class="avatar-list-group scrollbar">
                    <b-list-group style="max-width: 250px;">
                        <user-avatar :key="avatar.id" 
                                     :avatar="{...avatar, index}" 
                                     v-for="(avatar, index) in users"
                                     @onUserAvatarClickCallback="onChengedFocusAvatar">
                        </user-avatar>
                    </b-list-group>
                </div>
            </b-col>
            <b-col cols="8" md="8" sm="7">
                <b-row>
                    <b-col cols="12">
                        <div style="min-height:300px">

                            <user-message-dialog :key="data.id" 
                                                 :data="data" 
                                                 v-for="data in focusedUserMessages">
                            </user-message-dialog>

                            <b-row>
                                <b-col cols="12">
                                    <b-card>
                                        <b-media>
                                            <b-media-body>
                                                <div class="media align-items-center">
                                                    <b-avatar variant="primary" 
                                                              :text="this.currentUser.text" 
                                                              class="avatar avatar-lg mr-4 rounded-circle">
                                                    </b-avatar>
                                                    <div class="media-body">
                                                        <b-row>
                                                            <b-col cols="10" md="10" sm="9">
                                                                <input id="message"
                                                                       name="message"
                                                                       type="text"
                                                                       placeholder="Write your message"
                                                                       class="form-control"
                                                                       v-model="message">
                                                            </b-col>
                                                            <b-col cols="2" md="2" sm="3">
                                                                <b-button variant="primary" @click="sendMessage()">Send</b-button>
                                                            </b-col>
                                                        </b-row>
                                                    </div>
                                                </div>
                                            </b-media-body>
                                        </b-media>
                                    </b-card>

                                </b-col>
                            </b-row>

                        </div>
                    </b-col>
                </b-row>
            </b-col>
        </b-row>
        <b-row style="height:100px;">
            <b-col>
                
            </b-col>
        </b-row>
    </b-container>
</template>
<script>
    import UserAvatar from '../components/user-avatar.vue';
    import UserMessageDialog from '../components/user-message-dialog.vue';
    import UserService from '../services/user.service';

    const getInitials = function (fullName) {
        const allNames = fullName.trim().split(' ');
        const initial = (acc, curr, index) => (index === 0 || index === allNames.length - 1) ? `${acc}${curr.charAt(0).toUpperCase()}` : acc;
        return allNames.reduce(initial, '');
    };

    export default {
        name: "Chat",
        components: {
            'user-avatar': UserAvatar,
            'user-message-dialog': UserMessageDialog
        },
        data() {
            return {
                currentUser: {},
                message: '',
                messages: [],
                connection: null,
                users: [],
                focusedUserId: "",
                focusedUserMessages: [],
                onlineUsers: [],
            };
        },
        methods: {
            loadCurrentUser() {
                let user = this.$store.getters["auth/user"];
                this.currentUser = {
                    ...user,
                    text: getInitials(user.name),
                    image: null
                };

                this.startSignalR(user.accessToken);

                console.log("this.currentUser:", this.currentUser);
            },
            loadUsers() {
                return UserService.getUsers().then(response => {
                    console.log("UserService.getUsers() - response:", response);
                    if (response.status == 200 && response.data) {

                        this.users = response.data.filter(u => u.id != this.currentUser.userId).map(function (item) {
                            let text = getInitials(item.name);
                            return {
                                ...item,
                                text,
                                badgeVariant: "warning",
                                href: "#" + item.id,
                                variant: "primary",
                                active: '',
                                image: null
                            };
                        });

                        console.log("this.users:", this.users);
                    }

                    return Promise.resolve(response);
                }, error => {

                    console.log("UserService.getUsers() - error:", error);
                    return Promise.reject(error);
                });
            },
            loadActiveUsers() {
                return UserService.getActiveUsers().then(response => {
                    console.log("UserService.getActiveUsers() - response:", response);
                    if (response.status == 200 && response.data) {

                        let data = response.data.data;
                        console.log("UserService.getActiveUsers() - response.data.data:", data);

                        this.users = this.users.map(function (item) {
                            if (data.filter(a => a.userId == item.id)[0]) {
                                item.badgeVariant = 'success';
                            } else {
                                item.badgeVariant = 'warning';
                            }
                            return item;
                        });

                        this.onlineUsers = data.map(function (item) { return item.userId; });
                    }
                    return Promise.resolve(response);
                }, error => {
                    console.log("UserService.getActiveUsers() - error:", error);
                    return Promise.reject(error);
                });
            },
            onChengedFocusAvatar(user) {

                this.users = this.users.map(function (item) {
                    item.active = (item.id == user.id ? 'active' : '');
                    return item;
                });

                this.focusedUserId = user.id;

                UserService.getMessages({ userId: user.id }).then(response => {
                    console.log("UserService.getMessages - response:", response);
                    if (response.status == 200 && response.data) {

                        const cUser = this.currentUser;
                        const fUser = this.users.filter(x => x.id == user.id)[0];

                        this.focusedUserMessages = response.data.data.map(function (item) {

                            const message = {
                                id: item.messageId,
                                message: item.content,
                                date: item.date,
                                formatedDate: item.formatedDate
                            };

                            if (item.fromUserId == cUser.userId) {
                                message.name = cUser.name;
                                message.image = cUser.image;
                                message.initials = cUser.text;
                                message.align = 'right';
                            } else {
                                message.name = fUser.name;
                                message.image = fUser.image;
                                message.initials = fUser.text;
                                message.align = 'left';
                            }

                            return message;
                        });
                    }

                    return Promise.resolve(response);
                }, error => {

                    console.log("UserService.getMessages - error:", error);
                    return Promise.reject(error);
                });


            },
            sendMessage() {
                if (this.message.length > 0) {

                    let toUserId = this.focusedUserId;
                    //this.$chatHub.sendUserMessage(toUserId, this.message)

                    UserService.sendMessage({
                        toUserId: toUserId,
                        content: this.message
                    }).then(response => {
                        console.log("UserService.sendMessage - response:", response);
                        if (response.status == 200 && response.data.success) {
                            const data = response.data.data;
                            this.onUserMessageReceived({
                                senderUserId: this.currentUser.userId,
                                messageModel: data
                            });
                        }
                        return Promise.resolve(response);
                    }, error => {
                        console.log("UserService.sendMessage - error:", error);
                        return Promise.reject(error);
                    });

                    this.message = '';
                }
            },
            onUserMessageReceived({ senderUserId, messageModel }) {

                console.log("onUserMessageReceived - ", " senderUserId: ", senderUserId, " - messageModel: ", messageModel);

                const cUser = this.currentUser;
                const fUser = this.users.filter(x => x.id == this.focusedUserId)[0];
                if (fUser) {

                    const message = {
                        id: messageModel.messageId,
                        message: messageModel.content,
                        date: messageModel.date,
                        formatedDate: messageModel.formatedDate
                    };

                    if (senderUserId == this.currentUser.userId) {
                        message.name = cUser.name;
                        message.image = cUser.image;
                        message.initials = cUser.text;
                        message.align = 'right';
                    } else {
                        message.name = fUser.name;
                        message.image = fUser.image;
                        message.initials = fUser.text;
                        message.align = 'left';
                    }

                    this.focusedUserMessages.push(message);
                }
            },
            focusDefaultUser() {
                if (this.users) {
                    this.onChengedFocusAvatar(this.users[0]);
                }
            }
        },
        created: function () {

            Promise
                .resolve(this.loadCurrentUser())
                .then(_ => Promise.resolve(this.loadUsers()), error => Promise.reject(error))
                .then(_ => Promise.resolve(this.loadActiveUsers()), error => Promise.reject(error))
                .then(_ => Promise.resolve(this.focusDefaultUser()), error => Promise.reject(error));

            // Listen to answer changes from SignalR event
            this.$chatHub.$on('user-message-received', this.onUserMessageReceived);

            this.$chatHub.$on('active-user', (userId) => {

                console.log("this.$chatHub.$on('active-user'->userId:", userId);
                this.users = this.users.map(function (item) {
                    if (item.id == userId) {
                        item.badgeVariant = 'success';
                    }
                    return item;
                });

                this.onlineUsers.push(userId);
            });
            this.$chatHub.$on('passive-user', (userId) => {
                console.log("this.$chatHub.$on('passive-user'->userId:", userId);
                this.users = this.users.map(function (item) {
                    if (item.id == userId) {
                        item.badgeVariant = 'warning';
                    }
                    return item;
                });

                this.onlineUsers = this.onlineUsers.filter(u => u != userId);

            });
        },
        async mounted() {
            this.loadActiveUsers();
        },
        beforeDestroy() {
            // Make sure to cleanup SignalR event handlers when removing the component
            this.$chatHub.$off('user-message-received', this.onUserMessageReceived);
        },
        computed: {},
    };
</script>
<style scoped>
    .list-group-item {
        border-right: 0 !important;
        border-left: 0 !important;
        
    }

    .scrollbar {
        padding: 10px;
        float: left;
        /*height: 350px;
        width: 335px;
        overflow-y: scroll;*/
    }

    .avatar-list-group::-webkit-scrollbar-track {
        border-radius: 10px;
    }

    .avatar-list-group::-webkit-scrollbar {
        width: 10px;
    }

    .avatar-list-group::-webkit-scrollbar-thumb {
        border-radius: 10px;
        background-image: -webkit-gradient(linear, 40% 0%, 75% 84%, from(#4D9C41), to(#19911D), color-stop(.6,#54DE5D))
    }

    .user-card {
        background-color: #F5F5F5;
    }

    .avatar-list-group {
        background-color: white;
    }

</style>