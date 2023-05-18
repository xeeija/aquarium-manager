import { AnyAction } from "redux";
import { createReducer } from "typesafe-actions";
import { fetchCoralActions, fetchAnimalActions, editAnimalActions, editCoralActions, addAnimalActions, addCoralActions } from "../actions/item";
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
  // fetch coral
  .handleAction(fetchCoralActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchCoralActions.success, (state, action) =>
    ({ ...state, isLoading: false, coral: action.payload }))
  .handleAction(fetchCoralActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  // fetch animal
  .handleAction(fetchAnimalActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchAnimalActions.success, (state, action) =>
    ({ ...state, isLoading: false, animal: action.payload }))
  .handleAction(fetchAnimalActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  // edit coral
  .handleAction(editCoralActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(editCoralActions.success, (state, action) =>
    ({ ...state, isLoading: false, coral: action.payload.data }))
  .handleAction(editCoralActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  // edit animal
  .handleAction(editAnimalActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(editAnimalActions.success, (state, action) =>
    ({ ...state, isLoading: false, animal: action.payload.data }))
  .handleAction(editAnimalActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  // add coral
  .handleAction(addCoralActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(addCoralActions.success, (state, action) =>
    ({ ...state, isLoading: false, coral: action.payload.data }))
  .handleAction(addCoralActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  // add animal
  .handleAction(addAnimalActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(addAnimalActions.success, (state, action) =>
    ({ ...state, isLoading: false, animal: action.payload.data }))
  .handleAction(addAnimalActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
