import { FC, useEffect } from "react"
import { RouteComponentProps } from "react-router"
import { BuildForm, FieldDescriptionType, FormDescription } from "../../services/utils/form-builder";
import { Animal, Coral, ItemResponseModelOfAnimal } from "../../services/rest/interface";
import * as Validator from '../../services/utils/validators';
import { IonPage, IonHeader, IonToolbar, IonButtons, IonMenuButton, IonTitle, IonContent, IonToast } from "@ionic/react";
import { useDispatch, useSelector } from "react-redux";
import { executeDelayed } from "../../services/utils/async-helpers";
import { capitalize } from "../../services/utils/format";
import { ThunkDispatch } from "redux-thunk";
import { CoralResult, AnimalResult, fetchCoralAction, fetchAnimalAction, EditAnimalResult, editAnimalAction, editCoralAction, EditCoralResult, AddAnimalResult, addAnimalAction, addCoralAction, AddCoralResult } from "../../services/actions/item";
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
    position: 'floating', color: 'primary',
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
    position: 'floating', color: 'primary',
  },
  {
    name: 'deathDate', label: 'Death date', type: 'date',
    position: 'floating', color: 'primary',
  },
]

const coralFields: FieldDescriptionType<Readonly<Coral>>[] = [
  {
    name: 'coralType', label: 'Coral Type', type: 'select', options: [
      {
        key: "HardCoral",
        value: "Hard Coral"
      },
      {
        key: "SoftCoral",
        value: "Soft Coral"
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

  const { coral, animal, errorMessage } = useSelector((s: RootState) => s.item);
  const dispatch = useDispatch();

  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, CoralResult | AnimalResult>;
  const thunkDispatchEdit = dispatch as ThunkDispatch<RootState, null, EditAnimalResult | EditCoralResult>;
  const thunkDispatchAdd = dispatch as ThunkDispatch<RootState, null, AddAnimalResult | AddCoralResult>;

  useEffect(() => {
    if (mode === "add") {
      return
    }

    if (type === "animal") {
      thunkDispatch(fetchAnimalAction(match.params.id ?? "")).then(x => console.log(x));
    } else {
      thunkDispatch(fetchCoralAction(match.params.id ?? "")).then(x => console.log(x));
    }
  }, [match.params.id, thunkDispatch]);

  const submitAdd = async (itemData: Animal | Coral) => {
    dispatch(loading(true));

    const itemDispatch = type === "animal"
      ? thunkDispatchAdd(addAnimalAction(itemData))
      : thunkDispatchAdd(addCoralAction(itemData))

    itemDispatch
      .then(x => {
        console.log(x)
        executeDelayed(200, () => {
          const id = (x.payload as ItemResponseModelOfAnimal).data?.id
          if (id) {
            history.push(`/${type}/show/${id}`)
          }
        })
      })
      .catch((err) => {
        console.error(err)
      })
      .finally(() => dispatch(loading(false)));

  };

  const submitEdit = async (itemData: Animal | Coral) => {
    dispatch(loading(true));

    const itemDispatch = type === "animal"
      ? thunkDispatchEdit(editAnimalAction(itemData.id ?? "", itemData))
      : thunkDispatchEdit(editCoralAction(itemData.id ?? "", itemData))

    itemDispatch
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
        <Form handleSubmit={mode === "add" ? submitAdd : submitEdit} initialState={mode === "add" ? undefined : (type === "animal" ? animal : coral)} />
      </IonContent>

      <IonToast
        isOpen={errorMessage ? errorMessage.length > 0 : false}
        onDidDismiss={() => false}
        message={errorMessage}
        duration={5000}
        color='danger'
      />
    </IonPage>
  )
}
