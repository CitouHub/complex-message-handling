import React, { useState, useEffect } from 'react';
import TextField from '@mui/material/TextField';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import LoadingButton from '@mui/lab/LoadingButton';
import SendIcon from '@mui/icons-material/Send';
import NotificationMessage from '../component/notification/snackbar.message';

import QueueService from '../service/queue.service';
import TestService from '../service/test.service';
import DataSourceService from '../service/datasource.service';

const SendMessages = () => {
    const [loading, setLoading] = useState(true);
    const [startingTest, setStartingTest] = useState(false);
    const [notification, setNotification] = useState('');
    const [nbrOfMessages, setNbrOfMessages] = useState(0);
    const [dataSources, setDataSources] = useState([]);
    const [dataSource, setDataSource] = useState({});
    const [priorityQueueNames, setPriorityQueueNames] = useState([])
    const [priorityQueueName, setPriorityQueueName] = useState('');

    useEffect(() => {
        var getDataSources = DataSourceService.getDataSources();
        var getQueueNames = QueueService.getQueueNames('priority');

        Promise.all([getDataSources, getQueueNames]).then((result) => {
            setDataSources(result[0]);
            setPriorityQueueNames(result[1]);
            setLoading(false);
        });
    }, []);

    const startTest = () => {
        setStartingTest(true);
        TestService.startTest(nbrOfMessages, priorityQueueName, dataSource.id).then(() => {
            setStartingTest(false);
            setNotification('Test started!')
        });
    }

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <React.Fragment>
                    <div>
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="nbr-or-messages"
                            variant="outlined"
                            label="Nbr. of messages"
                            type="number"
                            value={nbrOfMessages}
                            onChange={e => setNbrOfMessages(e.target.value)}
                        />
                        <FormControl sx={{ width: '300px', paddingRight: '1rem' }}>
                            <InputLabel id="messages-data-source">Data source</InputLabel>
                            <Select
                                labelId="messages-data-source"
                                id="select-messages-data-source"
                                value={dataSource.description}
                                label="Data source"
                                onChange={e => setDataSource(e.target.value)}
                            >
                                {dataSources.map((dataSource) => (
                                    <MenuItem key={dataSource.id} value={dataSource}> {dataSource.description} </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                        <FormControl sx={{ width: '300px', paddingRight: '1rem' }}>
                            <InputLabel id="messages-data-source">Priority queue</InputLabel>
                            <Select
                                labelId="messages-pritority"
                                id="select-messages-priority"
                                value={priorityQueueName}
                                label="Priority queue"
                                onChange={e => setPriorityQueueName(e.target.value)}
                            >
                                {priorityQueueNames.map((priorityQueueName) => (
                                    <MenuItem key={priorityQueueName} value={priorityQueueName}> {priorityQueueName} </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                        <LoadingButton
                            loading={startingTest}
                            variant="contained"
                            loadingPosition="start"
                            startIcon={<SendIcon />}
                            onClick={startTest}>
                            {startingTest === false ? 'Start test' : 'Starting test...'}
                        </LoadingButton>
                    </div>
                    <NotificationMessage
                        open={notification !== ''}
                        close={() => setNotification('')}
                        text={notification}
                        severity='info' />
                </React.Fragment>
            }
        </div>
    );
}

export default SendMessages;
