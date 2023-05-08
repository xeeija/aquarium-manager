import ReactDOM from 'react-dom';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import { Provider } from "react-redux";
import { combineReducers } from "redux";
import rootReducer from "./services/reducers";
import { loggedIn } from './services/actions/actions';
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
  ReactDOM.render(
    <Provider store={store}>
      <App />
    </Provider>,
    document.getElementById("root")
  );
};

render();

// if (process.env.NODE_ENV === "development" && module.hot) {
//   module.hot.accept("./App", render);
// }

serviceWorkerRegistration.unregister();
