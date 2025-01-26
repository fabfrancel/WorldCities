import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { environment } from '../../environments/environment';
import { Country } from './country';

@Component({
    selector: 'app-countries',
    templateUrl: './countries.component.html',
    styleUrl: './countries.component.scss',
    standalone: false
})
export class CountriesComponent implements OnInit {

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  public countries!: MatTableDataSource<Country>;

  displayedColumns = ['id', 'name', 'iso2', 'iso3'];

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  defaultSortColumn: string = "name";
  defaultSortOrder: "asc" | "desc" = "asc"
  defaultFilterColumn: string = "name";
  filterQuery?: string;

  constructor(private http: HttpClient) { }

  ngOnInit(){
    this.loadData();
  }

  loadData(query?: string) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {

    var url = environment.baseUrl + 'Countries';

    var params = new HttpParams()
      .set("pageIndex", event.pageIndex.toString())
      .set("pageSize", event.pageSize.toString())
      .set("sortColumn", this.sort ? this.sort.active : this.defaultSortColumn)
      .set("sortOrder", this.sort ? this.sort.direction : this.defaultSortOrder);

    if (this.filterQuery) {
      params = params
        .set("filterColumn", this.defaultFilterColumn)
        .set("filterQuery", this.filterQuery);
    }

    this.http.get<any>(url, { params }).subscribe({
      next: (result) => {
        this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.countries = new MatTableDataSource<Country>(result.data);
      },
      error: (error) => console.error(error)
    });
  }

}
