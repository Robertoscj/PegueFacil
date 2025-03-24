import api from './api';

export interface Wallet {
  id: string;
  userId: string;
  balance: number;
  isBlocked: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CheckLowBalanceRequest {
  threshold: number;
}

const walletService = {
  getAll: async () => {
    const response = await api.get<{ data: Wallet[] }>('/wallet');
    return response.data.data;
  },

  getById: async (id: string) => {
    const response = await api.get<{ data: Wallet }>(`/wallet/${id}`);
    return response.data.data;
  },

  getByUserId: async (userId: string) => {
    const response = await api.get<{ data: Wallet }>(`/wallet/user/${userId}`);
    return response.data.data;
  },

  getBalance: async (id: string) => {
    const response = await api.get<{ data: number }>(`/wallet/${id}/balance`);
    return response.data.data;
  },

  block: async (id: string) => {
    const response = await api.post<{ message: string }>(`/wallet/${id}/block`);
    return response.data;
  },

  unblock: async (id: string) => {
    const response = await api.post<{ message: string }>(`/wallet/${id}/unblock`);
    return response.data;
  },

  hasSufficientFunds: async (id: string, amount: number) => {
    const response = await api.get<{ data: boolean }>(`/wallet/${id}/has-funds?amount=${amount}`);
    return response.data.data;
  },

  checkLowBalance: async (id: string, request: CheckLowBalanceRequest) => {
    const response = await api.post<{ message: string }>(`/wallet/${id}/check-low-balance`, request);
    return response.data;
  },
};

export default walletService; 