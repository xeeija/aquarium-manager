import { combineReducers } from "@reduxjs/toolkit";
import { user } from "./user";
import { items } from "./items";
import { item } from "./item";
import { formBuilderReducer } from "../utils/form-builder";

const rootReducer = combineReducers({
  user,
  // currentAquarium,
  items,
  item,
  formbuilder: formBuilderReducer
});

export type RootState = ReturnType<typeof rootReducer>;
export default rootReducer;
