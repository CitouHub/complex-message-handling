import Request from "../util/requesthandler"

export default {
    getPriorityStatistics: async () => await Request.send({
        url: `/statistics/messages/priority`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    getProcessStatistics: async () => await Request.send({
        url: `/statistics/messages/process`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    getRuntimeStatistics: async () => await Request.send({
        url: `/statistics/runtime`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    resetPriorityStatistics: async () => await Request.send({
        url: `/statistics/messages/priority/reset`,
        method: 'PUT'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    resetProcessStatistics: async () => await Request.send({
        url: `/statistics/messages/process/reset`,
        method: 'PUT'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    resetRuntimeStatistics: async () => await Request.send({
        url: `/statistics/runtime/reset`,
        method: 'PUT'
    }).then((response) => {
        return Request.handleResponse(response)
    })
}