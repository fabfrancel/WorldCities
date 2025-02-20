import { Component, Renderer2 } from '@angular/core';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrl: './nav-menu.component.scss',
    standalone: false
})
export class NavMenuComponent {
  currentTheme: string;

  constructor(private renderer: Renderer2) {
    this.currentTheme = 'light';
  }

  toggleTheme() {
    this.currentTheme = this.currentTheme === 'dark' ? 'light' : 'dark';
    this.renderer.setAttribute(document.body, 'data-bs-theme', this.currentTheme);
  }
}
