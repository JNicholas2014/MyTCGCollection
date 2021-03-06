import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Card } from './card';

@Component({
  selector: 'app-cards',
  templateUrl: './cards.component.html',
  styleUrls: ['./cards.component.css']
})
export class CardsComponent implements OnInit {
  public displayedColumns: string[] = ['cardNumber', 'cardName', 'cardRarity', 'quantity', 'cardValue', 'expansion'];
  public cards: MatTableDataSource<Card>;

  defaultPageIndex: number = 0;
  defaultPageSize: number = 50;
  public defaultSortColumn: string = "expansion";
  public defaultSortOrder: string = "asc";

  defaultFilterColumn: string = "expansion";
  filterQuery: string = null;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  ngOnInit() {
    this.loadData();
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
    var url = this.baseUrl + 'api/Cards';
    var params = new HttpParams().set("pageIndex", event.pageIndex.toString()).set("pageSize", event.pageSize.toString()).set("sortColumn", (this.sort) ? this.sort.active : this.defaultSortColumn).set("sortOrder", (this.sort) ? this.sort.direction : this.defaultSortOrder);

    if (this.filterQuery) { params = params.set("filterColumn", this.defaultFilterColumn).set("filterQuery", this.filterQuery);}
    this.http.get<any>(url, { params }).subscribe(result => {
      console.log(result);
      this.paginator.length = result.totalCount;
      this.paginator.pageIndex = result.pageIndex;
      this.paginator.pageSize = result.pageSize;
      this.cards = new MatTableDataSource<Card>(result.data);
    }, error => console.error(error));
  }
}
