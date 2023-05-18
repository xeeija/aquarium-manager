import { createRoot } from "react-dom/client"
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import { Provider } from "react-redux";
import { combineReducers } from "redux";
import rootReducer, { RootState } from "./services/reducers";
import { AquariumResult, fetchAquariumAction, loggedIn } from './services/actions/user';
import { loadUserData } from './services/rest/security-helper';
import store from './services/store';
import reportWebVitals from "./reportWebVitals";
import { AppStorage } from "./services/utils/app-storage";
import { ThunkDispatch } from "redux-thunk";
import { Aquarium } from "./services/rest/interface";

const AppReducer = combineReducers({
  rootReducer,
})
loadUserData().then(info => {
  const storage = new AppStorage();
  const aquariumThunkDispatch = store.dispatch as ThunkDispatch<RootState, null, AquariumResult>;

  if (info.user && info.authentication && info.aquarium) {
    store.dispatch(loggedIn({
      init(_data?: any): void { },
      toJSON(data?: any): any { },
      user: info.user,
      authenticationInformation: info.authentication
    }));

    const aq = info.aquarium as Aquarium;

    aquariumThunkDispatch(fetchAquariumAction(aq.id!)).then(x => console.log(x.payload));
  }
  else {
    return false;
  }

})

const render = () => {
  const App = require("./App").default;

  const root = createRoot(document.getElementById("root")!)
  root.render(
    <Provider store={store}>
      <App />
    </Provider>
  );
};

render();

// if (process.env.NODE_ENV === "development" && module.hot) {
//   module.hot.accept("./App", render);
// }

serviceWorkerRegistration.unregister();

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals()
