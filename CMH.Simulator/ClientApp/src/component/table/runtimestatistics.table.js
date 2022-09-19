import React from 'react';

const RuntimeStatisticsTable = ({ runtimeStatistics }) => {
    return (
        <div>
            <h2>Runtime statistics</h2>
            <h4>Overview</h4>
            <table>
                <thead>
                    <tr>
                        <td>Priority queue queries</td>
                        <td>Tot. msg. fetched</td>
                        <td>Avg. msg. per query</td>
                        <td>Avg. msg. fetch duration (ms)</td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{runtimeStatistics.priorityQueueQueries}</td>
                        <td>{runtimeStatistics.totalMessagesFetched}</td>
                        <td>{runtimeStatistics.avgMessagesPerQuery}</td>
                        <td>{runtimeStatistics.avgMessagesFetchDuration}</td>
                    </tr>
                </tbody>
                <thead>
                    <tr>
                        <td>Tot. msg. processed</td>
                        <td>Avg. msg. through put (msg/s)</td>
                        <td>Max parallell task</td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{runtimeStatistics.totalMessagesProcessed}</td>
                        <td>{runtimeStatistics.avgThroughPut}</td>
                        <td>{runtimeStatistics.maxParallellTasks}</td>
                    </tr>
                </tbody>
            </table>
            <h4>Costs</h4>
            <table>
                <thead>
                    <tr>
                        <td>Approx. exec. time</td>
                        <td>Approx. tot. exec.</td>
                        <td>Tot. cost</td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>$ {runtimeStatistics.approxExecutionTimeCost}</td>
                        <td>$ {runtimeStatistics.approxTotalExecutionCost}</td>
                        <td>$ {runtimeStatistics.approxExecutionTimeCost + runtimeStatistics.approxTotalExecutionCost}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
}

export default RuntimeStatisticsTable;