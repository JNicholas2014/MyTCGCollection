import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Game } from './game';

@Component({
  selector: 'app-games',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.css']
})

export class GamesComponent implements OnInit {
  public displayedColumns: string[] = ['id', 'gameName'];
  public games: MatTableDataSource<Game>;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 50;
  public defaultSortColumn: string = "gameName";
  public defaultSortOrder: string = "asc";

  defaultFilterColumn: string = "gameName";
  filterQuery: string = null;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  ngOnInit() {
    this.loadData(null);
  }
  loadData(query: string = null) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;
    if (query) {
      this.filterQuery = query;
    }
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    var url = this.baseUrl + 'api/Games';
    var params = new HttpParams().set("pageIndex", event.pageIndex.toString()).set("pageSize", event.pageSize.toString()).set("sortColumn", (this.sort) ? this.sort.active : this.defaultSortColumn).set("sortOrder", (this.sort) ? this.sort.direction : this.defaultSortOrder);

    if (this.filterQuery) {
      params = params.set("filterColumn", this.defaultFilterColumn).set("filterQuerty", this.filterQuery);
    }

    this.http.get<any>(url, { params }).subscribe(result => {
      this.paginator.length = result.totalCount;
      this.paginator.pageIndex = result.pageIndex;
      this.paginator.pageSize = result.pageSize;
      this.games = new MatTableDataSource<Game>(result.data);
    }, error => console.error(error));
  }
}
