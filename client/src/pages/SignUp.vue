<template>
    <form @submit.prevent="handleSubmit()">
        <div>
            <input type="text" v-model="firstName" placeholder="First Name" required />
        </div>
        <div>
            <input type="text" v-model="lastName" placeholder="Last Name" required />
        </div>
        <div>
            <input type="text" v-model="userName" placeholder="Username" required />
        </div>
        <div>
            <input type="email" v-model="email" placeholder="Email" required />
        </div>
        <div>
            <input :type="isPasswordShowing ? 'text' : 'password' " v-model="password" placeholder="Password" required />
            <button type="button" @click="isPasswordShowing = !isPasswordShowing">
                <span v-if="isPasswordShowing">Hide password</span>
                <span v-else>Show password</span>
            </button>
        </div>
        <input type="submit" value="Sign Up" />
    </form>
    <h1 v-show="errorMsg !== ''"></h1>
</template>

<script setup lang="ts">
    import axios from 'axios';
    import { ref } from 'vue';
    import { useRouter } from 'vue-router';
    
    const router = useRouter(); 

    const firstName = ref('');
    const lastName = ref('');
    const userName = ref('');
    const email = ref('');
    const password = ref('');
    const isPasswordShowing = ref(false);
    const errorMsg = ref('');

    async function handleSubmit() {
        try {
            await axios.post(`http://localhost:5057/api/signup`, {
                firstName: firstName.value,
                lastName: firstName.value,
                userName: userName.value,
                email: email.value,
                password: password.value
            });

            router.push({ path: '/' });
        } catch (error) {
            errorMsg.value = (error as Error).message;
        }
    }
</script>
