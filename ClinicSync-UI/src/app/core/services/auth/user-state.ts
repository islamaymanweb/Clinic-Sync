import { Injectable } from '@angular/core';
import { UserInfo } from '../../../shared/models/user';
import { CookieService } from 'ngx-cookie-service';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserState {
   private readonly currentUserSubject = new BehaviorSubject<UserInfo | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private readonly USER_KEY = 'healthsync_current_user';

  constructor(private cookieService: CookieService) {
    this.loadUserFromStorage();
  }

  /**
   * تعيين المستخدم الحالي
   */
  setCurrentUser(user: UserInfo): void {
    this.currentUserSubject.next(user);
    this.saveUserToStorage(user);
  }

  /**
   * مسح المستخدم الحالي
   */
  clearCurrentUser(): void {
    this.currentUserSubject.next(null);
    this.removeUserFromStorage();
  }

  /**
   * الحصول على المستخدم الحالي
   */
  getCurrentUser(): UserInfo | null {
    return this.currentUserSubject.value;
  }

  /**
   * التحقق من صلاحية الدور
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  /**
   * التحقق من وجود أي من الأدوار
   */
  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    return user ? roles.includes(user.role) : false;
  }

  /**
   * تحميل المستخدم من التخزين
   */
  private loadUserFromStorage(): void {
    try {
      const userData = this.cookieService.get(this.USER_KEY);
      if (userData) {
        const user = JSON.parse(userData) as UserInfo;
        this.currentUserSubject.next(user);
      }
    } catch (error) {
      console.error('Error loading user from storage:', error);
      this.removeUserFromStorage();
    }
  }

  /**
   * حفظ المستخدم في التخزين
   */
  private saveUserToStorage(user: UserInfo): void {
    try {
      const userData = JSON.stringify(user);
      this.cookieService.set(this.USER_KEY, userData, 1); // صلاحية يوم واحد
    } catch (error) {
      console.error('Error saving user to storage:', error);
    }
  }

  /**
   * إزالة المستخدم من التخزين
   */
  private removeUserFromStorage(): void {
    this.cookieService.delete(this.USER_KEY);
  }
}