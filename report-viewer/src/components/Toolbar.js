import React from 'react';
import {connect} from 'react-redux'
import AppBar from 'material-ui/AppBar';
import IconButton from 'material-ui/IconButton';
import IconMenu from 'material-ui/IconMenu';
import MenuItem from 'material-ui/MenuItem';
import FileDownload from 'material-ui/svg-icons/file/file-download';
import RefresheIcon from 'material-ui/svg-icons/navigation/refresh';
import RaisedButton from 'material-ui/RaisedButton';
import First from 'material-ui/svg-icons/av/skip-previous'
import Last from 'material-ui/svg-icons/av/skip-next'
import Left from 'material-ui/svg-icons/hardware/keyboard-arrow-left'
import Right from 'material-ui/svg-icons/hardware/keyboard-arrow-right'
import {setPage} from "../actions/index";
import FileSaver from 'file-saver';

const downaloadFormats = [
    'DOCX',
    'XLSX',
    'PDF',
    'CSV',
    'MHTML',
    'TIFF',
    'XML'
];

const makeNavigate = (type, apiUrl, reportPath, data) => {
    return () => {
        fetch(`${apiUrl}/ExportReport/?reportPath=${reportPath}&format=${type}`, {
            credentials: 'same-origin',
            method: 'post',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data)
        }).then(function(response) {
            console.log(response);
            return response.blob();
        }).then(function(blob) {
            FileSaver.saveAs(blob, `${reportPath.replace('/', '')}.${type}`);
        })
    }
};

const NavigationBar = props => {
    if (!props.report) return <div />;

    const items = [];

    for (let i = props.report.currentPage; i < props.report.totalPages; i++) {
        items.push(<MenuItem />)
    }

    let pageCount = props.report.totalPages === 0 ? 1 : props.report.totalPages;
    for (let i = props.report.currentPage; i <= pageCount; i++) {
        items.push(<MenuItem value={i} key={i} primaryText={`Page ${i}`}
                             onClick={() => props.dispatch(setPage(i))}/>);
    }

    return <div>
        <RaisedButton icon={<First/>} onClick={() => props.dispatch(setPage(1)) }/>
        <RaisedButton icon={<Left />} onClick={() => props.dispatch(setPage(props.page - 1)) }/>
        <RaisedButton
            {...props}
            label={`Page ${props.page}`}
        >
        </RaisedButton>
        <RaisedButton icon={<Right />} onClick={() => props.dispatch(setPage(props.page + 1)) }/>
        <RaisedButton icon={<Last />} onClick={() => props.dispatch(setPage(props.report.totalPages)) }/>
    </div>
}

const DownloadMenu = (props) => {
    return <IconMenu
        {...props}
        iconButtonElement={
            <IconButton><FileDownload /></IconButton>
        }
        targetOrigin={{horizontal: 'right', vertical: 'top'}}
        anchorOrigin={{horizontal: 'right', vertical: 'top'}}
    >
        {downaloadFormats.map((item, index) => <MenuItem key={item} primaryText={item}
                                                         onClick={makeNavigate(item, props.options.apiUrl, props.options.reportPath, props.data)}/>)}
    </IconMenu>
}

const Toolbar = (props) => {
    if (!props.options) return <div/>;

    return <div>
        <AppBar
            iconElementLeft={<IconButton><RefresheIcon /></IconButton>}
            iconElementRight={<DownloadMenu {...props} label="Save"/>}
        />
        <div style={{padding: 15}}>
            <NavigationBar {...props}/>
        </div>
    </div>
}

function stateToProps(state) {
    return state;
}

export default connect(stateToProps)(Toolbar);