<template>
    <nav>
        <router-link :to="{}">Home</router-link>
        <button @click="logout()">Logout</button>
    </nav>
</template>

<script setup lang="ts">
    import axios from 'axios';
    import { inject, ref } from 'vue';
    import router from '../router';
    import { Socket } from 'socket.io-client';

    const errorMsg = ref('');

    const socket = inject<Socket>('Socket');

    async function logout() {
        try {
            socket!.disconnect();

            await axios.delete(`http://localhost:5057/api/logout`, {
                withCredentials: true
            });

            router.push({ path: '/' });
        } catch (error) {
            errorMsg.value = (error as Error).message;
        }
    }
</script>
