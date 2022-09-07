import Request from "../util/requesthandler"

export default {
    getQueues: async (queuePrefix) => await Request.send({
        url: `/queue/${queuePrefix}/`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    })
}