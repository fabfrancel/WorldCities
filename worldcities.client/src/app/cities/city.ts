import { Country } from "../countries/country";

export interface City {
  id: number;
  name: string;
  lat: number;
  lon: number;
  countryId: number;
  country: [Country];
}
