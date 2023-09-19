import axios from "axios";

const fetcher = axios.create({
    baseURL: `http://localhost:5057/api`,
    withCredentials: true
});

function isAnyOfTheAttributesAnEmptyString(obj: any) {
    for (const key in obj) {
        if (typeof obj[key] === 'string') {
            if (obj[key] === '') {
                return true;
            }
        }
    }

    return false;
}

export { fetcher, isAnyOfTheAttributesAnEmptyString };
