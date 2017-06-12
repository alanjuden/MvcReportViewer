import React from 'react';
import {connect} from 'react-redux'

const Content = (props) => {
    if (!props.report) return <div></div>;

    return <div style={{minHeight: '100%'}}>
        <div dangerouslySetInnerHTML={{__html: props.report.content}}/>
    </div>
};

function stateToProps(state) {
    return state;
}

export default connect(stateToProps)(Content);