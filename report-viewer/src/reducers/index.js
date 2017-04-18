const initial = {
    page: 1,
    data: {}
};

const extracData = (parameters) => {
    let data = {};
    parameters.forEach(item => {
        data[item.name] = item.selectedValues;
    });

    return data
}

const handlers = {
    INIT_OPTIONS: (state, action) => ({...state, options: action.payload}),
    START_LOAD: (state) => ({...state, isLoad: true}),
    END_LOAD: (state, action) => ({
        ...state,
        isLoad: false,
        page: action.payload.currentPage,
        report: action.payload,
        data: extracData(action.payload.parameters)
    }),
    SET_PAGE: (state, action) => ({...state, page: action.payload}),
    SET_PARAMETER: (state, action) => ({
        ...state,
        data: {
            ...state.data,
            [action.payload.name]: action.payload.values
        }
    }),
};

export default function (state = initial, action) {
    if (handlers[action.type]) {
        return handlers[action.type](state, action);
    }

    return state;
}