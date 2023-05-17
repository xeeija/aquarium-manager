import { createRoot } from "react-dom/client"
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import { Provider } from "react-redux";
import { combineReducers } from "redux";
import rootReducer from "./services/reducers";
import { loggedIn } from './services/actions/user';
import { loadUserData } from './services/rest/security-helper';
import store from './services/store';

const AppReducer = combineReducers({
  rootReducer,
})

loadUserData()
  .then(info => {
    console.log('Loaded data: ' + JSON.stringify(info));
    return info.user && info.authentication ? store.dispatch(loggedIn({
      user: info.user,
      authenticationInformation: info.authentication,
      init: (_data) => { },
      toJSON: (data) => { },
    })) : false
  })
  .catch(e => console.log(e))

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
