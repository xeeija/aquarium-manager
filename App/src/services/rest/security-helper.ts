import { AppStorage } from "../utils/app-storage";
import { AuthenticationInformation } from "./interface";

const storage = new AppStorage();
//storage.//store.create();
export const loadUserData = async () => {
  const [user, authentication, aquarium] = await Promise.all([
    storage.get('user'),
    storage.get('authentication'),
    storage.get('aquarium')
  ])
  return {
    user: user ? JSON.parse(user) : null,
    authentication: authentication ? JSON.parse(authentication) : null,
    aquarium: aquarium ? JSON.parse(aquarium) : null
  }
}

export const clearUserData = async () => await Promise.all([
  storage.remove('user'),
  storage.remove('authentication'),
  storage.remove("aquarium"),
])

export const getUserInfo = async () => {
  // const [user, authentication, aquarium] = await Promise.all([
  const [user, authentication] = await Promise.all([
    storage.get('user'),
    storage.get('authentication'),
    // storage.get('aquarium'),
  ])

  // .then(([user, authentication]) => {
  if (user.value && authentication.value)
    return {
      user: JSON.parse(user.value),
      authentication: JSON.parse(authentication.value),
      // aquarium: JSON.parse(aquarium.value),
    }
  else throw new Error('Not logged in!')
}

export const isNotExpired = (token: AuthenticationInformation | null | undefined) => {
  if (!token || token.token === "") {
    return false;
  }
  const dec = token.expirationDate;
  if (dec !== undefined) {
    const future = new Date(dec * 1000);
    const now = new Date();
    if (future > now) {
      return true;
    } else {
      console.log("Token is null or expired");
      return false;
    }
  }
  return false;
}

// login(cred: LoginRequest , cancelToken?: CancelToken | undefined): Promise<ItemResponseModelOfUserResponse> {))
