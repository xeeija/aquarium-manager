import React from "react"
import { IonApp, IonRouterOutlet, IonSplitPane, setupIonicReact } from '@ionic/react';
import { IonReactRouter } from '@ionic/react-router';
import { Redirect, Route } from 'react-router-dom';
import Menu from './components/Menu';
import Home from './pages/home/Home';
import Login from './pages/login/Login';
import Register from "./pages/register/Register";

/* Core CSS required for Ionic components to work properly */
import '@ionic/react/css/core.css';

/* Basic CSS for apps built with Ionic */
import '@ionic/react/css/normalize.css';
import '@ionic/react/css/structure.css';
import '@ionic/react/css/typography.css';

/* Optional CSS utils that can be commented out */
import '@ionic/react/css/padding.css';
import '@ionic/react/css/float-elements.css';
import '@ionic/react/css/text-alignment.css';
import '@ionic/react/css/text-transformation.css';
import '@ionic/react/css/flex-utils.css';
import '@ionic/react/css/display.css';

/* Theme variables */
import './theme/variables.css';
import { SecureRoute } from "./components/SecureRoute";
import ItemsList from "./pages/items/ItemsList";
import ItemDetail from "./pages/items/ItemDetail";
import AddEditItem from "./pages/items/AddEditItem";

setupIonicReact();

const App: React.FC = () => {
  return (
    <IonApp>
      <IonReactRouter>
        <IonSplitPane contentId="main">
          <Menu />
          <IonRouterOutlet id="main">
            <Route path="/home" component={Home} exact={true} />
            <Route path="/login" component={Login} exact={true} />
            <Route path="/register" component={Register} exact={true} />
            <SecureRoute path="/items" component={ItemsList} exact={true} />
            <SecureRoute path="/coral/show/:id" component={ItemDetail("coral")} exact={true} />
            <SecureRoute path="/animal/show/:id" component={ItemDetail("animal")} exact={true} />
            <SecureRoute path="/coral/edit/:id" component={AddEditItem("coral", "edit")} exact={true} />
            <SecureRoute path="/animal/edit/:id" component={AddEditItem("animal", "edit")} exact={true} />
            <SecureRoute path="/coral/add" component={AddEditItem("coral", "add")} exact={true} />
            <SecureRoute path="/animal/add" component={AddEditItem("animal", "add")} exact={true} />
            <Route path="/" exact={true}>
              <Redirect to="/home" />
            </Route>
          </IonRouterOutlet>
        </IonSplitPane>
      </IonReactRouter>
    </IonApp>
  );
};

export default App;
