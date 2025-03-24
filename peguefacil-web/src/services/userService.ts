import { User, UserFormData } from '../types/user';

class UserService {
  private mockUsers: User[] = [
    {
      id: '1',
      name: 'João Silva',
      email: 'joao.silva@example.com',
      phoneNumber: '(11) 99999-9999',
      document: '123.456.789-00',
      type: 'personal',
      status: 'active',
      createdAt: '2024-01-15T10:30:00',
      lastLogin: '2024-03-20T15:45:00',
    },
    {
      id: '2',
      name: 'Empresa XYZ',
      email: 'contato@empresaxyz.com',
      phoneNumber: '(11) 98888-8888',
      document: '12.345.678/0001-90',
      type: 'business',
      status: 'active',
      createdAt: '2024-02-01T14:20:00',
      lastLogin: '2024-03-19T11:30:00',
    },
    {
      id: '3',
      name: 'Maria Santos',
      email: 'maria.santos@example.com',
      phoneNumber: '(11) 97777-7777',
      document: '987.654.321-00',
      type: 'personal',
      status: 'blocked',
      createdAt: '2024-02-15T09:15:00',
      lastLogin: '2024-03-10T16:20:00',
    },
    {
      id: '4',
      name: 'Pedro Oliveira',
      email: 'pedro.oliveira@example.com',
      phoneNumber: '(11) 96666-6666',
      document: '456.789.123-00',
      type: 'personal',
      status: 'pending',
      createdAt: '2024-03-01T11:45:00',
      lastLogin: '2024-03-01T11:45:00',
    },
  ];

  async getUsers(): Promise<User[]> {
    return this.mockUsers;
  }

  async getUserById(id: string): Promise<User | null> {
    return this.mockUsers.find(user => user.id === id) || null;
  }

  async createUser(data: UserFormData): Promise<User> {
    const newUser: User = {
      id: Math.random().toString(36).substr(2, 9),
      name: data.name,
      email: data.email,
      phoneNumber: data.phone,
      document: data.document,
      type: data.type,
      status: 'active',
      createdAt: new Date().toISOString(),
      lastLogin: new Date().toISOString(),
    };

    this.mockUsers.push(newUser);
    return newUser;
  }

  async updateUser(id: string, data: UserFormData): Promise<User | null> {
    const index = this.mockUsers.findIndex(user => user.id === id);
    if (index === -1) return null;

    const updatedUser: User = {
      ...this.mockUsers[index],
      name: data.name,
      email: data.email,
      phoneNumber: data.phone,
      document: data.document,
      type: data.type,
    };

    this.mockUsers[index] = updatedUser;
    return updatedUser;
  }

  async deleteUser(id: string): Promise<boolean> {
    const index = this.mockUsers.findIndex(user => user.id === id);
    if (index === -1) return false;

    this.mockUsers.splice(index, 1);
    return true;
  }

  async toggleUserStatus(id: string): Promise<User | null> {
    const index = this.mockUsers.findIndex(user => user.id === id);
    if (index === -1) return null;

    const user = this.mockUsers[index];
    const updatedUser: User = {
      ...user,
      status: user.status === 'active' ? 'blocked' : 'active',
    };

    this.mockUsers[index] = updatedUser;
    return updatedUser;
  }
}

export const userService = new UserService();
export type { User, UserFormData }; 