import React from 'react';

const MessageStatisticsTable = ({ title, category, messageStatistics }) => {
    return (
        <div>
            <h2>{title}</h2>
            <table>
                <thead>
                    <tr>
                        <td>{category}</td>
                        <td>Tot. msg. handled</td>
                        <td>Tot. msg. rescheduled</td>
                        <td>Tot. msg. discarded</td>
                        <td>Avg. msg. handle duration (ms)</td>
                        <td>Reschedule rate</td>
                    </tr>
                </thead>
                <tbody>
                    {Object.keys(messageStatistics).map((categoryId) => (
                        <tr key={categoryId}>
                            <td>{categoryId}</td>
                            <td>{messageStatistics[categoryId].totalMessagesHandled}</td>
                            <td>{messageStatistics[categoryId].totalMessagesRescheduled}</td>
                            <td>{messageStatistics[categoryId].totalMessagesDiscarded}</td>
                            <td>{messageStatistics[categoryId].avgMessageHandleDuration}</td>
                            <td>{messageStatistics[categoryId].rescheduleRate} %</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default MessageStatisticsTable;