import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class Environment {
 
  /**
   * الحصول على إعدادات البيئة الحالية
   */
  get currentEnvironment() {
    return environment;
  }

  /**
   * التحقق إذا كانت البيئة الحالية هي production
   */
  get isProduction(): boolean {
    return environment.production;
  }

 
  /**
   * الحصول على عنوان الـ API الأساسي
   */
  get apiUrl(): string {
    return environment.apiUrl;
  }

  /**
   * الحصول على اسم التطبيق
   */
  get appName(): string {
    return environment.appName;
  }

  /**
   * الحصول على إصدار التطبيق
   */
  get version(): string {
    return environment.version;
  }

  /**
   * الحصول على إعدادات المصادقة
   */
  get authSettings() {
    return environment.auth;
  }

  /**
   * الحصول على إعدادات التطبيق
   */
  get appSettings() {
    return environment.app;
  }

  /**
   * الحصول على نهايات الـ APIs
   */
  get endpoints() {
    return environment.endpoints;
  }

  /**
   * بناء عنوان URL كامل للـ API
   */
  buildApiUrl(endpoint: string): string {
    return `${this.apiUrl}${endpoint}`;
  }

  /**
   * الحصول على اللغة الافتراضية
   */
  get defaultLanguage(): string {
    return this.appSettings.defaultLanguage;
  }

  /**
   * الحصول على اللغات المدعومة
   */
  get supportedLanguages(): string[] {
    return this.appSettings.supportedLanguages;
  }
}