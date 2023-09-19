<template>
    <div class="container">
        <form @submit.prevent="handleSubmit()">
            <div>
                <input type="text" v-model="firstName" placeholder="First Name" minlength="2" maxlength="50" required />
            </div>
            <div>
                <input type="text" v-model="lastName" placeholder="Last Name" minlength="2" maxlength="50" required />
            </div>
            <div>
                <input type="text" v-model="username" placeholder="Username" minlength="8" maxlength="50" required />
            </div>
            <div>
                <input type="email" v-model="email" placeholder="Email" minlength="8" maxlength="50" required />
            </div>
            <div>
                <input :type="isPasswordShowing ? 'text' : 'password' " v-model="password" placeholder="Password" minlength="8" maxlength="64" required />
                <button type="button" @click="isPasswordShowing = !isPasswordShowing">
                    <span v-if="isPasswordShowing">Hide password</span>
                    <span v-else>Show password</span>
                </button>
            </div>
            <input type="submit" value="Sign Up" />
        </form>
    
        <h1 v-show="errorMsg">{{ errorMsg }}</h1>

        <router-link :to="{ name: 'Login' }">Sign In</router-link>
    </div>
</template>

<script setup lang="ts">
    import { useRouter } from 'vue-router';
    import { ref } from 'vue';
    import { fetcher, isAnyOfTheAttributesAnEmptyString } from '../functions';
    
    const router = useRouter(); 

    const firstName = ref('');
    const lastName = ref('');
    const username = ref('');
    const email = ref('');
    const password = ref('');
    
    const isPasswordShowing = ref(false);
    const errorMsg = ref('');

    function isPayloadValid(payload: any) {
        if (isAnyOfTheAttributesAnEmptyString(payload)) {
            return false;
        }

        if (!payload.email.toLowerCase().match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)) {
            return false;
        }

        if (payload.firstName.length < 2 || payload.firstName.length > 50 ||
            payload.lastName.length < 2 || payload.lastName.length > 50 ||
            payload.username.length < 8 || payload.username.length > 50 ||
            payload.email.length < 8 || payload.email.length > 50 ||
            payload.password.length < 8 || payload.password.length > 64
        ) {
            return false;
        }

        return true;
    }

    async function handleSubmit() {
        try {
            const payload = {
                firstName: firstName.value,
                lastName: lastName.value,
                username: username.value,
                email: email.value,
                password: password.value
            };

            if (!isPayloadValid(payload)) {
                throw new Error('Invalid Credentials!');
            }

            await fetcher.post(`/signup`, payload);

            router.push({ path: '/' });
        } catch (err) {
            errorMsg.value = (err as Error).message;
        }
    }
</script>

<style scoped>
    a {
        color: cornflowerblue;
    }
</style>
