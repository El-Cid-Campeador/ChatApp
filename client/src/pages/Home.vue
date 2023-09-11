<template>
    <NavBar />
    <div :class="$style.tmp">
        <ul>
            <template v-for="onlineUser in onlineUsers" :key="onlineUser.id">
                <RoomLink v-if="id !== onlineUser.id" :onlineUser="onlineUser" :id="id" />
            </template>
        </ul>
        <router-view />
    </div>
</template>

<script setup lang="ts">
    import NavBar from '../components/NavBar.vue';
    import { inject, onMounted, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import RoomLink from '../components/RoomLink.vue';

    const onlineUsers = ref<any[]>([]);

    const socket = inject<Socket>('Socket');

    const id = ref(JSON.parse(localStorage.getItem('data')!).id);

    socket!.on("get-users", async (users: any[]) => {
        onlineUsers.value = users;
    });

    onMounted(async () => {
        socket!.auth = { id: id.value };

        socket!.connect();

        socket!.emit("new-user-add", id.value);
    });
</script>

<style module>
    .tmp {
        display: grid;
        grid-template-columns: repeat(2, 50%);
    }
</style>
