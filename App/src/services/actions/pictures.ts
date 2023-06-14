import { createAsyncAction } from "typesafe-actions";
import { AquariumClient, FileParameter, ItemResponseModelOfPictureResponse, Picture } from "../rest/interface";
import { ThunkAction } from "redux-thunk";
import { RootState } from "../reducers";
import { AnyAction } from "redux";
import { IConfig } from "../rest/iconfig";
import config from "../rest/server-config"
import { formatErrors } from "../utils/format";

export const fetchPicturesActions = createAsyncAction(
  'FETCH_PICTURES_REQUEST',
  'FETCH_PICTURES_SUCCESS',
  'FETCH_PICTURES_FAILURE')<void, Picture[], Error>();

export type PicturesResult = ReturnType<typeof fetchPicturesActions.success> | ReturnType<typeof fetchPicturesActions.failure>

export const fetchPicturesAction = (): ThunkAction<Promise<PicturesResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(fetchPicturesActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    return aquariumClient.pictureAll(aquarium)
      .then(corals => dispatch(fetchPicturesActions.success(corals)))
      .catch(err => dispatch(fetchPicturesActions.failure(err)))
  };


export const addPictureActions = createAsyncAction(
  'ADD_PICTURE_REQUEST',
  'ADD_PICTURE_SUCCESS',
  'ADD_PICTURE_FAILURE')<void, ItemResponseModelOfPictureResponse, Error>();

export type AddPictureResult = ReturnType<typeof addPictureActions.success> | ReturnType<typeof addPictureActions.failure>

export const addPictureAction = (picture: Picture, formFile: FileParameter): ThunkAction<Promise<AddPictureResult>, RootState, null, AnyAction> =>
  (dispatch, getState) => {

    dispatch(addPictureActions.request());
    const token = getState().user.authenticationInformation!.token || '';
    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);
    const aquarium = getState().currentAquarium.name ?? ""

    // add aquarium property, otherwise post request fails (500, cors)
    const pictureRequest = new Picture()
    pictureRequest.init({ ...picture, aquarium: aquarium })

    return aquariumClient.picturePOST(aquarium, picture.description, formFile)
      .then(coralResponse => {
        if (coralResponse.hasError) {
          return dispatch(addPictureActions.failure(new Error(`An error occurred: ${formatErrors(coralResponse.errorMessages)}`)))
        }

        return dispatch(addPictureActions.success(coralResponse))
      })
      .catch(err => dispatch(addPictureActions.failure(err)))
  };