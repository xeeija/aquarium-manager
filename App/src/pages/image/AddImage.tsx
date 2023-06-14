import React, { useState } from 'react';
import { RouteComponentProps } from 'react-router';
import {
  IonHeader, IonToolbar, IonButtons, IonBackButton, IonTitle, IonContent, IonButton, IonImg, IonItem,
  IonInput, IonCard, IonIcon, IonSpinner, IonPage
} from '@ionic/react';
import { CameraSource, CameraOptions, CameraResultType } from '@capacitor/camera';
import { useSelector, useDispatch } from 'react-redux';
import { camera, cloudUpload } from 'ionicons/icons';
import { Capacitor, Plugins } from "@capacitor/core";
import { IConfig } from "../../services/rest/iconfig";
import { AquariumClient, FileParameter } from "../../services/rest/interface";
import config from "../../services/rest/server-config";
import { RootState } from "../../services/reducers";
import { executeDelayed } from "../../services/utils/async-helpers";
import { ThunkDispatch } from "redux-thunk";
import { PicturesResult, addPictureAction } from "../../services/actions/pictures";


interface RouteParams {
  poiId: string,
  tripId: string
}

const { Camera } = Plugins;


const cameraOptions: CameraOptions = {
  source: CameraSource.Prompt,
  resultType: CameraResultType.Uri
}

export const AddImage: React.FunctionComponent<RouteComponentProps<RouteParams>> = ({ match, history }) => {

  const currentAquarium = useSelector((s: RootState) => s.currentAquarium);
  const token = useSelector((s: RootState) => s.user.authenticationInformation!.token || '');
  const [imageUri, setImageUri] = useState('');
  const [error, setError] = useState<string>('');
  const [description, setDescription] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const dispatch = useDispatch();
  const thunkPicturesDispatch = dispatch as ThunkDispatch<RootState, null, PicturesResult>;

  const takePicture = async () => {
    const image = await Camera.getPhoto({
      quality: 90,
      allowEditing: true,
      resultType: CameraResultType.Uri
    });

    console.log(image);
    var imageUrl = image.webPath;
    setImageUri(imageUrl);

  };
  const performUpload = () => {
    setIsLoading(true);
    const accessheader = new IConfig();
    accessheader.setToken(token);
    // accessheader.getAuthorization()
    const aquariumClient = new AquariumClient(accessheader, config.host);

    return fetch(imageUri)
      .then(r => {
        //Get Image and Convert To Blob
        return r.blob()
      })
      .then(formaData => {
        console.log("formData", FormData)
        // TODO
        //Set File Parameters
        const formFile: FileParameter = {
          data: formaData,
          fileName: "test.png",
        }

        // thunkPicturesDispatch(addPictureAction(formFile))
        //Upload

      })
      //Forward to Home
      .then(() => executeDelayed(200, () => history.push('/home')))
      .catch(err => setError(err))
      .finally(() => setIsLoading(false));

  }

  const showImage = () => imageUri ? (
    <>
      <IonCard>
        <IonItem>
          <IonImg src={Capacitor.convertFileSrc(imageUri)} />
        </IonItem>
        <IonItem>
          <IonInput value={description} onIonChange={e => setDescription(e.detail.value!)} placeholder="add descr..." />
        </IonItem>
      </IonCard>
      <IonButton onClick={e => performUpload()}>
        {isLoading ? <IonSpinner /> : (<></>)}
        <IonIcon icon={cloudUpload} /> Upload</IonButton>
    </>
  ) : ''

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonBackButton defaultHref={`/trips/show/${match.params.tripId}`} />
          </IonButtons>
          <IonTitle>Add Image</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent>
        <IonButton onClick={takePicture} expand='block'><IonIcon icon={camera} />Add Image</IonButton>
        {showImage()}
      </IonContent>
    </IonPage>
  )
}