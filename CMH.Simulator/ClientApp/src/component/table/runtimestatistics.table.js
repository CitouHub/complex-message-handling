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
            </table>
        </div>
    );
}

export default RuntimeStatisticsTable;