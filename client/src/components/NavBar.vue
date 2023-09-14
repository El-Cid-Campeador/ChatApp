<template>
    <nav>
        <span @click="logout()" :class="$style.logout">Log out</span>
    </nav>
</template>

<script setup lang="ts">
    import { useRouter } from 'vue-router';
    import { inject, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import { fetcher } from '../functions';

    const router = useRouter();

    const errorMsg = ref('');

    const socket = inject<Socket>('Socket');

    async function logout() {
        try {
            socket!.disconnect();

            localStorage.setItem('data', '');

            await fetcher.delete(`http://localhost:5057/api/logout`);

            router.push({ path: '/' });
        } catch (error) {
            errorMsg.value = (error as Error).message;
        }
    }
</script>

<style module>
    .logout {
        color: white;
        cursor: pointer;
    }
</style>
