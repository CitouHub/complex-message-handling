import React, { useState, useEffect } from 'react';
import TextField from '@mui/material/TextField';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import LoadingButton from '@mui/lab/LoadingButton';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
import SaveIcon from '@mui/icons-material/Save';
import NotificationMessage from '../component/notification/snackbar.message';

import SettingsService from '../service/settings.service';
import ProcessChannelPolicyService from '../service/processchannelpolicy.service';

const Config = () => {
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [resetting, setResetting] = useState(false);
    const [settings, setSettings] = useState({});
    const [processChannelPolicies, setProcessChannelPolicies] = useState([]);
    const [notification, setNotification] = useState('');

    useEffect(() => {
        var getSettings = SettingsService.getSettings();
        var getProcessChannelPolicies = ProcessChannelPolicyService.getProcessChannelPolicies();

        Promise.all([getSettings, getProcessChannelPolicies]).then((result) => {
            setSettings(result[0]);
            setProcessChannelPolicies(result[1]);
            setLoading(false);
        });
    }, []);

    const updateSettings = () => {
        setSaving(true);
        SettingsService.updateSettings(settings).then(() => {
            setSaving(false);
            setNotification('Settings updated!')
        });
    }

    const resetSettings = () => {
        setResetting(true);
        SettingsService.resetSettings().then(() => {
            SettingsService.getSettings().then((result) => {
                setSettings(result);
                setResetting(false);
                setNotification('Settings reset!')
            });
        });
    }

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <React.Fragment>
                    <h2>Queue cache</h2>
                    <div className="d-flex mb-4">
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-qc-refresh-interval"
                            variant="outlined"
                            label="Refresh interval"
                            type="number"
                            value={settings.queueCache.refreshInterval}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.queueCache.refreshInterval = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                    </div>
                    <h2>Priority</h2>
                    <div className="d-flex mb-4">
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-p-tasks"
                            variant="outlined"
                            label="Tasks"
                            type="number"
                            value={settings.priority.tasks}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.priority.tasks = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-p-message-batch"
                            variant="outlined"
                            label="Message batch"
                            type="number"
                            value={settings.priority.messageBatch}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.priority.messageBatch = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-p-message-fetch-timeout"
                            variant="outlined"
                            label="Message fetch timeout (ms)"
                            type="number"
                            value={settings.priority.messageFetchTimeOut}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.priority.messageFetchTimeOut = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <FormControl sx={{ width: '200px', paddingRight: '1rem' }}>
                            <InputLabel id="c-p-default-process-channel">Def. process channel</InputLabel>
                            <Select
                                labelId="c-p-default-process-channel"
                                id="select-c-p-default-process-channel"
                                value={settings.priority.defaultProcessChannel}
                                label="Default process channel"
                                onChange={e => {
                                    let updatedSettings = JSON.parse(JSON.stringify(settings));
                                    updatedSettings.priority.defaultProcessChannel = e.target.value;
                                    setSettings(updatedSettings);
                                }}
                            >
                                {processChannelPolicies.map((processChannelPolicy) => (
                                    <MenuItem key={processChannelPolicy.name} value={processChannelPolicy.name}> {processChannelPolicy.name} </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </div>
                    <h2>Backoff policy: Empty iteration</h2>
                    <div className="d-flex mb-4">
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-ei-initial-sleep-time"
                            variant="outlined"
                            label="Initial sleep time"
                            type="number"
                            value={settings.backoffPolicy.emptyIteration.initialSleepTime}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.emptyIteration.initialSleepTime = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-ei-max-sleep-time"
                            variant="outlined"
                            label="Max sleep time"
                            type="number"
                            value={settings.backoffPolicy.emptyIteration.maxSleepTime}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.emptyIteration.maxSleepTime = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-ei-backoff-factor"
                            variant="outlined"
                            label="Backoff factor"
                            type="number"
                            value={settings.backoffPolicy.emptyIteration.backoffFactor}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.emptyIteration.backoffFactor = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                    </div>
                    <h2>Backoff policy: Process channel full</h2>
                    <div className="d-flex mb-4">
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-pcf-max-zise"
                            variant="outlined"
                            label="Max size"
                            type="number"
                            value={settings.backoffPolicy.processChannelFull.maxSize}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.processChannelFull.maxSize = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-pcf-priority-step-size"
                            variant="outlined"
                            label="Priority step size"
                            type="number"
                            value={settings.backoffPolicy.processChannelFull.priorityStepSize}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.processChannelFull.priorityStepSize = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-pcf-initial-sleep-time"
                            variant="outlined"
                            label="Initial sleep time"
                            type="number"
                            value={settings.backoffPolicy.processChannelFull.initialSleepTime}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.processChannelFull.initialSleepTime = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-pcf-priority-factor"
                            variant="outlined"
                            label="Priority factor"
                            type="number"
                            value={settings.backoffPolicy.processChannelFull.priorityFactor}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.processChannelFull.priorityFactor = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-pcf-try-factor"
                            variant="outlined"
                            label="Try factor"
                            type="number"
                            value={settings.backoffPolicy.processChannelFull.tryFactor}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.processChannelFull.tryFactor = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                        <TextField sx={{ width: '200px', paddingRight: '1rem' }}
                            id="c-bp-pcf-max-sleep-time"
                            variant="outlined"
                            label="Max sleep time"
                            type="number"
                            value={settings.backoffPolicy.processChannelFull.maxSleepTime}
                            onChange={e => {
                                let updatedSettings = JSON.parse(JSON.stringify(settings));
                                updatedSettings.backoffPolicy.processChannelFull.maxSleepTime = e.target.value;
                                setSettings(updatedSettings);
                            }}
                        />
                    </div>

                    <div className="d-flex">
                        <LoadingButton
                            loading={saving}
                            variant="contained"
                            loadingPosition="start"
                            startIcon={<SaveIcon />}
                            onClick={updateSettings}>Save
                        </LoadingButton>
                        <LoadingButton
                            loading={resetting}
                            variant="contained"
                            loadingPosition="start"
                            startIcon={<RestartAltIcon />}
                            onClick={resetSettings}>Reset
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

export default Config;
