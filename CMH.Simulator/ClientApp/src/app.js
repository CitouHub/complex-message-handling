import React, { useState, useEffect } from 'react';
import { Container } from "reactstrap";
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Box from '@mui/material/Box';
import SendMessages from './view/sendmessages';
import DataSources from './view/datasources';
import ProcessChannelPolicies from './view/processchannelpolicies';

import Config from './util/config';
import AppSettingsService from './service/appsettings.service';

import './custom.css';

const App = () => {
    const [loading, setLoading] = useState(true);
    const [value, setValue] = useState(0);

    useEffect(() => {
        AppSettingsService.get().then((value) => {
            Config.setApplicationSettings(value);
            setLoading(false);
        });
    }, []);

    const handleChange = (event, newValue) => {
        setValue(newValue);
    };

    function TabPanel(props) {
        const { children, value, index, ...other } = props;

        return (
            <div
                role="tabpanel"
                hidden={value !== index}
                id={`simple-tabpanel-${index}`}
                aria-labelledby={`simple-tab-${index}`}
                {...other}
            >
                {value === index && (
                    <div>
                        {children}
                    </div>
                )}
            </div>
        );
    }

    function getProps(index) {
        return {
            'id': `simple-tab-${index}`,
            'aria-controls': `simple-tabpanel-${index}`,
        };
    }

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <Container>
                    <h1>CMH Simulator</h1>
                    <Box sx={{ width: '100%' }}>
                        <Box sx={{ borderBottom: 1, borderColor: 'divider', marginBottom: '2rem' }}>
                            <Tabs value={value} onChange={handleChange}>
                                <Tab label="Send messages" {...getProps(0)} />
                                <Tab label="Data sources" {...getProps(1)} />
                                <Tab label="Process channels" {...getProps(2)} />
                                <Tab label="Settings" {...getProps(3)} />
                                <Tab label="Statistics" {...getProps(4)} />
                            </Tabs>
                        </Box>
                        <TabPanel value={value} index={0}>
                            <SendMessages />
                        </TabPanel>
                        <TabPanel value={value} index={1}>
                            <DataSources />
                        </TabPanel>
                        <TabPanel value={value} index={2}>
                            <ProcessChannelPolicies />
                        </TabPanel>
                        <TabPanel value={value} index={3}>
                            Item Four
                        </TabPanel>
                        <TabPanel value={value} index={4}>
                            Item Five
                        </TabPanel>
                    </Box>
                </Container>
            }
        </div>
    );
}

export default App;
