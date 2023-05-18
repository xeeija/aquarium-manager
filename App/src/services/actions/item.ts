import { AnyAction } from "redux";
import { ThunkAction } from "redux-thunk";
import { createAsyncAction } from "typesafe-actions";
import { RootState } from "../reducers";
import { IConfig } from "../rest/iconfig";
import { Coral, AquariumClient, Animal, ItemResponseModelOfAnimal, ItemResponseModelOfCoral } from "../rest/interface";
import config from "../rest/server-config";
import { formatErrors } from "../utils/format";

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
    const aquarium = getState().currentAquarium.name ?? ""

    // getCorals: getState().currentAquarium.name!
    return aquariumClient.getCoral(aquarium, id)
      .then(coral => dispatch(fetchCoralActions.success(coral)))
      .catch(err => dispatch(fetchCoralActions.failure(err)))
  };

export const fetchAnimalActions = createAsyncAction(
  'FETCH_ANIMAL_REQUEST',
  'FETCH_ANIMAL_SUCCESS',
  'FETCH_ANIMAL_FAILURE')<void, Animal, Error>();

export type AnimalResult = ReturnType<typeof fetchAnimalActions.success> | ReturnType<typeof fetchAnimalActions.failure>

export const fetchAnimalAction = (id: string): ThunkAction<Promise<AnimalResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(fetchAnimalActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    // getCorals: getState().currentAquarium.name!
    return aquariumClient.getAnimal(aquarium, id)
      .then(animal => dispatch(fetchAnimalActions.success(animal)))
      .catch(err => dispatch(fetchAnimalActions.failure(err)))
  };

export const editCoralActions = createAsyncAction(
  'EDIT_CORAL_REQUEST',
  'EDIT_CORAL_SUCCESS',
  'EDIT_CORAL_FAILURE')<void, ItemResponseModelOfCoral, Error>();

export type EditCoralResult = ReturnType<typeof editCoralActions.success> | ReturnType<typeof editCoralActions.failure>

export const editCoralAction = (coralId: string, coral: Coral): ThunkAction<Promise<EditCoralResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(editCoralActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    return aquariumClient.coralPUT(aquarium, coralId, coral)
      .then(coralResponse => {
        if (coralResponse.hasError) {
          return dispatch(editCoralActions.failure(new Error(`An error occurred: ${formatErrors(coralResponse.errorMessages)}`)))
        }

        return dispatch(editCoralActions.success(coralResponse))
      })
      .catch(err => dispatch(editCoralActions.failure(err)))
  };

export const editAnimalActions = createAsyncAction(
  'EDIT_ANIMAL_REQUEST',
  'EDIT_ANIMAL_SUCCESS',
  'EDIT_ANIMAL_FAILURE')<void, ItemResponseModelOfAnimal, Error>();

export type EditAnimalResult = ReturnType<typeof editAnimalActions.success> | ReturnType<typeof editAnimalActions.failure>

export const editAnimalAction = (animalId: string, animal: Animal): ThunkAction<Promise<EditAnimalResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(editAnimalActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    return aquariumClient.animalPUT(aquarium, animalId, animal)
      .then(animalResponse => {
        if (animalResponse.hasError) {
          return dispatch(editAnimalActions.failure(new Error(`An error occurred: ${formatErrors(animalResponse.errorMessages)}`)))
        }

        return dispatch(editAnimalActions.success(animalResponse))
      })
      .catch(err => dispatch(editAnimalActions.failure(err)))
  };

export const addCoralActions = createAsyncAction(
  'ADD_CORAL_REQUEST',
  'ADD_CORAL_SUCCESS',
  'ADD_CORAL_FAILURE')<void, ItemResponseModelOfCoral, Error>();

export type AddCoralResult = ReturnType<typeof addCoralActions.success> | ReturnType<typeof addCoralActions.failure>

export const addCoralAction = (coral: Coral): ThunkAction<Promise<AddCoralResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(addCoralActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    // add aquarium property, otherwise post request fails (500, cors)
    const coralRequest = new Coral()
    coralRequest.init({ ...coral, aquarium: aquarium })

    return aquariumClient.coralPOST(aquarium, coralRequest)
      .then(coralResponse => {
        if (coralResponse.hasError) {
          return dispatch(addCoralActions.failure(new Error(`An error occurred: ${formatErrors(coralResponse.errorMessages)}`)))
        }

        return dispatch(addCoralActions.success(coralResponse))
      })
      .catch(err => dispatch(addCoralActions.failure(err)))
  };

export const addAnimalActions = createAsyncAction(
  'ADD_ANIMAL_REQUEST',
  'ADD_ANIMAL_SUCCESS',
  'ADD_ANIMAL_FAILURE')<void, ItemResponseModelOfAnimal, Error>();

export type AddAnimalResult = ReturnType<typeof addAnimalActions.success> | ReturnType<typeof addAnimalActions.failure>

export const addAnimalAction = (animal: Animal): ThunkAction<Promise<AddAnimalResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(addAnimalActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    // add aquarium property, otherwise post request fails (500, cors)
    const animalRequest = new Animal()
    animalRequest.init({ ...animal, aquarium })

    return aquariumClient.animalPOST(aquarium, animalRequest)
      .then(animalResponse => {
        if (animalResponse.hasError) {
          return dispatch(addAnimalActions.failure(new Error(`An error occurred: ${formatErrors(animalResponse.errorMessages)}`)))
        }

        return dispatch(addAnimalActions.success(animalResponse))
      })
      .catch(err => dispatch(addAnimalActions.failure(err)))
  };
