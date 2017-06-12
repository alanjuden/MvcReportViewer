import React from 'react';
import ReactDOM from 'react-dom';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import {createStore, applyMiddleware } from 'redux'
import { Provider } from 'react-redux'
import thunk from 'redux-thunk';
import injectTapEventPlugin from 'react-tap-event-plugin';

import App from './App';
import './index.css';
import reducer from './reducers';
import {loadReport} from './actions'

let store = createStore(
    reducer,
    applyMiddleware(thunk)
);

// Needed for onTouchTap
// http://stackoverflow.com/a/34015469/988941
injectTapEventPlugin();

const targetElem = document.getElementById('root');
ReactDOM.render(
    <Provider store={store}>
        <MuiThemeProvider>
            <App {...targetElem.dataset}/>
        </MuiThemeProvider>
    </Provider>,
    targetElem
);

console.log(JSON.parse(targetElem.dataset.params));
store.dispatch(loadReport(targetElem.dataset.reportApiUrl, targetElem.dataset.reportPath, JSON.parse(targetElem.dataset.params)));

