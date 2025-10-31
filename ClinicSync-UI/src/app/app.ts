import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
 
import { Footer } from "./shared/layout/footer/footer";
import { Navbar } from './shared/layout/navbar/navbar';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, Footer],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'ClinicSync-UI';
}
