import Vue from 'vue'
import AuthService from '../services/auth.service';

const localStorageUser = JSON.parse(localStorage.getItem('user'));
const initialState = { status: { isAuthenticated: !!localStorageUser }, user: localStorageUser || null };

export const auth = {
    namespaced: true,
    state: initialState,
    getters: {
        isAuthenticated: state => state.status.isAuthenticated,
        user: state => state.user
    },
    actions: {
        login: ({ commit }, userModel) => {
            return AuthService.login(userModel).then(
                user => {
                    if (user) {
                        commit('loginSuccess', user);
                        //TODO:...
                        Vue.prototype.startSignalR(user.accessToken);
                    }
                    
                    return Promise.resolve(user);
                },
                error => {
                    console.log("login - error", error);
                    commit('logoutSuccess');
                    return Promise.reject(error);
                }
            );
        },
        logout: ({ commit }) => {
            //commit('logoutSuccess');
            return AuthService.logout().then(
                result => {
                    //...
                    commit('logoutSuccess');

                    //TODO:...
                    Vue.prototype.stopSignalR()

                    return Promise.resolve(result);
                },
                error => {
                    //...
                    commit('logoutSuccess');

                    //TODO:...
                    Vue.prototype.stopSignalR()

                    return Promise.reject(error);
                }
            );
        }
    },
    mutations: {
        loginSuccess(state, user) {
            state.status.isAuthenticated = true;
            state.user = user;
        },
        logoutSuccess(state) {
            localStorage.removeItem('user');
            state.status.isAuthenticated = false;
            state.user = null;
        }
    }
};