<template>
    <nav>
        <span @click="logout()" :class="$style.logout">Log out</span>
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

<style module>
    nav {
        background-color: black;
        height: 50px;
    }

    .logout {
        color: white;
        cursor: pointer;
    }
</style>
