import React from "react"
import { IonMenu, IonHeader, IonToolbar, IonTitle, IonContent, IonListHeader, IonNote, IonList, IonMenuToggle, IonItem, IonIcon, IonLabel } from "@ionic/react";
import { logOutOutline, logOutSharp, logInOutline, logInSharp, logIn, home, personAdd, listSharp } from "ionicons/icons";
import { useSelector, useDispatch, useStore } from "react-redux";
import { useLocation } from "react-router";
import { loggedOut } from "../services/actions/user";
import { RootState } from "../services/reducers";
import { isNotExpired } from "../services/rest/security-helper";

interface AppPage {
  url: string;
  iosIcon: string;
  mdIcon: string;
  title: string;
}

const appPages: AppPage[] = [
  {
    title: 'Home',
    url: '/home',
    iosIcon: home,
    mdIcon: home
  },
  {
    title: 'Register',
    url: '/register',
    iosIcon: personAdd,
    mdIcon: personAdd,
  },
];

var secureAppPage: AppPage[] = [

];

function AddMenu(item: AppPage) {
  if (secureAppPage.some(e => e.url === item.url) === false) {
    secureAppPage.push(item);
  }
}

const Menu: React.FC = () => {
  const location = useLocation();

  const { user, authenticationInformation: authentication } = useSelector((state: RootState) => state.user);

  const dispatch = useDispatch();
  const store = useStore();
  const token = "";
  var securityItem = null;

  if (isNotExpired(authentication)) {
    AddMenu({
      title: "Corals & Animals",
      url: "/items",
      iosIcon: listSharp,
      mdIcon: listSharp
    })

    securityItem = {
      title: 'Logout ' + user?.fullName,
      url: '/home',
      iosIcon: logOutOutline,
      mdIcon: logOutSharp,
      onClick: () => { dispatch(loggedOut()) }
    }

  }
  else {
    securityItem =
    {
      title: 'Login',
      url: '/login',
      iosIcon: logInOutline,
      mdIcon: logInSharp,
      onClick: (e: any) => { }
    }

    secureAppPage = [];
  }

  return (
    <IonMenu contentId="main" type="overlay">
      <IonHeader>
        <IonToolbar>
          <IonTitle>Menu</IonTitle>
        </IonToolbar>
      </IonHeader>
      <IonContent>
        <IonListHeader>Welcome</IonListHeader>
        <IonNote>{isNotExpired(authentication) ? 'Hello ' + user?.fullName : 'Not Logged in'}</IonNote>
        <IonList>
          {appPages.map((appPage, index) => {
            return (
              <IonMenuToggle key={index} autoHide={false}>
                <IonItem routerLink={appPage.url} routerDirection="none">
                  <IonIcon slot="start" ios={appPage.iosIcon} md={appPage.mdIcon} />
                  <IonLabel>{appPage.title}</IonLabel>
                </IonItem>
              </IonMenuToggle>
            );
          })}
          {secureAppPage.map((appPage, index) => {
            return (
              <IonMenuToggle key={index} autoHide={false}>
                <IonItem className={location.pathname === appPage.url ? 'selected' : ''} routerLink={appPage.url} routerDirection="none" lines="none" detail={false}>
                  <IonIcon slot="start" ios={appPage.iosIcon} md={appPage.mdIcon} />
                  <IonLabel>{appPage.title}</IonLabel>
                </IonItem>
              </IonMenuToggle>
            );
          })}
          <IonMenuToggle key={'sec2'} auto-hide="false">
            <IonItem routerLink={securityItem.url} lines="none" onClick={securityItem.onClick} >
              <IonIcon slot="start" ios={securityItem.iosIcon} md={securityItem.mdIcon} />
              <IonLabel>{securityItem.title}</IonLabel>
            </IonItem>
          </IonMenuToggle>
        </IonList>
      </IonContent>
    </IonMenu>
  );
};

export default Menu;