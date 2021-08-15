<template>
    <div>
        <b-navbar toggleable="lg" type="dark" variant="info">
            <b-navbar-brand href="#">Chat VueJs</b-navbar-brand>
            <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav>
                    <b-nav-item to="/">Home</b-nav-item>
                    <template v-if="isAuthenticated">
                        <b-nav-item to="/chat">Chat</b-nav-item>
                    </template>
                </b-navbar-nav>

                <!-- Right aligned nav items -->
                <b-navbar-nav class="ml-auto">
                    <template v-if="isAuthenticated">
                        <b-nav-item>
                            <b-avatar variant="primary" :text="userInitial"></b-avatar>
                        </b-nav-item>
                        <b-nav-item @click="logout" style="padding-top:5px;">Logout</b-nav-item>
                    </template>
                    <template v-else>
                        <b-nav-item to="/login">Login</b-nav-item>
                    </template>
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>
    </div>
</template>

<script>
    import store from "../store";
    import router from "../router";

    const getInitials = function (fullName) {
        const allNames = fullName.trim().split(' ');
        const initial = (acc, curr, index) => (index === 0 || index === allNames.length - 1) ? `${acc}${curr.charAt(0).toUpperCase()}` : acc;
        return allNames.reduce(initial, '');
    };

    export default {
        data() {
            return {
                
            };
        },
        methods: {
            logout() {
                this.$store.dispatch('auth/logout').then((result) => {
                    if (result) {
                        router.push({ 'path': '/' });
                    }
                });
            }
        },
        computed: {
            isAuthenticated: () => store.state.auth.status.isAuthenticated,
            userInitial: function () {
                const user = store.state.auth.user;
                if (user) {
                    return getInitials(user.name);
                }
                return null;
            }
        }
    };
</script>

<style scoped>
</style>