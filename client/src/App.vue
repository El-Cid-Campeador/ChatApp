<template>
    <router-view />
</template>

<script setup lang="ts">
    import { io } from 'socket.io-client';
    import { provide } from 'vue';
    const socket = io('http://localhost:9000', { autoConnect: false });

    provide('Socket', socket);
</script>
