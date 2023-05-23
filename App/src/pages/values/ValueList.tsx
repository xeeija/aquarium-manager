import {
  IonButtons,
  IonContent,
  IonHeader,
  IonIcon,
  IonItem,
  IonList,
  IonMenuButton,
  IonPage,
  IonTitle,
  IonToolbar,
  IonSpinner,
  IonItemSliding,
  IonCard,
  IonCardHeader,
  IonCardTitle,
  IonToast,
  RefresherEventDetail,
  IonRefresher,
  IonRefresherContent
} from '@ionic/react';
import { FC, useEffect } from 'react';
import { RouteComponentProps } from "react-router";
import { ThunkDispatch } from "redux-thunk";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../services/reducers";
import { fetchValuesAction, fetchValuesActions, ValuesResult } from "../../services/actions/values";
import { water } from 'ionicons/icons';
import { ValueClient, VisualsBinaryReturnModel, VisualsNumericReturnModel } from '../../services/rest/data';
import { IconConverter } from '../../services/utils/iconConverter';
import { IConfig } from '../../services/rest/iconfig';
import config from "../../services/rest/server-config"
//import {fetchValues} from "../../services/rest/values";


const ValueList: FC<RouteComponentProps> = ({ history }) => {

  const { values, isLoading, errorMessage } = useSelector((s: RootState) => s.values);
  const current = useSelector((s: RootState) => s.currentAquarium);
  const token = useSelector((s: RootState) => s.user.authenticationInformation!.token || '');
  const dispatch = useDispatch();

  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, ValuesResult>;

  useEffect(() => { if (values.length === 0) thunkDispatch(fetchValuesAction()).then(x => console.log(x)); }, []);

  const NoValuesInfo = () => !isLoading && values.length === 0 ?
    (<IonCard>
      <img src='assets/images/img.png' alt=""></img>
      <IonCardHeader>
        <IonCardTitle>No Values found...</IonCardTitle>
      </IonCardHeader>

    </IonCard>) : (<></>)

  const doRefresh = (event: CustomEvent<RefresherEventDetail>) => {
    const accessHeader = new IConfig()
    accessHeader.setToken(token)
    const dataClient = new ValueClient(accessHeader, config.datahost)

    // TODO: Move into custom redux action
    dataClient.getLastValues(current.name ?? "")
      .then(values => dispatch(fetchValuesActions.success(values)))
      .then(() => event.detail.complete)
      .catch(() => dispatch(fetchValuesActions.failure(new Error(`Error while refreshing`))))
  }

  const ListValues = () => {

    const items = values.map(value => {

      const icon = IconConverter(value?.visuals?.icon) ?? water;
      let unit = "";
      let visualText = `${value.sample.value}`

      if (value.visuals !== null) {
        if (value.visuals instanceof VisualsNumericReturnModel) {
          // const visual = value.visuals as VisualsNumericReturnModel
          unit = ` ${value.visuals.unit}`
          visualText += unit
        }

        if (value.visuals instanceof VisualsBinaryReturnModel) {
          // const visual = value.visuals as VisualsBinaryReturnModel
          visualText = value.visuals.finalText
        }
      }

      if (value.sample !== null) {
        return (
          <IonItemSliding key={value.dataPoint.id}>
            <IonItem key={value.dataPoint.id}>
              <IonIcon icon={icon} />
              {value.dataPoint.dataPointVisual === '' ? value.dataPoint.name : value.dataPoint.dataPointVisual}
              <div className="item-note" slot="end">
                {visualText}
              </div>
            </IonItem>
          </IonItemSliding>
        );
      }

      return null;
    });
    return items.length > 0 ? <IonList>{items}</IonList> : <NoValuesInfo />;
  };

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonTitle>Value List</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent>
        <IonRefresher slot='fixed' onIonRefresh={doRefresh}>
          <IonRefresherContent />
        </IonRefresher>
        {isLoading ? <IonItem><IonSpinner />Loading Values...</IonItem> : <ListValues />}
        <IonToast
          isOpen={errorMessage ? errorMessage.length > 0 : false}
          onDidDismiss={() => false}
          message={errorMessage}
          duration={5000}
          color='danger'
        />

      </IonContent>
    </IonPage>
  );
};

export default ValueList;
