import React, { useState } from 'react';
import TextField from '@mui/material/TextField';
import LoadingButton from '@mui/lab/LoadingButton';
import SaveIcon from '@mui/icons-material/Save';
import NotificationMessage from '../notification/snackbar.message';

import ProcessChannelPolicyService from '../../service/processchannelpolicy.service';

const ProcessChannelPolicyForm = ({ data }) => {
    const [processChannelPolicy, setProcessChannelPolicy] = useState(data);
    const [saving, setSaving] = useState(false);
    const [notification, setNotification] = useState('');

    const updateProcessChannelPolicy = () => {
        setSaving(true);
        ProcessChannelPolicyService.updateProcessChannelPolicy(processChannelPolicy.name, processChannelPolicy).then(() => {
            setSaving(false);
            setNotification('Process channel policy updated')
        });
    }

    return (
        <div className="d-flex mb-4">
            <h2 style={{ width: '8rem' }}>{processChannelPolicy.name}:</h2>
            <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                id="pcp-tries"
                variant="outlined"
                label="Tries"
                type="number"
                value={processChannelPolicy.tries}
                onChange={e => setProcessChannelPolicy({ ...processChannelPolicy, tries: e.target.value })}
            />
            <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                id="pcp-initial-sleep-time"
                variant="outlined"
                label="Initial sleep time"
                type="number"
                value={processChannelPolicy.initialSleepTime}
                onChange={e => setProcessChannelPolicy({ ...processChannelPolicy, initialSleepTime: e.target.value })}
            />
            <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                id="pcp-backoff-factor"
                variant="outlined"
                label="Backoff factor"
                type="number"
                value={processChannelPolicy.backoffFactor}
                onChange={e => setProcessChannelPolicy({ ...processChannelPolicy, backoffFactor: e.target.value })}
            />
            <LoadingButton
                loading={saving}
                variant="outlined"
                loadingPosition="start"
                startIcon={<SaveIcon />}
                onClick={updateProcessChannelPolicy}>Save
            </LoadingButton>
            <NotificationMessage
                open={notification !== ''}
                close={() => setNotification('')}
                text={notification}
                severity='info' />
        </div>
    );
}

export default ProcessChannelPolicyForm;
