import { createReducer } from "typesafe-actions";
import { Aquarium, AquariumUserResponse, UserResponse } from "../rest/interface";
import { AnyAction } from "redux";
import { loggedIn, loggedOut, currentAquarium as currentAqua, fetchAquariumsActions, fetchAquariumActions } from "../actions/user";
import { clearUserData } from "../rest/security-helper";

export interface AquariumState {
  isLoading: boolean;
  errorMessage: string;
  // aquariumuserresponse : Aquarium | null;
  aquariumlist: AquariumUserResponse[] | null;
}

const initialState: UserResponse = {
  init(_data?: any): void { },
  toJSON(data?: any): any { },
  user: undefined,
  authenticationInformation: undefined
}

const initialAquariumState: AquariumState =
{
  isLoading: false,
  errorMessage: "",
  //aquariumuserresponse: null,
  aquariumlist: []
}

export const user = createReducer<UserResponse, AnyAction>(initialState)
  .handleAction(loggedIn, (state, action) => {
    return action.payload
  })
  .handleAction(loggedOut,
    (state, action) => {
      clearUserData();
      return initialState;
    }
  )


export const currentAquarium = createReducer<Aquarium, AnyAction>(new Aquarium())
  .handleAction(currentAqua, (state, action) => {
    return action.payload
  })
  .handleAction(loggedOut,
    (state, action) => {
      clearUserData();
      return initialState;
    }
  )

export const aquariums = createReducer<AquariumState, AnyAction>(initialAquariumState)
  .handleAction(fetchAquariumsActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchAquariumActions.request, (state, action) =>
    ({ ...state, isLoading: true, errorMessage: '' }))
  .handleAction(fetchAquariumActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  .handleAction(fetchAquariumActions.success, (state, action) =>
    ({ ...state, isLoading: false, aquariumuserresponse: action.payload }))
  .handleAction(fetchAquariumsActions.failure, (state, action) =>
    ({ ...state, isLoading: false, errorMessage: action.payload.message }))
  .handleAction(fetchAquariumsActions.success, (state, action) =>
    ({ ...state, isLoading: false, aquariumlist: action.payload }))
