<template>
    <NavBar />
    <div class="container">
        <div>
            <h1 v-if="isLoading">
                Loading...
            </h1>
            <div v-else >
                <h2 v-if="errorGetMsgs" >{{ errorGetMsgs }}</h2>
                <div v-else :class="$style['msgs-container']">
                    <h1>{{ name }}</h1>
                    <ul>
                        <template v-for="msg, i in msgList" :key="msg.id" >
                            <li :class="[msg.senderId === senderId ? $style.msg : $style['others-msg'] ]" :title="new Date(msg.date).toLocaleString()">
                                <b>{{ msg.content }}</b>
                                <p v-if="i === msgList.length - 1 && isSending">{{ sendingState }}</p>
                            </li>
                        </template>
                    </ul>
                </div>
            </div>
        </div>

        <div v-show="!errorGetMsgs" :class="$style['input-msg']">
            <textarea v-model="inputMsg"></textarea>
            <button @click="submitMsg()">Send</button>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { useRoute, useRouter } from 'vue-router';
    import { inject, onBeforeMount, onMounted, ref } from 'vue';
    import { Socket } from 'socket.io-client';
    import { fetcher } from '../functions';
    import NavBar from '../components/NavBar.vue';

    const route = useRoute();
    const router = useRouter();

    const roomId = ref(route.params.id);

    const isSending = ref(false);
    const inputMsg = ref('');
    const senderId = ref<string>(JSON.parse(localStorage.getItem('data')!).id);
    const msgList = ref<Message[]>([]);
    const errorGetMsgs = ref('');
    const isLoading = ref(false);
    const sendingState = ref('');
    const name = ref('');

    const socket = inject<Socket>('Socket');

    async function getMsgs() {
        isLoading.value = true;

        try {
            const { data } = await fetcher.get(`/messages/${roomId.value}`);
            
            msgList.value = data;

            errorGetMsgs.value = '';
        } catch (err) {
            errorGetMsgs.value = (err as Error).message;
        } finally {
            isLoading.value = false;
        }
    }

    async function submitMsg() {
        if (!inputMsg.value) {
            return;
        }

        isSending.value = true;

        sendingState.value = 'Sending...';

        const payload = {
            id: crypto.randomUUID(),
            senderId: senderId.value,
            content: inputMsg.value,
            date: new Date()
        }

        msgList.value.push({ ...payload });

        try {
            await fetcher.post(`/messages/${roomId.value}`, payload);
    
            inputMsg.value = '';

            socket!.emit('from', { msg: payload, roomId: roomId.value });
        } catch (err) {
            sendingState.value = 'Not sent!';
        } finally {
            isSending.value = false;
        }
    }

    function playSoundNotification() {
        new Audio('../../chat.mp3').play();
    }

    socket!.on("connect_error", (err) => {
        console.log(err);
        
        router.push({ path: `/` });
    });

    socket!.on('to', (msg: Message) => {
        console.log(msg.senderId);
        
        if (msg.senderId !== senderId.value) {
            playSoundNotification();
        }

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
    .msgs-container {
        overflow-y: auto;
        margin-bottom: 70px;
    }
    
    .msg, .others-msg {
        border-radius: 10px;
        background-color: #0077ff; 
        color: white;
        padding: 10px;
        margin-top: 20px;
        margin-bottom: 20px;
        width: fit-content;
        overflow-wrap: break-word;
        max-width: 300px;
    }

    .others-msg {
        background-color: #b182ee;
        margin-left: 50px;
    }

    .input-msg {
        display: flex;
        align-items: flex-end; 
        position: fixed;
        left: 0;
        right: 0;
        bottom: 0;
        z-index: 9999;
        margin-top: 100px;
        padding: 10px;
    }

    @media screen and (width <= 320px) {
        .msg, .others-msg {
            max-width: 200px;
        }
    }
</style>
