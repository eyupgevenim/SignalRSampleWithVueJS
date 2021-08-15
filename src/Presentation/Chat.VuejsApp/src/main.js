import Vue from 'vue';
import "./plugins";
import App from './App.vue';
import router from "./router";
import store from "./store";
import "./services/auth-header";

Vue.config.productionTip = true;

let vue = new Vue({
    router: router,
    store: store,
    render: h => h(App)
}).$mount('#app');