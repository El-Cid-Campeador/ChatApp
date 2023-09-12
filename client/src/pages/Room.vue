<template>
    <div>
        <div>
            <h1 v-if="isLoading">
                Loading...
            </h1>
            <div v-else >
                <ul v-if="errorMsg !== ''">
                    <template v-for="msg, i in msgList" :key="msg.id" >
                        <li :class="$style.msg">
                            {{ msg.content }} {{ msg.senderId }} {{ msg.date }}
                            <p v-if="i === msgList.length - 1 && isSending">Sending...</p>
                        </li>
                    </template>
                </ul>
                <h2 v-else>{{ errorMsg }}</h2>
            </div>
        </div>

        <form @submit.prevent="submitMsg()">
            <input type="text" v-model="inputMsg" />
        </form>
    </div>
</template>

<script setup lang="ts">
    import { inject, onBeforeMount, onMounted, ref } from 'vue';
    import { useRoute } from 'vue-router';
    import axios from 'axios';
    import { Socket } from 'socket.io-client';

    const route = useRoute();

    const roomId = ref(route.params.id);

    const isSending = ref(false);
    const inputMsg = ref('');
    const senderId = ref(JSON.parse(localStorage.getItem('data')!).id);
    const msgList = ref<any[]>([]);
    const isLoading = ref(false);
    const errorMsg = ref('');

    const socket = inject<Socket>('Socket');

    async function getMsgs() {
        isLoading.value = true;

        try {
            const { data } = await axios.get(`http://localhost:5057/api/messages/${roomId.value}`, {
                withCredentials: true
            });

            errorMsg.value = '';

            msgList.value = data;
        } catch (error) {
            errorMsg.value = (error as Error).message;
        } finally {
            isLoading.value = false;
        }
    }

    async function submitMsg() {
        isSending.value = true;

        try {
            const payload = {
                id: crypto.randomUUID(),
                senderId: senderId.value,
                content: inputMsg.value,
                date: new Date().toISOString()
            }

            msgList.value.push({ ...payload });

            await axios.post(`http://localhost:5057/api/messages/${roomId.value}`, payload, {
                withCredentials: true
            });
    
            socket!.emit('from', { msg: payload, roomId: roomId.value });
    
            inputMsg.value = '';

            errorMsg.value = '';
        } catch (error) {
            errorMsg.value = (error as Error).message;
        } finally {
            isSending.value = false;
        }
    }

    socket!.on('to', (msg: any) => {
        msgList.value.push(msg);
    });

    onBeforeMount(async () => {
        await getMsgs();
    });

    onMounted(async () => {       
        socket!.emit('joinRoom', `${roomId.value}`);
    });
</script>

<style module>
    .msg {
        border-radius: 10px;
        background-color: cyan; 
        padding: 10px;
        margin-top: 20px;
        margin-bottom: 20px;
        width: fit-content;
    }
</style>
