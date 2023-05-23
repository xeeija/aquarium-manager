import { combineReducers } from "@reduxjs/toolkit";
import { user, aquariums, currentAquarium } from "./user";
import { items } from "./items";
import { item } from "./item";
import { formBuilderReducer } from "../utils/form-builder";
import { values } from "./values";

const rootReducer = combineReducers({
  user,
  currentAquarium,
  aquariums,
  items,
  item,
  values,
  formbuilder: formBuilderReducer
});

export type RootState = ReturnType<typeof rootReducer>;
export default rootReducer;
