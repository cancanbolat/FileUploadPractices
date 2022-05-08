import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FileService } from './_service/file.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public photos: string[] = [];

  constructor(private http: HttpClient, private fileService: FileService) {

  }

  ngOnInit() {
    this.getPhotos();
  }

  private getPhotos() {
    this.fileService.getPhotos().subscribe(
      data => this.photos = data["photos"]
    );
  }

  public createImgPath = (serverPath: string) => {
    return `https://localhost:5001/${serverPath}`;
  }
}
