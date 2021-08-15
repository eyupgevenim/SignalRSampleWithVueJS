import Vue from 'vue';
import axios from 'axios';

//// By default the project simulates the client application being hosted independently from the server
//// These lines setup axios so all requests are sent to the backend server
//// However, you can comment them and the site will behave as if both client and server were hosted in localhost:8080
//// due to the proxy dev server configured in vue.config.js
axios.defaults.baseURL = 'http://localhost:5100'; // same as the Url the server listens to
axios.defaults.withCredentials = true;

// Setup axios as the Vue default $http library
Vue.prototype.$http = axios;

axios.interceptors.request.use((config) => {
    const user = JSON.parse(localStorage.getItem('user'));
    if (user) {
        const authToken = user.accessToken;//.access_token;
        if (authToken) {
            config.headers.Authorization = `Bearer ${authToken}`;
        }
    }
    return config;
}, (err) => {
    console.log("axios.interceptors.request.use -> ", err);
    //What do we do when we get errors?
    return Promise.reject(err);
});


//export default function authHeader() {
//    let user = JSON.parse(localStorage.getItem('user'));

//    if (user && user.accessToken) {
//        return { Authorization: 'Bearer ' + user.accessToken }; // for Spring Boot back-end
//        // return { 'x-access-token': user.accessToken };       // for Node.js Express back-end
//    } else {
//        return {};
//    }
//}

