import { AnyAction } from "redux";
import { createReducer } from "typesafe-actions";
import { fetchCoralActions, fetchAnimalActions } from "../actions/item";
import { Coral, Animal } from "../rest/interface";

const initialState: AquariumItemState = {
  isLoading: false,
  coral: undefined,
  animal: undefined,
  errorMessage: '',
}

export interface AquariumItemState {
  isLoading: boolean;
  coral?: Coral;
  animal?: Animal;
  errorMessage: string;
}

export const item = createReducer<AquariumItemState, AnyAction>(initialState)
  .handleAction(fetchCoralActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchCoralActions.success, (state, action) =>
    ({ ...state, isLoading: false, coral: action.payload }))
  .handleAction(fetchCoralActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  .handleAction(fetchAnimalActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchAnimalActions.success, (state, action) =>
    ({ ...state, isLoading: false, animal: action.payload }))
  .handleAction(fetchAnimalActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
