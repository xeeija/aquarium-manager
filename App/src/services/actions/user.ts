import { createAction } from "typesafe-actions"
import { UserResponse } from "../rest/interface";

export const loggedIn = createAction('user/loggedIn')<UserResponse>();
export const loggedOut = createAction('user/loggedOut')<void>();

export const currentAquarium = createAction('user/currentAquarium')<void>();
