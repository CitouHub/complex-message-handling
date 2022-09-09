import Request from "../util/requesthandler"

export default {
    getProcessChannelPolicies: async () => await Request.send({
        url: `/processchannelpolicy`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    updateProcessChannelPolicy: async (name, processChannelPolicy) => await Request.send({
        url: `/processchannelpolicy/${name}`,
        method: 'PUT',
        data: processChannelPolicy
    }).then((response) => {
        return Request.handleResponse(response)
    }),
}