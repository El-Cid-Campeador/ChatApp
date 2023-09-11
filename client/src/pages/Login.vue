<template>
    <form @submit.prevent="handleSubmit()">
        <div>
            <input type="text" v-model="username" placeholder="Username" required />
        </div>
        <div>
            <input :type="isPasswordShowing ? 'text' : 'password' " v-model="password" placeholder="Password" required />
            <button type="button" @click="isPasswordShowing = !isPasswordShowing">
                <span v-if="isPasswordShowing">Hide password</span>
                <span v-else>Show password</span>
            </button>
        </div>
        <input type="submit" value="Login" />
    </form>
    <h1 v-show="errorMsg !== ''"></h1>
    <router-link :to="{ name: 'SignUp' }">Sign Up</router-link>
</template>

<script setup lang="ts">
    import axios from 'axios';
    import { ref } from 'vue';
    import { useRouter } from 'vue-router';

    const router = useRouter(); 

    const username = ref('');
    const password = ref('');
    const isPasswordShowing = ref(false);
    const errorMsg = ref('');

    async function handleSubmit() {
        try {
            const { data } = await axios.post(`http://localhost:5057/api/login`, {
                usernameOrEmail: username.value,
                password: password.value
            }, {
                withCredentials: true
            });

            localStorage.setItem('data', JSON.stringify({ id: data.id, username: data.username }));            
            
            errorMsg.value = '';

            router.push({ path: '/home' });
        } catch (error) {
            errorMsg.value = (error as Error).message;
        }
    }
</script>
