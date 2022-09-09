import React, { useState, useEffect } from 'react';
import AddIcon from '@mui/icons-material/Add';
import LoadingButton from '@mui/lab/LoadingButton';
import DataSourceForm from '../component/form/datasource.form';
import NotificationMessage from '../component/notification/snackbar.message';

import DataSourceService from '../service/datasource.service';
import ProcessChannelPolicyService from '../service/processchannelpolicy.service';

const DataSources = () => {
    const [loading, setLoading] = useState(true);
    const [dataSources, setDataSources] = useState([]);
    const [processChannelPolicies, setProcessChannelPolicies] = useState([]);
    const [notification, setNotification] = useState('');

    useEffect(() => {
        var getDataSources = DataSourceService.getDataSources();
        var getProcessChannelPolicies = ProcessChannelPolicyService.getProcessChannelPolicies();

        Promise.all([getDataSources, getProcessChannelPolicies]).then((result) => {
            setDataSources(result[0]);
            setProcessChannelPolicies(result[1]);
            setLoading(false);
        });
    }, []);

    const deleteDataSource = (dataSourceId) => {
        DataSourceService.deleteDataSource(dataSourceId).then(() => {
            let updatedDataSources = [...dataSources];
            let index = updatedDataSources.findIndex(_ => _.id === dataSourceId);
            updatedDataSources.splice(index, 1);
            setDataSources(updatedDataSources);
            setNotification('Data source removed');
        });
    }

    const addDataSource = () => {
        DataSourceService.newDataSource().then((result) => {
            setDataSources([...dataSources, result]);
            setNotification('Data source added');
        });
    }

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <React.Fragment>
                    <div>
                        {dataSources.sort((a, b) => a.id > b.id).map((dataSource) => (
                            <DataSourceForm
                                key={dataSource.id}
                                data={dataSource}
                                processChannelPolicies={processChannelPolicies}
                                deleteDataSource={deleteDataSource} >
                            </DataSourceForm>
                        ))}
                    </div>
                    <LoadingButton
                        loading={false}
                        variant="contained"
                        loadingPosition="start"
                        startIcon={<AddIcon />}
                        onClick={addDataSource}>Add new data source
                    </LoadingButton>
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

export default DataSources;
