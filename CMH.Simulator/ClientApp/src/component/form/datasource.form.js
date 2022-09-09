import React, { useState } from 'react';
import TextField from '@mui/material/TextField';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import LoadingButton from '@mui/lab/LoadingButton';
import SaveIcon from '@mui/icons-material/Save';
import DeleteIcon from '@mui/icons-material/Delete';
import NotificationMessage from '../notification/snackbar.message';

import DataSourceService from '../../service/datasource.service';

const DataSourceForm = ({ data, processChannelPolicies, deleteDataSource }) => {
    const [dataSource, setDataSource] = useState(data);
    const [saving, setSaving] = useState(false);
    const [notification, setNotification] = useState('');

    const updateDataSource = () => {
        setSaving(true);
        DataSourceService.updateDataSource(dataSource.id, dataSource).then(() => {
            setSaving(false);
            setNotification('Data source updated')
        });
    }

    return (
        <div className="d-flex mb-4">
            <h2 style={{ width: '3rem' }}>{dataSource.id}:</h2>
            <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                id="ds-fail-rate"
                variant="outlined"
                label="Fail rate"
                type="number"
                value={dataSource.failRate}
                onChange={e => setDataSource({ ...dataSource, failRate: e.target.value })}
            />
            <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                id="ds-min-process-time"
                variant="outlined"
                label="Min. process time (ms)"
                type="number"
                value={dataSource.minProcessTime}
                onChange={e => setDataSource({ ...dataSource, minProcessTime: e.target.value })}
            />
            <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                id="ds-max-process-time"
                variant="outlined"
                label="Max. process time (ms)"
                type="number"
                value={dataSource.maxProcessTime}
                onChange={e => setDataSource({ ...dataSource, maxProcessTime: e.target.value })}
            />
            <FormControl sx={{ width: '300px', paddingRight: '1rem' }}>
                <InputLabel id="ds-process-channel">Process channel</InputLabel>
                <Select
                    labelId="ds-process-channel"
                    id="select-ds-process-channel"
                    value={dataSource.processChannel}
                    label="Process channel"
                    onChange={e => setDataSource({ ...dataSource, processChannel: e.target.value })}
                >
                    {processChannelPolicies.map((processChannelPolicy) => (
                        <MenuItem key={processChannelPolicy.name} value={processChannelPolicy.name}> {processChannelPolicy.name} </MenuItem>
                    ))}
                </Select>
            </FormControl>
            <LoadingButton
                loading={saving}
                variant="outlined"
                loadingPosition="start"
                startIcon={<SaveIcon />}
                onClick={updateDataSource}>Save
            </LoadingButton>
            <LoadingButton
                loading={false}
                variant="outlined"
                loadingPosition="start"
                startIcon={<DeleteIcon />}
                onClick={() => deleteDataSource(dataSource.id)}>Remove
            </LoadingButton>
            <NotificationMessage
                open={notification !== ''}
                close={() => setNotification('')}
                text={notification}
                severity='info' />
        </div>
    );
}

export default DataSourceForm;
