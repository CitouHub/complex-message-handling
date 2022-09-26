import axios from "axios"

const appSettingsService = {
    get: async () => {
        try {
            let result = await axios.get('/api/AppSettings');
            return result.data;
        } catch (error) {
            console.error(error);
        }
    }
}

export default appSettingsService;