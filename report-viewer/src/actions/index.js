export function endLoad(payload) {
    return {type: 'END_LOAD', payload}
}

export function initOptions(apiUrl, reportPath) {
    return {type: 'INIT_OPTIONS', payload: {apiUrl, reportPath}}
}

export function startLoad() {
    return {type: 'START_LOAD'}
}

export function setPage(pageNumber) {
    return async function (dispatch) {
        dispatch({type: 'SET_PAGE', payload: pageNumber});
        dispatch(viewReport())
    }
}

export function setParameter(name, values) {
    console.log(name, values);
    return {type: 'SET_PARAMETER', payload: {name, values}};
}

async function executeReport(getState, body) {
    const response = await fetch(
        `${getState().options.apiUrl}?reportPath=${getState().options.reportPath}&page=${getState().page}`, {
            method: 'POST',
            mode: 'cors',
            headers: {
                "Content-type": "application/json"
            },
            body: JSON.stringify(body)
        });

    return await response.json();
}

export function loadReport(apiUrl, reportPath, body = {}) {
    return async function (dispatch, getState) {
        dispatch(initOptions(apiUrl, reportPath));
        dispatch(startLoad());
        dispatch(endLoad(await executeReport(getState, body)))
    }
}

export function viewReport() {
    return async function (dispatch, getState) {
        dispatch(startLoad());
        let body = getState().data;
        dispatch(endLoad(await executeReport(getState, body)))
    }
}