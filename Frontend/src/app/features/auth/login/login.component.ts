import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';
  isLoading = false;
  showPassword = false;
  isAdminLogin = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      passkey: ['']
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  setLoginType(isAdmin: boolean) {
    this.isAdminLogin = isAdmin;
    if (isAdmin) {
      this.loginForm.get('passkey')?.setValidators([Validators.required]);
    } else {
      this.loginForm.get('passkey')?.clearValidators();
    }
    this.loginForm.get('passkey')?.updateValueAndValidity();
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      
      const loginPayload = {
        ...this.loginForm.value,
        isAdminLogin: this.isAdminLogin
      };

      this.authService.login(loginPayload).subscribe({
        next: () => {
          if (this.isAdminLogin) {
            this.router.navigate(['/dashboard/admin']);
          } else {
            this.router.navigate(['/dashboard']);
          }
        },
        error: (err) => {
          this.isLoading = false;
          if (err.status === 0) {
            this.errorMessage = 'Backend server is unreachable. Please ensure the microservices are running.';
          } else if (err.status === 401 || err.status === 400) {
            this.errorMessage = err.error?.message || 'Invalid email or password. Please try again.';
          } else {
            this.errorMessage = 'An unexpected error occurred. Please try again later.';
          }
        }
      });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }

  navigateToRegister() {
    this.router.navigate(['/register']);
  }
}
