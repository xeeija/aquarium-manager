import { FC } from 'react';
import { useDispatch } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { AquariumResult, AquariumsResult, currentAquarium, fetchAquariumAction, fetchAquariumsAction, loggedIn } from '../../services/actions/user';
import { executeDelayed } from '../../services/utils/async-helpers';
import { FormDescription, BuildForm } from '../../services/utils/form-builder';
import * as Validator from '../../services/utils/validators';
import { IonButtons, IonContent, IonHeader, IonMenuButton, IonTitle, IonToolbar, IonPage } from '@ionic/react';
import store from "../../services/store";
import { LoginRequest, UserClient, UserResponse } from '../../services/rest/interface';
import { IConfig } from '../../services/rest/iconfig';
import config from "../../services/rest/server-config"
import { AppStorage } from "../../services/utils/app-storage";
import { ThunkDispatch } from 'redux-thunk';
import { RootState } from '../../services/reducers';

type FormData = Readonly<LoginRequest>;

const formDescription: FormDescription<FormData> = {
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

export const Login: FC<RouteComponentProps<any>> = (props) => {

  const dispatch = useDispatch();

  const accessHeader = new IConfig()
  const userClient = new UserClient(accessHeader, config.host)

  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, AquariumsResult>;
  const aquariumThunkDispatch = dispatch as ThunkDispatch<RootState, null, AquariumResult>;

  const submit = async (loginData: LoginRequest) => {
    dispatch(loading(true));

    // userClient.login(loginData)
    //   .then(async (loginInfo) => {
    //     const authresponse = loggedIn(loginInfo);
    //     dispatch(authresponse);

    //     if (loginInfo.hasError) {
    //       dispatch(error("Username or password is incorrect"))
    //       return
    //     }

    //     const jwtStore = new AppStorage()

    //     await Promise.all([
    //       jwtStore.set("user", JSON.stringify(
    //         typeof loginInfo.data?.user === "object"
    //           ? loginInfo.data.user
    //           : {}
    //       )),
    //       jwtStore.set("authentication", JSON.stringify(
    //         typeof loginInfo.data?.authenticationInformation === "object"
    //           ? loginInfo.data.authenticationInformation
    //           : {})
    //       ),
    //     ])

    //     executeDelayed(200, () => props.history.replace('/home'))
    //   })
    //   .catch((err: Error) => dispatch(error('Error while logging in: ' + err.message)))
    //   .finally(() => dispatch(loading(false)))

    userClient.login(loginData)
      .then(async (loginInfo) => {
        if (loginInfo.hasError || !(loginInfo.data instanceof UserResponse)) {
          dispatch(error('Error while logging in: ' + loginInfo.errorMessages));
          return
        }

        const authResponse = loggedIn(loginInfo.data);

        dispatch(authResponse);
        console.log(authResponse);

        const user = loginInfo.data?.user
        const authInfo = loginInfo.data?.authenticationInformation

        const jwtStore = new AppStorage();
        await Promise.all([
          jwtStore.set('user', JSON.stringify((typeof user === 'object') ? user : {})),
          jwtStore.set('authentication', JSON.stringify((typeof authInfo === 'object') ? authInfo : {}))
        ])

        const allAquariums = await thunkDispatch(fetchAquariumsAction())

        if (Array.isArray(allAquariums.payload)) {
          const aquariumResponse = await aquariumThunkDispatch(fetchAquariumAction(allAquariums.payload[0].aquarium?.id!))

          const aquarium = aquariumResponse.payload
          jwtStore.set('aquarium', JSON.stringify((typeof aquarium === 'object') ? aquarium : {}))

          store.dispatch(currentAquarium(allAquariums.payload[0].aquarium!));
        }

        executeDelayed(200, () => props.history.push('/home'))
      })
      .catch((err: Error) => {
        dispatch(error('Error while logging in: ' + err.message));
      })
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
