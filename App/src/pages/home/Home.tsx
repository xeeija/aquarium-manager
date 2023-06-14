import {
  IonButtons,
  IonCard,
  IonCardContent,
  IonCardHeader,
  IonCardSubtitle,
  IonCardTitle,
  IonContent,
  IonHeader,
  IonMenuButton,
  IonPage,
  IonTitle,
  IonToolbar,
  IonItem, IonList, IonSpinner
} from '@ionic/react';
import React, { useEffect, useState } from 'react';
import './Home.css';
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../../services/reducers";
import { ThunkDispatch } from "redux-thunk";
import {
  AquariumClient,
  ItemResponseModelOfPictureResponse,
  PictureResponse
} from "../../services/rest/interface";
import { IConfig } from "../../services/rest/iconfig";
import config from "../../services/rest/server-config";
import { fetchPicturesActions, PicturesResult } from "../../services/actions/pictures";


const Home: React.FunctionComponent = () => {

  const { pictures, isLoading, errorMessage } = useSelector((s: RootState) => s.pictures);
  const currentAquarium = useSelector((s: RootState) => s.currentAquarium);
  const dispatch = useDispatch();
  const thunkDispatch = dispatch as ThunkDispatch<RootState, null, PicturesResult>;
  const token = useSelector((s: RootState) => s.user.authenticationInformation!.token || '');
  console.log("Current name: " + currentAquarium.name)

  const [imageURLs, setImageUrls] = useState({} as { [key: string]: string });

  useEffect(() => {

    dispatch(fetchPicturesActions.request());
    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);

    aquariumClient.pictureAll(currentAquarium!.name!)
      .then(pics => {
        console.log(pics);
        fetchImages(pics);
        dispatch(fetchPicturesActions.success(pics));
      })
      .catch(err => dispatch(fetchPicturesActions.failure(err)))

  }, [])

  const fetchImages = ((images: PictureResponse[]) =>
    Promise.all(images.map(i => getImageURL(i.picture?.id!)))
      .then(urls => {
        setImageUrls(images.reduce((acc, img, idx) => ({ ...acc, [img.picture?.id!]: urls[idx] }), {}))
      })
      .then(x => {
        console.log(imageURLs);
      }))

  const getImageURL = (imageId: string): Promise<string> => {

    const accessheader = new IConfig();
    accessheader.setToken(token);
    const aquariumClient = new AquariumClient(accessheader, config.host);

    const urlCreator = window.URL || (window as any).webkitURL;
    return aquariumClient.getPicture(currentAquarium!.name!, imageId)
      .then(x => {
        let y = x as ItemResponseModelOfPictureResponse;
        return y.data
      })
      .then(data => {
        const base64Data = data!.base64;
        const mimetype = data!.picture!.contentType;
        const base64Response = fetch(`data:${mimetype};base64,${base64Data}`);
        return base64Response;

      }).then(da => {
        const blob = da.blob();
        return blob;
      })
      .then(blob => urlCreator.createObjectURL(blob))
  }

  const showImage = (image: PictureResponse) => (
    <IonCard key={image.picture?.id!}>
      <img src={imageURLs[image.picture?.id!]} alt="" />
      <IonCardContent>
        <p>{image.picture?.description ? image.picture?.description : 'no description available'}</p>
        <p className='credits'>Uploaded at {new Date(image.picture?.uploaded!).toLocaleString()}</p>
      </IonCardContent>
    </IonCard>)

  const ListImages = () => {

    const items = (pictures ?? []).map(showImage);
    return (pictures?.length ?? 0) > 0 ? <IonList>{items}</IonList> : <NoValuesInfo />;
  };

  const NoValuesInfo = () => !isLoading && currentAquarium === null ?
    (<IonCard>
      <img src='assets/images/img.png' alt=""></img>
      <IonCardHeader>
        <IonCardTitle>Currently you do not have any Aquariums</IonCardTitle>
      </IonCardHeader>
    </IonCard>) : (<></>)

  const AquariumInfo = () => {
    return (
      <IonCard>
        <ListImages />
        <IonCardHeader>
          <IonCardTitle>{currentAquarium!.name} {currentAquarium!.liters}L</IonCardTitle>
          <IonCardSubtitle>{currentAquarium!.waterType}</IonCardSubtitle>
        </IonCardHeader>

        <IonCardContent>
          <p>Length: {currentAquarium!.length}cm</p>
          <p>Height: {currentAquarium!.height}cm</p>
          <p>Depth: {currentAquarium!.depth}cm</p>
        </IonCardContent>
      </IonCard>
    )
  }

  const AquariumPanel = () => currentAquarium !== null ? <AquariumInfo /> : <NoValuesInfo />

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonButtons slot="start">
            <IonMenuButton />
          </IonButtons>
          <IonTitle>Home</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent>
        <IonCard class="welcome-card">

          {isLoading ? <IonItem><IonSpinner />Loading Values...</IonItem> : <AquariumPanel />}

        </IonCard>
      </IonContent>
    </IonPage>
  );
};

export default Home;