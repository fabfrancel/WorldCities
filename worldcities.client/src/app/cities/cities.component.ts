import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { CityDataSource } from './city-data-source';


@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss',
  standalone: false
})
export class CitiesComponent implements OnInit {

  /*
 *  @ViewChild
 *  é um decorador no Angular que permite acessar um componente filho,
 *  diretiva ou elemento DOM dentro de um componente pai.
 *  Ele é usado para obter uma referência a um elemento específico no template do componente pai,
 *  permitindo interagir com ele diretamente no código TypeScript.
 */
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  public dataSource!: CityDataSource;

  displayedColumns = ['id', 'name', 'lat', 'lon', 'country'];

  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  defaultSortColumn: string = "name";
  defaultSortOrder: "asc" | "desc" = "asc"
  defaultFilterColumn: string = "name";
  filterQuery?: string;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.dataSource = new CityDataSource(this.http)
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
    this.dataSource.loadCities(
      event.pageIndex,
      event.pageSize,
      this.sort ? this.sort.active : this.defaultFilterColumn,
      this.sort ? this.sort.direction : this.defaultSortOrder,
      this.filterQuery ? this.defaultFilterColumn : undefined,
      this.filterQuery
    );

    this.dataSource.totalItems$.subscribe(totalItems => {
      this.paginator.length = totalItems;
    });

    this.dataSource.pageIndex$.subscribe(pageIndex => {
      this.paginator.pageIndex = pageIndex;
    });

    this.dataSource.pageSize$.subscribe(pageSize => {
      this.paginator.pageSize = pageSize;
    });

  }

}


