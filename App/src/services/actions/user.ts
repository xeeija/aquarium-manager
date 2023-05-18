import { createAction, createAsyncAction } from "typesafe-actions"
import { Aquarium, AquariumClient, AquariumUserResponse, UserResponse } from "../rest/interface";
import config from "../rest/server-config";
import { AnyAction } from "redux";
import { ThunkAction } from "redux-thunk";
import { RootState } from "../reducers";
import { IConfig } from "../rest/iconfig";

export const loggedIn = createAction('user/loggedIn')<UserResponse>();
export const loggedOut = createAction('user/loggedOut')<void>();

export const currentAquarium = createAction('user/currentAquarium')<Aquarium>();

const accessHeader = new IConfig();

export const fetchAquariumsActions = createAsyncAction(
  'FETCH_AQUARIUMS_REQUEST',
  'FETCH_AQUARIUMS_SUCCESS',
  'FETCH_AQUARIUMS_FAILURE')<void, AquariumUserResponse[], Error>();

export const fetchAquariumActions = createAsyncAction(
  'FETCH_AQUARIUM_REQUEST',
  'FETCH_AQUARIUM_SUCCESS',
  'FETCH_AQUARIUM_FAILURE')<void, AquariumUserResponse, Error>();

export type AquariumResult = ReturnType<typeof fetchAquariumActions.success> | ReturnType<typeof fetchAquariumActions.failure>;
export type AquariumsResult = ReturnType<typeof fetchAquariumsActions.success> | ReturnType<typeof fetchAquariumsActions.failure>;

export const fetchAquariumsAction = (): ThunkAction<Promise<AquariumsResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {
    dispatch(fetchAquariumsActions.request());

    const token = getState().user.authenticationInformation!.token || '';

    accessHeader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessHeader, config.host);

    return aquariumClient.forUser()
      .then(value => dispatch(fetchAquariumsActions.success(value)))
      .catch(error => dispatch(fetchAquariumsActions.failure(error)));
  };

export const fetchAquariumAction = (id: string): ThunkAction<Promise<AquariumResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {
    dispatch(fetchAquariumActions.request());

    const token = getState().user.authenticationInformation!.token || '';
    accessHeader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessHeader, config.host);

    return aquariumClient.get(id)
      .then(value => dispatch(fetchAquariumActions.success(value)))
      .catch(error => dispatch(fetchAquariumActions.failure(error)));
  };
