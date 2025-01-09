import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { environment } from '../../environments/environment';
import { City } from './city';
import { Country } from '../countries/country';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrl: './city-edit.component.scss'
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

  baseUrl: string =  `${environment.baseUrl}Cities/`;

  constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) { }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(),
      lat: new FormControl(),
      lon: new FormControl(),
      countryId: new FormControl()
    });
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
}

/*
  Função do !
  Evita Erros: O operador ! diz ao compilador do TypeScript para não considerar
  city como null ou undefined no momento da execução.

  Assunção de Segurança: O desenvolvedor está assumindo a responsabilidade de garantir que city realmente tem um valor e,
  portanto, city.id não causará um erro de referência nulo.

  Contexto de Uso
  Esse operador é útil em situações onde você tem certeza que uma variável não é null ou undefined,
  mas o TypeScript não consegue garantir isso por conta própria. No entanto, é importante usá-lo com cuidado,
  pois usá-lo indevidamente pode mascarar problemas no código.
*/ 
