<template>
    <div class="container">
        <form @submit.prevent="handleSubmit()">
            <div>
                <input type="text" v-model="usernameInput" placeholder="Username or Email" minlength="8" maxlength="50" required />
            </div>
            <div>
                <input :type="isPasswordShowing ? 'text' : 'password' " v-model="passwordInput" placeholder="Password" minlength="8" maxlength="64" required />
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
    import { fetcher, isAnyOfTheAttributesAnEmptyString } from '../functions';

    const router = useRouter(); 

    const usernameInput = ref('');
    const passwordInput = ref('');
    
    const isPasswordShowing = ref(false);
    const errorMsg = ref('');

    async function handleSubmit() {
        try {
            const payload = {
                usernameOrEmail: usernameInput.value,
                password: passwordInput.value
            };

            if (isAnyOfTheAttributesAnEmptyString(payload) ||
                payload.usernameOrEmail.length < 8 || payload.usernameOrEmail.length > 50 ||
                payload.password.length < 8 || payload.password.length > 64
            ) {
                throw new Error('Invalid credentials!');
            }

            const { data } = await fetcher.post(`/login`, payload) as { data: UserData };

            localStorage.setItem('data', JSON.stringify({ ...data }));            
            
            errorMsg.value = '';

            router.push({ path: '/home' });
        } catch (err) {
            const error = (err as Error).message;

            if (error !== 'Invalid credentials!') {
                errorMsg.value = 'Incorrect credentials! Try again!';
            } else {
                errorMsg.value = error;
            }
        }
    }
</script>

<style scoped>
    a {
        color: cornflowerblue;
    }
    
    h1 {
        color: red;
    }
</style>
