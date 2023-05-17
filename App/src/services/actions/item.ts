import { AnyAction } from "redux";
import { ThunkAction } from "redux-thunk";
import { createAsyncAction } from "typesafe-actions";
import { RootState } from "../reducers";
import { IConfig } from "../rest/iconfig";
import { Coral, AquariumClient } from "../rest/interface";
import config from "../rest/server-config";

export const fetchCoralActions = createAsyncAction(
  'FETCH_CORAL_REQUEST',
  'FETCH_CORAL_SUCCESS',
  'FETCH_CORAL_FAILURE')<void, Coral, Error>();

export type CoralResult = ReturnType<typeof fetchCoralActions.success> | ReturnType<typeof fetchCoralActions.failure>

export const fetchCoralAction = (id: string): ThunkAction<Promise<CoralResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(fetchCoralActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessheader, config.host);

    // getCorals: getState().currentAquarium.name!
    return aquariumClient.getCoral("SchiScho", id)
      .then(
        coral => dispatch(fetchCoralActions.success(coral))
      )
      .catch(
        err => dispatch(fetchCoralActions.failure(err))
      )
  };

export const fetchAnimalActions = createAsyncAction(
  'FETCH_ANIMAL_REQUEST',
  'FETCH_ANIMAL_SUCCESS',
  'FETCH_ANIMAL_FAILURE')<void, Coral, Error>();

export type AnimalResult = ReturnType<typeof fetchAnimalActions.success> | ReturnType<typeof fetchAnimalActions.failure>

export const fetchAnimalAction = (id: string): ThunkAction<Promise<AnimalResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(fetchAnimalActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessheader, config.host);

    // getCorals: getState().currentAquarium.name!
    return aquariumClient.getAnimal("SchiScho", id)
      .then(
        animal => dispatch(fetchAnimalActions.success(animal))
      )
      .catch(
        err => dispatch(fetchAnimalActions.failure(err))
      )
  };
