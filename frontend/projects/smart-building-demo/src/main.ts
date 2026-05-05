import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { environment } from './environments/environment';

if (!environment.production) {
  // Development-only: surface unhandled rejections in the console so they are
  // not silently swallowed during local development.
  window.addEventListener('unhandledrejection', (event) => {
    console.error('Unhandled promise rejection:', event.reason);
  });
}

bootstrapApplication(App, appConfig).catch((err) => console.error(err));
