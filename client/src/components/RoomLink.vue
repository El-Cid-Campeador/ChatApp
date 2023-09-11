<template>
    <li>
        <router-link :to="{ path: `/home/room/${roomId}` }">{{ props.onlineUser.id }}</router-link>
    </li>
</template>

<script setup lang="ts">
    import axios from 'axios';
    import { onBeforeMount, ref } from 'vue';

    const props = defineProps({
        onlineUser: {
            type: Object,
            required: true
        },
        id: {
            type: String,
            required: true
        }
    });

    const roomId = ref('');

    onBeforeMount(async () => {
        if (props.id !== props.onlineUser.id) {
            const { data } = await axios.get(`http://localhost:5057/api/rooms?userId0=${props.id}&userId1=${props.onlineUser.id}`, {
                withCredentials: true
            });
    
            roomId.value = data.result;
        }
    });
</script>
