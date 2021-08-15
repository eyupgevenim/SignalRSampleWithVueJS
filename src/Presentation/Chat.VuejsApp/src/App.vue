<template>
    <div id="app">

        <div id="nav">
            <top-header></top-header>
        </div>

        <router-view />

    </div>
</template>
<script>
    import TopHeader from "@/components/top-header.vue";
    export default {
        components: { "top-header": TopHeader}
    };
</script>

<style scoped>
</style>

