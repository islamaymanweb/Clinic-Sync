import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class Cookie {
 set(name: string, value: string, days: number): void {
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = `expires=${date.toUTCString()}`;
    document.cookie = `${name}=${value}; ${expires}; path=/; SameSite=Strict; Secure`;
  }

  /**
   * الحصول على قيمة من الكوكيز
   */
  get(name: string): string | null {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    
    return null;
  }

  /**
   * حذف قيمة من الكوكيز
   */
  delete(name: string): void {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
  }

  /**
   * التحقق من وجود قيمة في الكوكيز
   */
  exists(name: string): boolean {
    return this.get(name) !== null;
  }
}