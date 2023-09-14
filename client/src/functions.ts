import axios from "axios";

const fetcher = axios.create({
    baseURL: `http://localhost:5057/api`,
    withCredentials: true
});

export { fetcher };
