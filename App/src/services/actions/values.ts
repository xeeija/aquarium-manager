import { createAsyncAction } from 'typesafe-actions';
import { ThunkAction } from "redux-thunk";
import { RootState } from "../reducers";
import { AnyAction } from "redux";
import { IConfig } from "../rest/iconfig";
import config from "../rest/server-config";
import { ValueReturnModelSingle, ValueClient } from "../rest/data";

export const fetchValuesActions = createAsyncAction(
  'FETCH_VALUES_REQUEST',
  'FETCH_VALUES_SUCCESS',
  'FETCH_VALUES_FAILURE')<void, ValueReturnModelSingle[], Error>();

export type ValuesResult = ReturnType<typeof fetchValuesActions.success> | ReturnType<typeof fetchValuesActions.failure>

export const fetchValuesAction = (): ThunkAction<Promise<ValuesResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {
    dispatch(fetchValuesActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    // accessheader.getAuthorization()
    const dataClient = new ValueClient(accessheader, config.datahost);

    return dataClient.getLastValues(getState().currentAquarium.name!)
      .then(values => dispatch(fetchValuesActions.success(values)))
      .catch(err => dispatch(fetchValuesActions.failure(err)))
  };
