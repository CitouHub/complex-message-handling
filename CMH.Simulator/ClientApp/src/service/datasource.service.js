import Request from "../util/requesthandler"

export default {
    getDataSources: async () => await Request.send({
        url: `/datasource`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    })
}