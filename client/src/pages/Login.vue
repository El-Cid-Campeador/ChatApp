<template>
    <div class="container">
        <form @submit.prevent="handleSubmit()">
            <div>
                <input type="text" v-model="usernameInput" placeholder="Username" required />
            </div>
            <div>
                <input :type="isPasswordShowing ? 'text' : 'password' " v-model="passwordInput" placeholder="Password" required />
                <button type="button" @click="isPasswordShowing = !isPasswordShowing">
                    <span v-if="isPasswordShowing">Hide password</span>
                    <span v-else>Show password</span>
                </button>
            </div>
            <input type="submit" value="Login" />
        </form>
    
        <h1 v-show="errorMsg">{{ errorMsg }}</h1>
    
        <router-link :to="{ name: 'SignUp' }">Sign Up</router-link>
    </div>
</template>

<script setup lang="ts">
    import { useRouter } from 'vue-router';
    import { ref } from 'vue';
    import { fetcher } from '../functions';

    const router = useRouter(); 

    const usernameInput = ref('');
    const passwordInput = ref('');
    
    const isPasswordShowing = ref(false);
    const errorMsg = ref('');

    async function handleSubmit() {
        try {
            const { data } = await fetcher.post(`/login`, {
                usernameOrEmail: usernameInput.value,
                password: passwordInput.value
            }) as { data: UserData };

            localStorage.setItem('data', JSON.stringify({ ...data }));            
            
            errorMsg.value = '';

            router.push({ path: '/home' });
        } catch (err) {
            errorMsg.value = 'Invalid credentials!';
        }
    }
</script>

<style scoped>
    h1 {
        color: red;
    }
</style>
