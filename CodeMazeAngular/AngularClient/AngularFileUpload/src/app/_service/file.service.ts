import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private http: HttpClient) { }

  private url = "https://localhost:5001/api/upload";

  public upload(formData: FormData) {
    return this.http.post(this.url + "/multipleupload", formData, {
      reportProgress: true,
      observe: 'events'
    })
  }

  public download(fileUrl: string) {
    return this.http.get(this.url + '/download?fileUrl=' + fileUrl, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob'
    })
  }

  public getPhotos() {
    return this.http.get(this.url + "/GetPhotosFromFile")
  }
}
