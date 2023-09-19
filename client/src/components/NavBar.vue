<template>
    <nav>
        <router-link :to="{ name: 'Home' }">Home</router-link>
        <span @click="logout()" :class="$style.logout">Log out</span>
    </nav>
</template>

<script setup lang="ts">
    import { useRouter } from 'vue-router';
    import { inject } from 'vue';
    import { Socket } from 'socket.io-client';
    import { fetcher } from '../functions';

    const router = useRouter();

    const socket = inject<Socket>('Socket');

    async function logout() {
        try {
            await fetcher.delete(`http://localhost:5057/api/logout`);

            localStorage.removeItem('data');

            socket!.disconnect();

            router.push({ path: '/' });
        } catch (err) {
            console.log(err);
        }
    }
</script>

<style module>
    .logout {
        color: white;
        cursor: pointer;
    }
</style>
