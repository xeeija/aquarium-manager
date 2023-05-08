import { useDispatch } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { loggedIn } from '../../services/actions/actions';
import { executeDelayed } from '../../services/utils/async-helpers';
import { FormDescription, BuildForm } from '../../services/utils/form-builder';
import * as Validator from '../../services/utils/validators';
import {
  IonButtons,
  IonContent,
  IonHeader,
  IonMenuButton,
  IonTitle,
  IonToolbar,
  IonPage
} from '@ionic/react';
import { LoginRequest, UserClient } from '../../services/rest/interface';
import { IConfig } from '../../services/rest/iconfig';
import config from "../../services/rest/server-config"
import { AppStorage } from '../../services/utils/app-storage';

type formData = Readonly<LoginRequest>;

const formDescription: FormDescription<formData> = {
  name: 'login',
  fields: [
    {
      name: 'username', label: 'Email', type: 'email',
      position: 'floating', color: 'primary', validators: [Validator.required, Validator.email]
    },
    {
      name: 'password', label: 'Password', type: 'password',
      position: 'floating', color: 'primary', validators: [Validator.required]
    }
  ],
  submitLabel: 'Login'
}

const { Form, loading, error } = BuildForm(formDescription);

export const Login: React.FunctionComponent<RouteComponentProps<any>> = (props) => {

  const dispatch = useDispatch();

  const accessHeader = new IConfig()
  const userClient = new UserClient(accessHeader, config.host)

  const submit = async (loginData: LoginRequest) => {
    dispatch(loading(true));

    userClient.login(loginData)
      .then(async (loginInfo) => {
        const authresponse = loggedIn(loginInfo);
        dispatch(authresponse);

        if (loginInfo.hasError) {
          dispatch(error("Username or password is incorrect"))
          return
        }

        const jwtStore = new AppStorage()

        await Promise.all([
          jwtStore.set("user", JSON.stringify(
            typeof loginInfo.data?.user === "object"
              ? loginInfo.data.user
              : {}
          )),
          jwtStore.set("authentication", JSON.stringify(
            typeof loginInfo.data?.authenticationInformation === "object"
              ? loginInfo.data.authenticationInformation
              : {})
          ),
        ])

        executeDelayed(200, () => props.history.replace('/home'))
      })
      .catch((err: Error) => dispatch(error('Error while logging in: ' + err.message)))
      .finally(() => dispatch(loading(false)))
  };
  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonTitle>Login</IonTitle>
        </IonToolbar>
      </IonHeader>

      <IonContent>
        <Form handleSubmit={submit} />
      </IonContent>
    </IonPage>
  );
}

export default Login