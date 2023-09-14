import axios from "axios";

const fetcher = axios.create({
    url: `http://localhost:5057/api`,
    withCredentials: true
});

export { fetcher };
