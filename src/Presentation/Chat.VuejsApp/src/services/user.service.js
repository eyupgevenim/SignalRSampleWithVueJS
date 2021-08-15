import axios from 'axios';

const API_URL = 'http://localhost:5100';

const UserService = {
    getUsers() {
        return axios.get(`${API_URL}/api/Users`);
    },
    register(user) {
        return axios.post(`${API_URL}/api/Users`, {
            username: user.username,
            email: user.email,
            password: user.password
        });
    },
    sendMessage(message) {
        return axios.post(`${API_URL}/api/Users/SendMessage`, {
            FromUserId: "", //currentUserId : server site
            ToUserId: message.toUserId,
            Content: message.content
        });
    },
    getMessages({ userId }) {
        return axios.get(`${API_URL}/api/Users/${userId}/Messages`);
    },
    getActiveUsers() {
        return axios.get(`${API_URL}/api/Users/ActiveUsers`);
    }
};

export default UserService;