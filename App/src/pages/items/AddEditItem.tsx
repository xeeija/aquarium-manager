import { FC, useEffect } from "react"
import { RouteComponentProps } from "react-router"
import { BuildForm, FieldDescriptionType, FormDescription } from "../../services/utils/form-builder";
import { Animal, AquariumClient, Coral } from "../../services/rest/interface";
import * as Validator from '../../services/utils/validators';
import { IonPage, IonHeader, IonToolbar, IonButtons, IonMenuButton, IonTitle, IonContent } from "@ionic/react";
import { useDispatch, useSelector } from "react-redux";
import { IConfig } from "../../services/rest/iconfig";
import { executeDelayed } from "../../services/utils/async-helpers";
import config from "../../services/rest/server-config"
import { capitalize } from "../../services/utils/format";
import { ThunkDispatch } from "redux-thunk";
import { CoralResult, AnimalResult, fetchCoralAction, fetchAnimalAction, EditAnimalResult, editAnimalAction, editCoralAction, EditCoralResult } from "../../services/actions/item";
import { RootState } from "../../services/reducers";

type FormDataCoral = Readonly<Coral>;
type FormDataAnimal = Readonly<Animal>;

const itemFields: FieldDescriptionType<Readonly<Animal | Coral>>[] = [
  {
    name: 'name', label: 'Name', type: 'text',
    position: 'floating', color: 'primary', validators: [Validator.required]
  },
  {
    name: 'species', label: 'Species', type: 'text',
    position: 'floating', color: 'primary', validators: [Validator.required]
  },
  {
    name: 'inserted', label: 'Inserted', type: 'date',
    position: 'floating', color: 'primary', validators: [Validator.required]
  },
  {
    name: 'amount', label: 'Amount', type: 'number',
    position: 'floating', color: 'primary', validators: [Validator.required, Validator.minValue(1)]
  },
  {
    name: 'description', label: 'Description', type: 'text',
    position: 'floating', color: 'primary', validators: [Validator.required]
  },
]

const animalFields: FieldDescriptionType<Readonly<Animal>>[] = [
  {
    name: 'isAlive', label: 'Alive', type: 'select', options: [
      {
        key: true,
        value: "Still Alive"
      },
      {
        key: false,
        value: "Dead"
      },
    ],
    position: 'floating', color: 'primary', validators: []
  },
  {
    name: 'deathDate', label: 'Death date', type: 'date',
    position: 'floating', color: 'primary', validators: []
  },
]

const coralFields: FieldDescriptionType<Readonly<Coral>>[] = [
  {
    name: 'coralType', label: 'Coral Type', type: 'select', options: [
      {
        key: "HardCoral",
        value: "HardCoral"
      },
      {
        key: "SoftCoral",
        value: "SoftCoral"
      },
    ],
    position: 'floating', color: 'primary', validators: [Validator.required]
  },
]

const formDescription: (mode: "add" | "edit", type: "coral" | "animal") => FormDescription<FormDataAnimal | FormDataCoral> = (mode, type) => ({
  name: `${mode}-${type}`,
  fields: [
    ...itemFields,
    ...(type === "animal" ? animalFields : coralFields),
  ],
  submitLabel: `${capitalize(mode)}`
})

// eslint-disable-next-line import/no-anonymous-default-export
export default (type: "coral" | "animal", mode: "add" | "edit"): FC<RouteComponentProps<{ id?: string }>> => ({ match, history }) => {

  const { Form, loading, error } = BuildForm(formDescription(mode, type));

  const { coral, animal } = useSelector((s: RootState) => s.item);
  const dispatch = useDispatch();

  const thunkDispatchEdit = dispatch as ThunkDispatch<RootState, null, EditAnimalResult | EditCoralResult>;
  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, CoralResult | AnimalResult>;

  useEffect(() => {
    if (mode === "add") {
      return
    }

    if (type === "coral") {
      thunkDispatch(fetchCoralAction(match.params.id ?? "")).then(x => console.log(x));
      console.log(coral);
    } else {
      thunkDispatch(fetchAnimalAction(match.params.id ?? "")).then(x => console.log(x));
      console.log(animal);
    }
  }, []);

  const accessHeader = new IConfig()
  const aquariumClient = new AquariumClient(accessHeader, config.host);

  const submitAdd = async (itemData: Animal | Coral) => {
    dispatch(loading(true));

    const itemPost = type === "animal" ? aquariumClient.animalPOST : aquariumClient.coralPOST

    itemPost("SchiScho", itemData)
      .then(async (itemResponse) => {

        if (itemResponse.hasError) {
          const errors = Object.entries(itemResponse.errorMessages ?? {}).map(([k, v]) => `${k}: ${v}`).join("\\n")
          dispatch(error(`An error occurred: ${errors}`))
          return
        }

        executeDelayed(200, () => history.push(history.location.pathname.replace("edit", "show")))
      })
      .catch((err: Error) => dispatch(error('Error while registering: ' + err.message)))
      .finally(() => dispatch(loading(false)))
  };

  const submitEdit = async (itemData: Animal | Coral) => {
    dispatch(loading(true));

    const editAction = type === "animal" ? editCoralAction : editCoralAction

    thunkDispatchEdit(editAction(itemData.id ?? "", itemData))
      .then(x => {
        console.log(x)
        executeDelayed(200, () => history.push(history.location.pathname.replace("edit", "show")))
      })
      .finally(() => dispatch(loading(false)));
  };

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonTitle>{capitalize(mode)} {capitalize(type)}</IonTitle>
        </IonToolbar>
      </IonHeader>

      <IonContent>
        <Form handleSubmit={mode === "add" ? submitAdd : submitEdit} initialState={type === "animal" ? animal : coral} />
      </IonContent>
    </IonPage>
  )
}
