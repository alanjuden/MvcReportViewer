import React from 'react';
import RaisedButton from 'material-ui/RaisedButton';
import {connect} from 'react-redux'

import Toolbar from './components/Toolbar'
import Content from './components/Content';
import Filter from './components/Filter';
import RefreshIndicator from 'material-ui/RefreshIndicator';

import {viewReport} from './actions'
import './App.css';

const style = {
    container: {
        position: 'relative',
    },
    refresh: {
        display: 'inline-block',
        position: 'relative',
        marginLeft: '50%'
    },
};

const App = (props) => {
    if (props.isLoad) {
        return <div style={style.container}>
            <RefreshIndicator
                size={40}
                left={-20}
                top={120}
                status="loading"
                style={style.refresh}
            />
        </div>
    }

    return (
        <div className="App">
            <Filter/>
            <Toolbar/>
            <RaisedButton label="View Report"
                          primary={true}
                          onClick={() => props.dispatch(viewReport())}
                          style={{marginTop: 10}}/>
            <Content/>
        </div>
    );
};

export default connect(state => state)(App);
