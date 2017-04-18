import React from 'react';
import {connect} from 'react-redux'
import Divider from 'material-ui/Divider';
import Paper from 'material-ui/Paper';
import TextField from 'material-ui/TextField';
import SelectField from 'material-ui/SelectField';
import DatePicker from 'material-ui/DatePicker';
import Checkbox from 'material-ui/Checkbox';
import MenuItem from 'material-ui/MenuItem';
import {Grid, Row, Col} from 'react-flexbox-grid';

import {setParameter} from '../actions';

const styles = {
    checkbox: {
        paddingTop: 15,
        paddingBottom: 15,
    }
};

const menuItems = (selectedVelues, validValues) => {
    return validValues.map((name) => (
        <MenuItem
            key={name.value}
            insetChildren={true}
            checked={selectedVelues && selectedVelues.includes(name.value)}
            value={name.value}
            primaryText={name.label}
        />
    ));
};

const makeField = (props, parameter) => {
    if (!parameter.promptUser) {
        return <div></div>
    }

    switch (parameter.type) {
        case 0:
            return <Checkbox
                label={parameter.prompt}
                style={styles.checkbox}
                value={props.data[parameter.name][0] === 'True'}
                onCheck={(ee, value) => props.dispatch(setParameter(parameter.name, [value ? 'True' : 'False']))}
            />;

        case 1:
            let currentDate = null;
            if(props.data[parameter.name][0]){
                currentDate = new Date(props.data[parameter.name][0]);
            }

            return <DatePicker underlineShow={false}
                               hintText={parameter.prompt}
                               onChange={(e, value) => props.dispatch(setParameter(parameter.name, [value]))}
                               value={currentDate}/>;
        case 2:
        case 3:
        case 4:
            if (parameter.multiValue) {
                return <SelectField
                    multiple={true}
                    underlineShow={false}
                    floatingLabelText={parameter.prompt}
                    floatingLabelFixed={true}
                    hintText={parameter.prompt}
                    value={props.data[parameter.name]}
                    onChange={(a, b, values) => props.dispatch(setParameter(parameter.name, values))}
                >
                    {menuItems(props.data[parameter.name], parameter.validValues)}
                </SelectField>
            }

            return <TextField underlineShow={false}
                              hintText={parameter.prompt}
                              value={props.data[parameter.name]}
            />;
        default:
            return <TextField underlineShow={false}
                              hintText={parameter.prompt}
                              value={props.data[parameter.name]}
            />
    }
};

const Filter = (props) => {
    if (!props.report) {
        return <div></div>
    }

    const rowCols = [];
    props.report.parameters.forEach((item, index) => {
        if (index % 3 === 0) {
            rowCols.push(<Col xs={12}> <Divider /> </Col>);
        }

        rowCols.push(<Col key={index} xs={4}>{makeField(props, item)}</Col>);
    });

    rowCols.push(<Col key={props.report.parameters.length + 1} xs={12}> <Divider /> </Col>);

    return <div>
        <Paper zDepth={2} style={{paddingBottom: 15}}>
            <Grid fluid>
                <Row>
                    {rowCols}
                </Row>
            </Grid>
        </Paper>
    </div>
};

function stateToProps(state) {
    return state;
}

export default connect(stateToProps)(Filter);