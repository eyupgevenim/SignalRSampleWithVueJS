<template>
    <b-row>
        <b-col class="mb-2">
            <b-card>
                <b-media>
                    <b-media-body>
                        <p class="m-0" :class="data.align == 'left' ? 'text-left' :'text-right'">
                            <template v-if="data.align == 'left'">
                                <b-avatar variant="info" 
                                          :src="data.image"
                                          :text="data.initials" 
                                          class="avatar avatar-lg mr-4 rounded-circle"
                                          >
                                </b-avatar>
                                <b> {{data.name}} </b>
                            </template>
                            <template v-else>
                                <b> {{data.name}} </b>
                                <b-avatar variant="info" 
                                          :src="data.image"
                                          :text="data.initials" 
                                          class="avatar avatar-lg mr-4 rounded-circle">
                                </b-avatar>
                            </template>
                        </p>
                        <hr />
                        <b-alert show :variant="data.align == 'left' ? 'success' : 'secondary'">
                            <p>{{data.formatedDate}}</p>
                            <p class="m-0">
                                {{data.message}}
                            </p>
                        </b-alert>
                    </b-media-body>
                </b-media>
            </b-card>
        </b-col>
    </b-row>
</template>
<script>
    export default {
        name: "UserMessageDialog",
        props: ["data"]
    };
</script>
<style scoped>
    
</style>