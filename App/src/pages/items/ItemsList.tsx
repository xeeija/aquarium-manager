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
  IonItemOptions,
  IonItemOption,
  IonCard,
  IonCardHeader,
  IonCardTitle,
  IonToast,
  IonButton,
  IonLabel
} from '@ionic/react';
import {
  information,
  fish, flower
} from 'ionicons/icons';
import React, { useEffect } from 'react';
//import '../../services/actions/security';
import { RouteComponentProps } from "react-router";
import { ThunkDispatch } from "redux-thunk";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../services/reducers";
import {
  AnimalsResult,
  CoralsResult,
  fetchAnimalsAction,
  fetchCoralsAction
} from "../../services/actions/items";
//import {fetchValues} from "../../services/rest/values";


const ItemsList: React.FC<RouteComponentProps> = ({ history }) => {

  const { corals, animals, isLoading, errorMessage } = useSelector((s: RootState) => s.items);
  // const token = useSelector((s: RootState) => s.user.authenticationInformation!.token || '');
  const dispatch = useDispatch();
  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, CoralsResult | AnimalsResult>;
  useEffect(() => {
    thunkDispatch(fetchCoralsAction()).then(x => console.log(x));
    thunkDispatch(fetchAnimalsAction()).then(x => console.log(x));
  }, [thunkDispatch]);

  const NoValuesInfo = () => !isLoading && corals.length === 0 ?
    (<IonCard>
      {/* <img src='assets/images/img.png' alt=""></img> */}
      <IonCardHeader>
        <IonCardTitle>No Corals or Animals found...</IonCardTitle>
      </IonCardHeader>
    </IonCard>) : (<></>)

  const ListCorals = () => {

    const items = corals.map(coral => {

      let icon = flower;
      // let unit = "";

      return (
        <IonItemSliding key={coral.id}>
          <IonItemOptions side="end">
            <IonItemOption onClick={() => { console.log(coral.name) }}><IonIcon icon={information} /> Details</IonItemOption>
          </IonItemOptions>
          <IonItem key={coral.id} onClick={() => history.push('/coral/show/' + coral.id)}>
            <IonIcon icon={icon} />
            {coral.name} ({coral.amount})
            <div className="item-note" slot="end">
              {coral.species}
            </div>
          </IonItem>
        </IonItemSliding>
      );

    });
    return corals.length > 0 ? <IonList>{items}</IonList> : <NoValuesInfo />;
  };

  const ListAnimals = () => {

    const items = animals.map(animal => {

      let icon = fish;
      // let unit = "";

      return (
        <IonItemSliding key={animal.id}>
          <IonItemOptions side="end">
            <IonItemOption onClick={() => { console.log(animal.name) }}><IonIcon icon={information} /> Details</IonItemOption>
          </IonItemOptions>
          <IonItem key={animal.id} onClick={() => history.push('/animal/show/' + animal.id)}>
            <IonIcon icon={icon} />
            {animal.name} ({animal.amount})
            <div className="item-note" slot="end">
              {animal.species}
            </div>
          </IonItem>
        </IonItemSliding>
      );

    });
    return animals.length > 0 ? <IonList>{items}</IonList> : <NoValuesInfo />;
  };

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonButtons slot="primary">
            <IonButton onClick={() => history.push('/animal/add')}>
              <IonIcon slot="icon-only" icon={fish} />
            </IonButton>
            <IonButton onClick={() => history.push('/coral/add')}>
              <IonIcon slot="icon-only" icon={flower} />
            </IonButton>
          </IonButtons>
          <IonTitle>Corals and Animals List</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent>
        <IonItem>
          <IonLabel>Corals</IonLabel>
        </IonItem>

        {isLoading ? <IonItem><IonSpinner />Loading Corals...</IonItem> : <ListCorals />}

        <IonItem>
          <IonLabel>Animals</IonLabel>
        </IonItem>

        {isLoading ? <IonItem><IonSpinner />Loading Animals...</IonItem> : <ListAnimals />}

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

export default ItemsList;
