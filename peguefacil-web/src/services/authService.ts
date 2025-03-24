interface LoginCredentials {
  email: string;
  password: string;
}

interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

class AuthService {
  private mockUser: User = {
    id: '1',
    name: 'Admin',
    email: 'admin@peguefacil.com',
    role: 'admin',
  };

  async login(credentials: LoginCredentials): Promise<User> {
    // Simular chamada à API
    await new Promise((resolve) => setTimeout(resolve, 1000));

    if (credentials.email === 'admin@peguefacil.com' && credentials.password === 'admin') {
      localStorage.setItem('user', JSON.stringify(this.mockUser));
      return this.mockUser;
    }

    throw new Error('Credenciais inválidas');
  }

  async logout(): Promise<void> {
    // Simular chamada à API
    await new Promise((resolve) => setTimeout(resolve, 500));
    localStorage.removeItem('user');
  }

  getCurrentUser(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  isAuthenticated(): boolean {
    return !!this.getCurrentUser();
  }
}

export const authService = new AuthService();
export type { User, LoginCredentials }; 