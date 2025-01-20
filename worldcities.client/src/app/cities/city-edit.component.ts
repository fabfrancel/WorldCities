import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { City } from './city';
import { Country } from '../countries/country';

@Component({
    selector: 'app-city-edit',
    templateUrl: './city-edit.component.html',
    styleUrl: './city-edit.component.scss',
    standalone: false
})
export class CityEditComponent implements OnInit {
  // the view title
  public title?: string;

  // the form model
  form!: FormGroup;

  // the city object to edit or create
  city!: City;

  // the city object id, as fetched from the active route:
  // It's NULL when we're adding a new city,
  // and not NULL when we're editing an existing one.
  id?: number;


  // the countries array for the select
  countries?: Country[];
  selectedCountryId: number = 0;

  baseUrl: string = `${environment.baseUrl}Cities/`;

  constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) { }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(4)]),
      lat: new FormControl('', [Validators.required, Validators.min(-90), Validators.max(90)]),
      lon: new FormControl('', [Validators.required, Validators.min(-180), Validators.max(180)]),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    this.loadData();
  }

  loadData() {

    this.loadCountries();

    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    // O snapshot é uma "imagem instantânea" do estado da rota no momento em que o componente foi inicializado.
    // snapshot.paramMap é uma abstração que fornece métodos de conveniência, como get, para acessar os parâmetros de rota.
    // paramMap é um Map que contém todos os parâmetros de rota.

    this.id = idParam ? +idParam : 0;
    // O operador + é usado para converter idParam de uma string para um número.

    if (this.id) {
      // fetch the city from the server
      var url = this.baseUrl + this.id;
      this.http.get<City>(url).subscribe({
        next: (result) => {
          this.city = result;
          this.title = `Edit - ${this.city.name}`;
          this.selectedCountryId = this.city.countryId;
          // update the form with the city value
          this.form.patchValue(this.city);
        },
        error: (error) => console.error(error)
      });
    }
    else {
      this.title = 'Create a new City';
    }
  }

loadCountries() {
    var url = `${environment.baseUrl}Countries`;

    var params = new HttpParams()
      .set('pageIndex', '0')
      .set('pageSize', '9999')
      .set('sortColumn', 'name');

    this.http.get<any>(url, { params }).subscribe({
      next: (result) => {
        this.countries = result.data;
      },
      error: (error) => console.error(error)
    });
  }


  onSubmit() {
    var city = this.city;

    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = this.form.controls['lat'].value;
      city.lon = this.form.controls['lon'].value;
      city.countryId = this.form.controls['countryId'].value;

      var url = this.baseUrl + city.id;

      this.http.put<City>(url, city).subscribe({
        next: (result) => {
          console.log(`City ${city!.id} has been updated`);
          this.goBack();
        },
        error: (error) => console.error(error)
      })
    }
    else {
      // create a new city with the form value
      city = <City>this.form.value;

      this.http.post<City>(this.baseUrl, city).subscribe({
        next: (result) => {
          console.log(`City ${result.id} has been created`);
          this.goBack();
        },
        error: (error) => console.error(error)
      });
    }
  }


  // go back to city view
  goBack() {
    this.router.navigate(['/cities']);
  }

  //  Return an AsyncValidatorFn than, in turn, returns an Observable: this means that is not returning a value,
  //  but a subscriber function instance tha will eventually return a value - which will be either a key / value object or null.
  //  This value will only be emitted when the observable is executed.
  //  The inner funcion create a temporary city object, fill it with the real-time form data,
  //  calls a IsDupeCity back-end URL and eventually returns either true or null, depending on the result.
  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> =>
    {
      var city = <City>{};

      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['contryId'].value;

      var url = environment.baseUrl + 'api/cities/IsDupeCity';

      // Instead subscribing to the HttpClient, we're manipulating it using the pipe and map RxJs operators
      return this.http.post<boolean>(url, city).pipe(map(result => {

        return (result ? { isDupeCity: true } : null)
      }));
    }
  }
}

