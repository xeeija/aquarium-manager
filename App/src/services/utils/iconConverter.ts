import { alarm, car, close, fish, flash, flower, power, shieldCheckmark, sunnySharp, water } from "ionicons/icons"

export const IconConverter = (icon: string) => {
  switch (icon) {
    case "Coral": return flower;
    case "Animal": return fish;
    case "Temperature": return sunnySharp
    case "Climate": return sunnySharp
    case "Electrcity": return flash
    case "Car": return car
    case "Volt": return power
    case "Current": return power
    case "Check": return shieldCheckmark
    case "Alarm": return alarm
    case "OnOff": return close
    default: return water
  }
}