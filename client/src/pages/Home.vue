<template>
    <NavBar />
    <div :class="$style.container">
        <ul :class="$style['online-users-container']">
            <template v-for="onlineUser in onlineUsers" :key="onlineUser.id">
                <li v-show="userData!.id !== onlineUser.id" :class="$style['online-users']">
                    <img v-show="onlineUser.profilePicPath" :src="onlineUser.profilePicPath" :alt="`${onlineUser.username}'s photo`" />
                    <div @click="getRoomId(onlineUser.id, onlineUser.username)" :title="`${onlineUser.firstName} ${onlineUser.lastName}`">
                        {{ onlineUser.username }}
                    </div>
                </li>
            </template>
        </ul>

        <router-view :key="Date.now()" />
    </div>
</template>

<script setup lang="ts">
    import NavBar from '../components/NavBar.vue';
    import { inject, onBeforeMount, onMounted, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import axios from 'axios';
    import { useRouter } from 'vue-router';

    const router = useRouter();

    const onlineUsers = ref<OnlineUser[]>([]);

    const userData = ref<OnlineUser>();

    const socket = inject<Socket>('Socket');

    async function getRoomId(userId: string, username: string) {
        const { data } = await axios.get(`http://localhost:5057/api/rooms?userId0=${userData.value?.id}&userId1=${userId}`, {
            withCredentials: true
        });

        router.push({ path: `/home/room/${data.result}`, query: { name: username } });
    }

    socket!.on("connect_error", (err) => {
        console.log(err.message);
    });

    socket!.on('getAllUsers', (users: OnlineUser[]) => {
        onlineUsers.value = users;
    });

    onBeforeMount(() => {
        userData.value = JSON.parse(localStorage.getItem('data')!);
    });

    onMounted(async () => {
        socket!.auth = userData.value!;

        socket!.connect();

        socket!.emit('join', JSON.stringify({ ...userData.value }));
    });
</script>

<style module>
    .container {
        display: grid;
        grid-template-columns: repeat(2, 50%);
    }

    .online-users-container {
        margin: 10px;
    }

    .online-users {
        border-radius: 20px;
        background-color: #eee;
        cursor: pointer;
        padding: 10px;
        margin-bottom: 10px;
    }
</style>
