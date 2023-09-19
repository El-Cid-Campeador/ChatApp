<template>
    <NavBar />
    <div>
        <ul :class="$style['online-users-container']">
            <template v-for="onlineUser in onlineUsers" :key="onlineUser.id">
                <li v-show="userData!.id !== onlineUser.id" :class="$style['online-user']" @click="getRoomId(onlineUser.id, onlineUser.username)">
                    <img v-show="onlineUser.profilePicPath" :src="onlineUser.profilePicPath" :alt="`${onlineUser.username}'s photo`" />
                    <div style="display: flex; align-items: center;">
                        <h1 style="margin-right: 5px;">{{ onlineUser.username }}</h1> (<i>{{ `${onlineUser.firstName} ${onlineUser.lastName}` }}</i>)
                    </div>
                </li>
            </template>
        </ul>
    </div>
</template>

<script setup lang="ts">
    import NavBar from '../components/NavBar.vue';
    import { useRouter } from 'vue-router';
    import { inject, onBeforeMount, onMounted, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import { fetcher } from '../functions';

    const router = useRouter();

    const onlineUsers = ref<OnlineUser[]>([]);

    const userData = ref<OnlineUser>();

    const socket = inject<Socket>('Socket');

    async function getRoomId(userId: string, username: string) {
        try {
            const { data } = await fetcher.get(`/rooms?userId0=${userData.value?.id}&userId1=${userId}`);
    
            router.push({ path: `/room/${data.result}`, query: { name: username } });
        } catch (err) {
            router.push({ path: `/` });
        }
    }

    socket!.on("connect_error", (err) => {
        console.log(err);
        
        router.push({ path: `/` });
    });

    socket!.on('getAllUsers', (users: OnlineUser[]) => {
        onlineUsers.value = users;
    });

    onBeforeMount(() => {
        try {
            
            userData.value = JSON.parse(localStorage.getItem('data')!);
        } catch (error) {
            router.push({ path: `/` });
        }
    });

    onMounted(async () => {
        try {
            socket!.auth = userData.value!;
    
            socket!.connect();
    
            socket!.emit('join', JSON.stringify({ ...userData.value }));
        } catch (error) {
            router.push({ path: `/` });
        }

        console.clear();
    });
</script>

<style module>
    .online-users-container {
        margin: 10px;
    }

    .online-user {
        width: 50%;
        border-radius: 20px;
        background-color: #eee;
        cursor: pointer;
        padding: 10px;
        margin-bottom: 10px;
    }

    @media screen and (width <= 320px) {
        .online-user {
            width: 95%;
        }
    }
</style>
