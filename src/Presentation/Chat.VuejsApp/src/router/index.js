import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import Login from "../views/Login.vue";
import Chat from "../views/Chat.vue";
import store from "../store";

Vue.use(VueRouter);

const routes = [
    {
        path: "/",
        name: "home",
        component: Home
    },
    {
        path: "/login",
        name: "login",
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        //component: () => import(/* webpackChunkName: "about" */ "../views/Login.vue")
        component: Login
    },
    {
        path: "/chat",
        name: "chat",
        component: Chat,
        meta: {
            requiresAuth: true
        }
    }
];

const beforeEach = async (to, from, next) => {
    let status = store.state.auth.status || { isAuthenticated: false }
    if (!status.isAuthenticated && to.matched.some(record => record.meta.requiresAuth)) {

        router.push({ 'path': 'login?returnPath=' + to.path });

    } else{
        next();
    }
};

const router = new VueRouter({
    mode: "history",
    base: process.env.BASE_URL,
    routes: routes
});

router.beforeEach(beforeEach);

export default router;