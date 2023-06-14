import { AnyAction } from "redux";
import { addPictureActions, fetchPicturesActions } from "../actions/pictures";
import { createReducer } from "typesafe-actions";
import { Picture } from "../rest/interface";

const initialState: PicturesState = {
  isLoading: false,
  pictures: undefined,
  errorMessage: '',
}

export interface PicturesState {
  isLoading: boolean
  pictures?: Picture[]
  errorMessage: string
}

export const pictures = createReducer<PicturesState, AnyAction>(initialState)
  // fetch pictures
  .handleAction(fetchPicturesActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchPicturesActions.success, (state, action) =>
    ({ ...state, isLoading: false, corals: action.payload }))
  .handleAction(fetchPicturesActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  // add picture
  .handleAction(addPictureActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(addPictureActions.success, (state, action) =>
    ({ ...state, isLoading: false, animal: action.payload.data }))
  .handleAction(addPictureActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
