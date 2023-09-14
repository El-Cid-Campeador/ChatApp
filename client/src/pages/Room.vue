<template>
    <div :class="$style.container">
        <div>
            <h1 v-if="isLoading">
                Loading...
            </h1>
            <div v-else >
                <h1>{{ name }}</h1>
                <ul>
                    <template v-for="msg, i in msgList" :key="msg.id" >
                        <li :class="[msg.senderId === senderId ? $style.msg : $style['others-msg'] ]" :title="new Date(msg.date).toLocaleString()">
                            <b>{{ msg.content }}</b>
                            <p v-if="i === msgList.length - 1 && isSending">Sending...</p>
                        </li>
                    </template>
                </ul>
            </div>
        </div>

        <div :class="$style['input-msg']">
            <textarea cols="60" rows="5" v-model="inputMsg"></textarea>
            <button @click="submitMsg()">Send</button>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { useRoute, useRouter } from 'vue-router';
    import { inject, onBeforeMount, onMounted, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import { fetcher } from '../functions';

    const route = useRoute();
    const router = useRouter();

    const roomId = ref(route.params.id);

    const isSending = ref(false);
    const inputMsg = ref('');
    const senderId = ref(JSON.parse(localStorage.getItem('data')!).id);
    const msgList = ref<Message[]>([]);
    const isLoading = ref(false);
    const errorMsg = ref('');

    const name = ref('');

    const socket = inject<Socket>('Socket');

    async function getMsgs() {
        isLoading.value = true;

        try {
            const { data } = await fetcher.get(`/messages/${roomId.value}`, {
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
            if (inputMsg.value) {
                const payload = {
                    id: crypto.randomUUID(),
                    senderId: senderId.value,
                    content: inputMsg.value,
                    date: new Date()
                }
    
                msgList.value.push({ ...payload });
    
                await fetcher.post(`/messages/${roomId.value}`, payload, {
                    withCredentials: true
                });
        
                socket!.emit('from', { msg: payload, roomId: roomId.value });
        
                inputMsg.value = '';
    
                errorMsg.value = '';
            }

        } catch (error) {
            errorMsg.value = (error as Error).message;
        } finally {
            isSending.value = false;
        }
    }

    socket!.on('to', (msg: Message) => {
        msgList.value.push(msg);
    });

    onBeforeMount(async () => {
        if (!route.query.name) {
            router.push({ path: `/home` })
        }

        name.value = route.query.name as string;

        await getMsgs();
    });

    onMounted(async () => {       
        socket!.emit('joinRoom', `${roomId.value}`);
    });
</script>

<style module>
    .container {
        overflow-y: auto;
        padding: 10px;
        width: 100%;
        height: 100vh;
        background-color: rgb(255, 246, 246);
    }
    
    .msg, .others-msg {
        border-radius: 10px;
        background-color: #0077ff; 
        color: white;
        padding: 10px;
        margin-top: 20px;
        margin-bottom: 20px;
        width: fit-content;
    }

    .others-msg {
        background-color: #b182ee;
        margin-left: 50px;
    }

    .input-msg {
        display: flex;
        align-items: flex-end; 
        position: fixed;
        right: 20px;
        bottom: 20px;
    }
</style>
