import React, { useState, useEffect } from 'react';
import TextField from '@mui/material/TextField';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';

import QueueService from '../service/queue.service';
import DataSourceService from '../service/datasource.service';

const SendMessages = () => {
    const [loading, setLoading] = useState(true);
    const [messages, setMessages] = useState(0);
    const [dataSources, setDataSources] = useState([]);
    const [dataSource, setDataSource] = useState({});
    const [priorityQueues, setPriorityQueues] = useState([])
    const [priorityQueue, setPriorityQueue] = useState({});

    useEffect(() => {
        var getDataSources = DataSourceService.getDataSources();
        var getQueues = QueueService.getQueues('priority');

        Promise.all([getDataSources, getQueues]).then((result) => {
            setDataSources(result[0]);
            setPriorityQueues(result[1]);
            setLoading(false);
        });
    }, []);

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <div>
                    <TextField
                        id="nbr-or-messages"
                        variant="outlined"
                        label="Nbr. of messages"
                        type="number"
                        value={messages}
                        onChange={e => setMessages(e.target.value)}
                    />
                    <FormControl sx={{ width: '200px' }}>
                        <InputLabel id="messages-data-source">Data source</InputLabel>
                        <Select
                            labelId="messages-data-source"
                            id="select-messages-data-source"
                            value={dataSource}
                            label="Data source"
                            onChange={e => setDataSource(e.target.value)}
                        >
                            {dataSources.map((dataSource) => (
                                <MenuItem key={dataSource.id} value={dataSource.id}> {dataSource.description} </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <FormControl sx={{ width: '200px' }}>
                        <InputLabel id="messages-data-source">Priority queue</InputLabel>
                        <Select
                            labelId="messages-pritority"
                            id="select-messages-priority"
                            value={priorityQueue}
                            label="Priority queue"
                            onChange={e => setPriorityQueue(e.target.value)}
                        >
                            {priorityQueues.map((priorityQueue) => (
                                <MenuItem key={priorityQueue} value={priorityQueue}> {priorityQueue} </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
            }
        </div>
    );
}

export default SendMessages;
