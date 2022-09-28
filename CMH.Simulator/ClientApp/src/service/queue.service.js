import Request from "../util/requesthandler"

const queueService = {
    getQueueNames: async (queuePrefix) => await Request.send({
        url: `/queue/${queuePrefix}/`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    })
}

export default queueService;