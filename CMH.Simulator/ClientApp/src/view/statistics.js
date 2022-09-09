import React, { useState, useEffect } from 'react';
import NotificationMessage from '../component/notification/snackbar.message';
import MessageStatisticsTable from '../component/table/messagestatistics.table';

import StatisticsService from '../service/statistics.service';

const Statistics = () => {
    const [loading, setLoading] = useState(true);
    const [priorityStatistics, setPriorityStatistics] = useState([]);
    const [processStatistics, setProcessStatistics] = useState([]);
    const [runtimeStatistics, setRuntimeStatistics] = useState({});

    useEffect(() => {
        updateStatistics();
    }, []);

    const updateStatistics = () => {
        var getPriorityStatistics = StatisticsService.getPriorityStatistics();
        var getProcessStatistics = StatisticsService.getProcessStatistics();
        var getRuntimeStatistics = StatisticsService.getRuntimeStatistics();

        Promise.all([getPriorityStatistics, getProcessStatistics, getRuntimeStatistics]).then((result) => {
            setPriorityStatistics(result[0]);
            setProcessStatistics(result[1]);
            setRuntimeStatistics(result[2]);
            setLoading(false);
        });
    }

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <React.Fragment>
                    <MessageStatisticsTable title='Priority statistics' category='Priority' messageStatistics={priorityStatistics} />
                    <MessageStatisticsTable title='Process statistics' category='Process' messageStatistics={processStatistics} />
                    <div>
                        <h2>Runtime statistics</h2>
                    </div>
                </React.Fragment>
            }
        </div>
    );
}

export default Statistics;
