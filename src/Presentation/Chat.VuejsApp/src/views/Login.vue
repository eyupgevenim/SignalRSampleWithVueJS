<template>
    <b-container class="bv-example-row">
        <b-row class="justify-content-md-center mt-5" align-v="center" align-h="center">
            <b-col cols="12" md="6">
                <b-card>
                    <b-form @submit="onSubmit" @reset="onReset" v-if="show">
                        <b-row class="my-1 mb-2">
                            <b-col sm="12">
                                <h4 class="text-center">Login</h4>
                            </b-col>
                        </b-row>
                        <b-card-body>
                            <b-row class="my-1 mb-3">
                                <b-col sm="12">
                                    <b-form-input type="email" 
                                                  id="email"
                                                  name="email"
                                                  placeholder="Email"
                                                  :state="isValidEmail"
                                                  v-on:change="emailChange"
                                                  v-model="user.email">
                                    </b-form-input>
                                </b-col>
                            </b-row>
                            <b-row class="my-1 mb-2">
                                <b-col sm="12">
                                    <b-form-input type="password"
                                                  id="password"
                                                  name="password"
                                                  placeholder="Password"
                                                  :state="isValidPassword"
                                                  v-on:change="passwordChange"
                                                  v-model="user.password">
                                    </b-form-input>
                                </b-col>
                            </b-row>
                        </b-card-body>
                        <b-row class="my-1 mb-3">
                            <b-col sm="12">
                                <div class="text-center" align-v="center" align-h="center">
                                    <b-button type="submit" variant="success" style="width: 180px;">Login</b-button>
                                    <!--<b-button type="reset" variant="danger">Reset</b-button>-->
                                </div>
                            </b-col>
                        </b-row>
                    </b-form>
                </b-card>
            </b-col>
        </b-row>
        <b-row class="justify-content-md-center mt-5" align-v="center" align-h="center">
            <b-col cols="12" md="6">
                eyup@gevenim.com:Pass@word1<br />
                atest2@user.com:Pass@word2<br />
                btest3@cuser.com:Pass@word3<br />
                dtest4@duser.com:Pass@word4
            </b-col>
        </b-row>
    </b-container>
</template>

<script>
    import router from "../router";

    function validateEmail(email) {
        const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(String(email).toLowerCase());
    }

    export default {
        name: "Login",
        data() {
            return {
                user: {
                    email: '',
                    password: '',
                },
                
                isValidEmail: null,
                isValidPassword: null,

                show: true
            };
        },
        methods: {
            emailChange(event) { this.isValidEmail = validateEmail(event); },
            passwordChange(event) { this.isValidPassword = !!(event); },
            onSubmit(event) {
                event.preventDefault();
                console.log("JSON.stringify(this.user): ",JSON.stringify(this.user));

                if (this.isValidEmail && this.isValidPassword) {
                    
                    this.$store.dispatch('auth/login', this.user).then(user => {

                        console.log('Login.vue - user:', user);
                        if (user) {
                            const returnPath = this.$route.query.returnPath || '/';
                            router.push({ 'path': returnPath });
                        }

                    });
                }
            },
            onReset(event) {
                event.preventDefault()

                // Reset our form values
                this.user.email = ''
                this.user.password = ''

                this.isValidEmail = null
                this.isValidPassword = null

                // Trick to reset/clear native browser form validation state
                this.show = false
                this.$nextTick(() => {
                    this.show = true
                })
            }
        }
    };
</script>
<style scoped>
</style>