import { Injectable } from '@angular/core';
import { UserInfo } from '../../../shared/models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserState {
   private readonly currentUserSubject = new BehaviorSubject<UserInfo | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private readonly USER_KEY = 'healthsync_current_user';

  constructor() {
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
   * تحميل المستخدم من التخزين - استخدام localStorage
   */
  private loadUserFromStorage(): void {
    try {
      const userData = localStorage.getItem(this.USER_KEY);
      if (userData) {
        const user = JSON.parse(userData) as UserInfo;
        this.currentUserSubject.next(user);
        console.log('✅ User loaded from localStorage:', user.email);
      }
    } catch (error) {
      console.error('Error loading user from storage:', error);
      this.removeUserFromStorage();
    }
  }

  /**
   * حفظ المستخدم في التخزين - استخدام localStorage
   */
  private saveUserToStorage(user: UserInfo): void {
    try {
      const userData = JSON.stringify(user);
      localStorage.setItem(this.USER_KEY, userData);
      console.log('✅ User saved to localStorage:', user.email);
    } catch (error) {
      console.error('Error saving user to storage:', error);
    }
  }

  /**
   * إزالة المستخدم من التخزين
   */
  private removeUserFromStorage(): void {
    try {
      localStorage.removeItem(this.USER_KEY);
      console.log('✅ User removed from localStorage');
    } catch (error) {
      console.error('Error removing user from storage:', error);
    }
  }
}