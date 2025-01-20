import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { HttpClient, HttpParams } from '@angular/common/http';
import { City } from './city';
import { environment } from '../../environments/environment';

export class CityDataSource implements DataSource<City> {

  private citiesSubject = new BehaviorSubject<City[]>([]);
  private totalItemsSubject = new BehaviorSubject<number>(0);
  private pageIndexSubject = new BehaviorSubject<number>(0);
  private pageSizeSubject = new BehaviorSubject<number>(10);

  private loadingSubject = new BehaviorSubject<boolean>(false);

  public loading$ = this.loadingSubject.asObservable();
  public totalItems$ = this.totalItemsSubject.asObservable();
  public pageIndex$ = this.pageIndexSubject.asObservable();
  public pageSize$ = this.pageSizeSubject.asObservable();

  constructor(private http: HttpClient) { }

  loadCities(pageIndex: number, pageSize: number, sortColumn: string, sortOrder: string, filterColumn?: string, filterQuery?: string) {

    this.loadingSubject.next(true);

    let params = new HttpParams()
      .set("pageIndex", pageIndex.toString())
      .set("pageSize", pageSize.toString())
      .set("sortColumn", sortColumn)
      .set("sortOrder", sortOrder);

    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }

    this.http.get<any>(`${environment.baseUrl}Cities`, { params }).pipe(
      catchError((error) => {
        return of({ data: [], totalCount: 0 });
      }),
      finalize(() => this.loadingSubject.next(false))
    ).subscribe(result => {
      this.citiesSubject.next(result.data);
      this.totalItemsSubject.next(result.totalCount);
      this.pageIndexSubject.next(result.pageIndex);
      this.pageSizeSubject.next(result.pageSize);
    });
  }

  connect(collectionViewer: CollectionViewer): Observable<City[]> {
    return this.citiesSubject.asObservable();
  }

  disconnect(collectionViewer: CollectionViewer): void {
    this.citiesSubject.complete();
    this.loadingSubject.complete();
    this.totalItemsSubject.complete();
    this.pageIndexSubject.complete();
    this.pageSizeSubject.complete();
  }
}
