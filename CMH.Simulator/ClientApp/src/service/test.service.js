import Request from "../util/requesthandler"

const testService = {
    startTest: async (nbrOfMessages, queueName, dataSourceId) => {
        let params = [
            queueName === undefined || queueName === '' ? `` : `queueName=${queueName}`,
            dataSourceId === undefined ? `` : `dataSourceId=${dataSourceId}`
        ]
        let query = params.filter(_ => _ !== '').length === 0 ? `` : `?${params.filter(_ => _ !== '').join('&')}`;

        await Request.send({
            url: `/test/start/${nbrOfMessages}${query}`,
            method: 'POST'
        }).then((response) => {
            return Request.handleResponse(response)
        })
    }
}

export default testService;