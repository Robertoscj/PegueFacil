export interface User {
  id: string;
  name: string;
  email: string;
  phoneNumber: string;
  document: string;
  type: 'personal' | 'business';
  status: 'active' | 'blocked' | 'pending';
  createdAt: string;
  lastLogin: string;
}

export interface UserFormData {
  name: string;
  email: string;
  phone: string;
  document: string;
  type: 'personal' | 'business';
} 