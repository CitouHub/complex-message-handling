import axios from "axios"

async function get() {
    try {
        let result = await axios.get('/api/AppSettings');
        return result.data;
    } catch (error) {
        console.error(error);
    }
}

export default {
    get
}