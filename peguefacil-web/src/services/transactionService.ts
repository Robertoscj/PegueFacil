import api from './api';

export enum TransactionType {
  Deposit = 'deposit',
  Withdrawal = 'withdrawal',
  Transfer = 'transfer',
  Payment = 'payment',
  Refund = 'refund',
}

export enum TransactionStatus {
  Pending = 'pending',
  Completed = 'completed',
  Failed = 'failed',
  Cancelled = 'cancelled',
}

export interface Transaction {
  id: string;
  senderId: string;
  receiverId?: string;
  amount: number;
  type: TransactionType;
  status: TransactionStatus;
  description: string;
  fee: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateDepositRequest {
  userId: string;
  amount: number;
  description: string;
}

export interface CreateWithdrawalRequest {
  userId: string;
  amount: number;
  description: string;
}

export interface CreateTransferRequest {
  senderId: string;
  receiverId: string;
  amount: number;
  description: string;
}

export interface CreatePaymentRequest {
  payerId: string;
  merchantId: string;
  amount: number;
  description: string;
}

export interface CreateRefundRequest {
  originalTransactionId: string;
  reason: string;
}

export interface CancelTransactionRequest {
  reason: string;
}

const transactionService = {
  getById: async (id: string) => {
    const response = await api.get<{ data: Transaction }>(`/transaction/${id}`);
    return response.data.data;
  },

  getUserTransactions: async (userId: string, startDate?: Date, endDate?: Date) => {
    let url = `/transaction/user/${userId}`;
    if (startDate || endDate) {
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate.toISOString());
      if (endDate) params.append('endDate', endDate.toISOString());
      url += `?${params.toString()}`;
    }
    const response = await api.get<{ data: Transaction[] }>(url);
    return response.data.data;
  },

  getByStatus: async (status: TransactionStatus) => {
    const response = await api.get<{ data: Transaction[] }>(`/transaction/status/${status}`);
    return response.data.data;
  },

  getByType: async (type: TransactionType) => {
    const response = await api.get<{ data: Transaction[] }>(`/transaction/type/${type}`);
    return response.data.data;
  },

  createDeposit: async (request: CreateDepositRequest) => {
    const response = await api.post<Transaction>('/transaction/deposit', request);
    return response.data;
  },

  createWithdrawal: async (request: CreateWithdrawalRequest) => {
    const response = await api.post<Transaction>('/transaction/withdrawal', request);
    return response.data;
  },

  createTransfer: async (request: CreateTransferRequest) => {
    const response = await api.post<Transaction>('/transaction/transfer', request);
    return response.data;
  },

  createPayment: async (request: CreatePaymentRequest) => {
    const response = await api.post<Transaction>('/transaction/payment', request);
    return response.data;
  },

  createRefund: async (request: CreateRefundRequest) => {
    const response = await api.post<Transaction>('/transaction/refund', request);
    return response.data;
  },

  process: async (id: string) => {
    const response = await api.post<{ data: boolean }>(`/transaction/${id}/process`);
    return response.data.data;
  },

  cancel: async (id: string, request: CancelTransactionRequest) => {
    const response = await api.post<{ data: boolean }>(`/transaction/${id}/cancel`, request);
    return response.data.data;
  },
};

export default transactionService; 