import { fish, flower } from "ionicons/icons"

export const IconConverter = (icon: string) => {
  switch (icon) {
    case "Coral": return flower;
    case "Animal": return fish;
  }
}