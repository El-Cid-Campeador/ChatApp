import { createWebHistory, createRouter, RouteRecordRaw } from "vue-router";
import NotFound from "./pages/NotFound.vue";
import Login from "./pages/Login.vue";
import SignUp from "./pages/SignUp.vue";
import Home from "./pages/Home.vue";
import Room from "./pages/Room.vue";

const routes: RouteRecordRaw[] = [
    {
        path: '/',
        name: 'Login',
        component: Login
    },
    {
        path: '/signup',
        name: 'SignUp',
        component: SignUp
    },
    {
        path: '/home',
        name: 'Home',
        component: Home, 
        children: [
            {
                path: 'room/:id',
                name: 'Room',
                component: Room
            }
        ]
    },
    {
        path: "/:catchAll(.*)",
        component: NotFound
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;
