<template>
    <b-row class="text-center" @click="onUserAvatarClick">
        <b-col>
            <b-list-group-item class="d-flex align-items-center" 
                               :class="avatar.active" 
                               :style="[avatar.index == 0 ? { 'border-top':'none'} : null]">
                <b-avatar badge
                          :badge-variant="avatar.badgeVariant"
                          :href="avatar.href"
                          :variant="avatar.variant"
                          :text="avatar.text"
                          class="align-baseline">
                </b-avatar>
                <span class="mr-auto ml-3">{{ avatar.name }}</span>
            </b-list-group-item>
        </b-col>
    </b-row>
</template>
<script>
    export default {
        name: "UserAvatar",
        props: [ "avatar"],
        data() {
            return {

            };
        },
        methods: {
            onUserAvatarClick(event) {
                this.$emit('onUserAvatarClickCallback', this.avatar)
            },
        }
    };
</script>
<style scoped>
    .b-avatar {
        margin-left: calc(-9px);
        margin-right: calc(-9px);
        width: 60px;
        height: 60px;
    }

    .mr-auto {
        margin-left: 15px !important;
    }

    .list-group-item {
        border-right: none !important;
        border-left: none !important;
        border-bottom: none !important;
    }

    .active{
        background-color:#f2f2f2 !important;
    }

</style>