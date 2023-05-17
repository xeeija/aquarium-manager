import { combineReducers } from "@reduxjs/toolkit";
import { user } from "./user";
import { items } from "./items";
import { formBuilderReducer } from "../utils/form-builder";

const rootReducer = combineReducers({
  user,
  // currentAquarium,
  items,
  formbuilder: formBuilderReducer
});

export type RootState = ReturnType<typeof rootReducer>;
export default rootReducer;
