import axios from "axios"
import Config from "./config"

axios.interceptors.response.use(response => {
    return response;
}, error => {
    if (error.response.status === 401) {
        window.location.href = '/';
    }
});

export default {
    send: async (request) => {
        try {
            let url = Config.apiURL() + request.url;
            let headers = {
             
            }

            if (request.method === 'GET') {
                return await axios.get(url, { headers });
            } else if (request.method === 'POST') {
                return await axios.post(url, request.data, { headers });
            } else if (request.method === 'PUT') {
                return await axios.put(url, request.data, { headers });
            } else if (request.method === 'DELETE') {
                return await axios.delete(url, { headers });
            }
        } catch (error) {
            console.error(error);
        }
    },
    handleResponse: async (response) => {
        try {
            if (response) {
                if (response.status === 200) {
                    return response.data;
                } else {
                    console.warn("Unexpected API result!");
                    console.warn(response);
                }
            }
        } catch (error) {
            console.error(error);
        }
    }
}