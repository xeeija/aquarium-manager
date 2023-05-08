import { createReducer } from "typesafe-actions";
import { UserResponse } from "../rest/interface";
import { AnyAction } from "redux";
import { loggedIn, loggedOut } from "../actions/actions";
import { clearUserData } from "../rest/security-helper";

const initialState: UserResponse = {
  init(_data?: any): void { },
  toJSON(data?: any): any { },
  user: undefined,
  authenticationInformation: undefined
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
