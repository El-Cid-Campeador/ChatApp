<template>
    <NavBar />
    <div :class="$style.tmp">
        <ul>
            <template v-for="onlineUser in onlineUsers" :key="onlineUser.id">
                <li v-show="id !== onlineUser.id">
                    <div @click="getRoomId(onlineUser.id)" style="cursor: pointer;">{{ onlineUser.id }}</div>
                </li>
            </template>
        </ul>
        <router-view :key="Date.now()" />
    </div>
</template>

<script setup lang="ts">
    import NavBar from '../components/NavBar.vue';
    import { inject, onMounted, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import axios from 'axios';
    import { useRouter } from 'vue-router';

    const router = useRouter();

    const onlineUsers = ref<any[]>([]);

    const socket = inject<Socket>('Socket');

    const id = ref(JSON.parse(localStorage.getItem('data')!).id);

    async function getRoomId(userId: string) {
        const { data } = await axios.get(`http://localhost:5057/api/rooms?userId0=${id.value}&userId1=${userId}`, {
            withCredentials: true
        });

        router.push({ path: `/home/room/${data.result}` });
    }

    socket!.on("connect_error", (err) => {
        console.log(err.message);
    });

    socket!.on('getAllUsers', (users: any[]) => {
        onlineUsers.value = users;
    });

    onMounted(async () => {
        socket!.auth = { id: id.value };

        socket!.connect();

        socket!.emit('join', JSON.stringify({ id: id.value }));
    });
</script>

<style module>
    .tmp {
        display: grid;
        grid-template-columns: repeat(2, 50%);
    }
</style>
