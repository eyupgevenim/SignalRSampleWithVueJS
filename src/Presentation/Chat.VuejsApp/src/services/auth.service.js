import axios from 'axios';

const API_URL = 'http://localhost:5100';

const AuthService = {
    login(user) {
        return axios
            .post(`${API_URL}/api/Auth/token`, {
                Email: user.email,
                Password: user.password
            })
            .then(response => {

                console.log('login - response:', response);

                if (response.data.success) {
                    localStorage.setItem('user', JSON.stringify(response.data.data));
                    return response.data.data;
                }

                return null;
            });
    },
    logout() {
        return axios.post(`${API_URL}/api/Auth/logout`);
    },
    register(user) {
        return axios.post(`${API_URL}/api/Auth/signup`, {
            username: user.username,
            email: user.email,
            password: user.password
        });
    }
};

export default AuthService;
