import { useDispatch } from 'react-redux';
import { RouteComponentProps } from 'react-router';
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
import { RegisterRequest, UserClient } from '../../services/rest/interface';
import { IConfig } from '../../services/rest/iconfig';
import config from "../../services/rest/server-config"
import { FC } from 'react';

type FormData = Readonly<RegisterRequest>;

const formDescription: FormDescription<FormData> = {
  name: 'register',
  fields: [
    {
      name: 'firstname', label: 'Firstname', type: 'text',
      position: 'floating', color: 'primary', validators: [Validator.required]
    },
    {
      name: 'lastname', label: 'Lastname', type: 'text',
      position: 'floating', color: 'primary', validators: [Validator.required]
    },
    {
      name: 'email', label: 'Email', type: 'email',
      position: 'floating', color: 'primary', validators: [Validator.required, Validator.email]
    },
    {
      name: 'password', label: 'Password', type: 'password',
      position: 'floating', color: 'primary', validators: [Validator.required]
    },
  ],
  submitLabel: 'Register'
}

const { Form, loading, error } = BuildForm(formDescription);

export const Register: FC<RouteComponentProps<any>> = (props) => {

  const dispatch = useDispatch();

  const accessHeader = new IConfig()
  const userClient = new UserClient(accessHeader, config.host)

  const submit = async (registerData: RegisterRequest) => {
    dispatch(loading(true));

    // const user = new User({
    //   ...registerData,
    //   active: true,
    // })

    userClient.register(registerData)
      .then(async (registerInfo) => {

        if (registerInfo.hasError) {
          const errors = Object.entries(registerInfo.errorMessages ?? {}).map(([k, v]) => `${k}: ${v}`).join("\\n")
          dispatch(error(`An error occurred: ${errors}`))
          return
        }

        executeDelayed(200, () => props.history.push('/login'))
      })
      .catch((err: Error) => dispatch(error('Error while registering: ' + err.message)))
      .finally(() => dispatch(loading(false)))
  };
  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonTitle>Register</IonTitle>
        </IonToolbar>
      </IonHeader>

      <IonContent>
        <Form handleSubmit={submit} />
      </IonContent>
    </IonPage>
  );
}

export default Register