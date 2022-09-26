import Request from "../util/requesthandler"

const dataSourceService = {
    getDataSources: async () => await Request.send({
        url: `/datasource`,
        method: 'GET'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    updateDataSource: async (id, dataSource) => await Request.send({
        url: `/datasource/${id}`,
        method: 'PUT',
        data: dataSource
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    newDataSource: async () => await Request.send({
        url: `/datasource`,
        method: 'POST',
        data: {
            failRate: 0,
            minProcessTime: 100,
            maxProcessTime: 1000,
            processChannel: 'Default'
        }
    }).then((response) => {
        return Request.handleResponse(response)
    }),
    deleteDataSource: async (id) => await Request.send({
        url: `/datasource/${id}`,
        method: 'DELETE'
    }).then((response) => {
        return Request.handleResponse(response)
    }),
}

export default dataSourceService;