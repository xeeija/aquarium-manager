import { FC, useEffect } from "react"
import { useSelector, useDispatch } from "react-redux";
import { RouteComponentProps } from "react-router"
import { ThunkDispatch } from "redux-thunk";
import { CoralResult, AnimalResult, fetchCoralAction, fetchAnimalAction } from "../../services/actions/item";
import { RootState } from "../../services/reducers";
import { IonPage, IonHeader, IonToolbar, IonButtons, IonMenuButton, IonButton, IonIcon, IonTitle, IonContent, IonItem, IonLabel, IonToast, IonCard, IonCardContent, IonCardHeader, IonCardSubtitle, IonCardTitle, IonSpinner } from "@ionic/react";
import { pencil } from "ionicons/icons";
import { capitalize, shortDate } from "../../services/utils/format";

// eslint-disable-next-line import/no-anonymous-default-export
export default (type: "coral" | "animal"): FC<RouteComponentProps<{ id: string }>> => ({ match, history }) => {

  const { coral, animal, isLoading, errorMessage } = useSelector((s: RootState) => s.item);
  const token = useSelector((s: RootState) => s.user.authenticationInformation!.token || '');
  const dispatch = useDispatch();
  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, CoralResult | AnimalResult>;
  useEffect(() => {
    if (type === "coral") {
      thunkDispatch(fetchCoralAction(match.params.id)).then(x => console.log(x));
    } else {
      thunkDispatch(fetchAnimalAction(match.params.id)).then(x => console.log(x));
    }
  }, [match.params.id, thunkDispatch]);

  const item = type === "coral" ? coral : animal;

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonButtons slot="primary">
            <IonButton onClick={() => history.push(`${history.location.pathname.replace("show", "edit")}`)}>
              <IonIcon slot="icon-only" icon={pencil} />
            </IonButton>
          </IonButtons>
          <IonTitle>{item?.name}</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent>
        {isLoading && <IonItem><IonSpinner />Loading {capitalize(type)}s...</IonItem>}
        {!isLoading && (
          <IonCard>
            <IonCardHeader>
              <IonCardTitle>{item?.name}</IonCardTitle>
              <IonCardSubtitle>Species: {item?.species}</IonCardSubtitle>
            </IonCardHeader>

            <IonCardContent>
              {type === "coral" && <p>Type: {coral?.coralType}</p>}
              {type === "animal" && <p>{animal?.isAlive ? "Still Alive" : `Died on ${shortDate(animal?.deathDate)}`}</p>}
              <p>Inserted: {shortDate(item?.inserted)}</p>
              <p>Amount: {item?.amount}</p>
            </IonCardContent>

            {item?.description && (
              <IonCardContent>
                {item?.description}
              </IonCardContent>
            )}
          </IonCard>
        )}

        <IonToast
          isOpen={errorMessage ? errorMessage.length > 0 : false}
          onDidDismiss={() => false}
          message={errorMessage}
          duration={5000}
          color='danger'
        />

      </IonContent>
    </IonPage>
  )
}
