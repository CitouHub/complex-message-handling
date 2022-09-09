import React, { useState, useEffect } from 'react';
import ProcessChannelPolicyForm from '../component/form/processchannelpolicy.form';

import ProcessChannelPolicyService from '../service/processchannelpolicy.service';

const ProcessChannelPloicies = () => {
    const [loading, setLoading] = useState(true);
    const [processChannelPolicies, setProcessChannelPolicies] = useState([]);

    useEffect(() => {
        var getProcessChannelPolicies = ProcessChannelPolicyService.getProcessChannelPolicies();

        Promise.all([getProcessChannelPolicies]).then((result) => {
            setProcessChannelPolicies(result[0]);
            setLoading(false);
        });
    }, []);

    return (
        <div>
            {loading && <p>Loading...</p>}
            {!loading &&
                <React.Fragment>
                    <div>
                        {processChannelPolicies.sort((a, b) => a.id > b.id).map((processChannelPolicy) => (
                            <ProcessChannelPolicyForm
                                key={processChannelPolicy.name}
                                data={processChannelPolicy} >
                            </ProcessChannelPolicyForm>
                        ))}
                    </div>
                </React.Fragment>
            }
        </div>
    );
}

export default ProcessChannelPloicies;
