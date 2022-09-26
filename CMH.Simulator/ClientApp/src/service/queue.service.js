import Request from "../util/requesthandler"

const queueService = {
    getQueueNames: async (queuePrefix) => await Request.send({
        url: `/queue/${queuePrefix}/`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    resetQueues: async () => await Request.send({
        url: `/queue/reset/`,
        method: 'POST'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    sendMessages: async (nbrOfMessages, queueName, dataSourceId) => {
        let params = [
            queueName === undefined || queueName === '' ? `` : `queueName=${queueName}`,
            dataSourceId === undefined ? `` : `dataSourceId=${dataSourceId}`
        ]
        let query = params.filter(_ => _ !== '').length === 0 ? `` : `?${params.filter(_ => _ !== '').join('&')}`;

        await Request.send({
            url: `/queue/send/${nbrOfMessages}${query}`,
            method: 'POST'
        }).then((response) => {
            return Request.handleResponse(response)
        })
    }
}

export default queueService;