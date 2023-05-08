import { applyMiddleware, createStore } from "redux";
import rootReducer from "./reducers";
import thunk from "redux-thunk";

// TODO: Deprecated
const store = createStore(rootReducer, applyMiddleware(thunk))

export type AppDispatch = typeof store.dispatch
export default store
