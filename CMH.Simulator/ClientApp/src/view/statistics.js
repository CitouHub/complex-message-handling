import React, { useState, useEffect, useCallback } from 'react';
import LoadingButton from '@mui/lab/LoadingButton';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
import MessageStatisticsTable from '../component/table/messagestatistics.table';
import RuntimeStatisticsTable from '../component/table/runtimestatistics.table';

import StatisticsService from '../service/statistics.service';

const Statistics = () => {
    const [loading, setLoading] = useState(true);
    const [resetting, setResetting] = useState(false);
    const [priorityStatistics, setPriorityStatistics] = useState([]);
    const [processStatistics, setProcessStatistics] = useState([]);
    const [runtimeStatistics, setRuntimeStatistics] = useState({});

    const refreshStatistics = useCallback(() => {
        setTimeout(() => {
            updateStatistics();
            refreshStatistics();
        }, 1000);
    });

    useEffect(() => {
        updateStatistics();
        refreshStatistics();
    }, [refreshStatistics]);

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

    const reset = () => {
        setResetting(true);
        var resetPriorityStatistics = StatisticsService.resetPriorityStatistics();
        var resetProcessStatistics = StatisticsService.resetProcessStatistics();
        var resetRuntimeStatistics = StatisticsService.resetRuntimeStatistics();

        Promise.all([resetPriorityStatistics, resetProcessStatistics, resetRuntimeStatistics]).then(() => {
            setPriorityStatistics([]);
            setProcessStatistics([]);
            setRuntimeStatistics([]);
            updateStatistics();
            setResetting(false);
        });
    }

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <React.Fragment>
                    <MessageStatisticsTable title='Priority statistics' category='Priority' messageStatistics={priorityStatistics} />
                    <MessageStatisticsTable title='Process statistics' category='Process' messageStatistics={processStatistics} />
                    <RuntimeStatisticsTable runtimeStatistics={runtimeStatistics} />
                    <LoadingButton
                        loading={resetting}
                        variant="contained"
                        loadingPosition="start"
                        startIcon={<RestartAltIcon />}
                        onClick={reset}>
                        {resetting === false ? 'Reset' : 'Resetting...'}
                    </LoadingButton>
                </React.Fragment>
            }
        </div>
    );
}

export default Statistics;
