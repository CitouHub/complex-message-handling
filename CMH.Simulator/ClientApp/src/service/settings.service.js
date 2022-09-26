import Request from "../util/requesthandler"

const settingsService = {
    getSettings: async () => await Request.send({
        url: `/config`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    updateSettings: async (settings) => await Request.send({
        url: `/config`,
        method: 'PUT',
        data: settings
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    resetSettings: async () => await Request.send({
        url: `/config/reset`,
        method: 'PUT',
    }).then((response) => {
        return Request.handleResponse(response)
    })
}

export default settingsService;