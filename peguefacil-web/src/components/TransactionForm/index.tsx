import { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  InputAdornment,
  Grid,
  Typography,
} from '@mui/material';
import { TransactionType } from '../../services/transactionService';
import useNotification from '../../hooks/useNotification';

interface TransactionFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: TransactionFormData) => Promise<void>;
  type: TransactionType;
  users?: { id: string; name: string }[];
}

export interface TransactionFormData {
  amount: number;
  description: string;
  userId?: string;
  senderId?: string;
  receiverId?: string;
  merchantId?: string;
}

const TransactionForm = ({ open, onClose, onSubmit, type, users = [] }: TransactionFormProps) => {
  const { showError } = useNotification();
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<TransactionFormData>({
    amount: 0,
    description: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.amount <= 0) {
      showError('O valor deve ser maior que zero');
      return;
    }

    try {
      setLoading(true);
      await onSubmit(formData);
      onClose();
    } catch (error) {
      showError('Erro ao processar transação');
    } finally {
      setLoading(false);
    }
  };

  const getTitle = () => {
    switch (type) {
      case TransactionType.Deposit:
        return 'Novo Depósito';
      case TransactionType.Withdrawal:
        return 'Novo Saque';
      case TransactionType.Transfer:
        return 'Nova Transferência';
      case TransactionType.Payment:
        return 'Novo Pagamento';
      default:
        return 'Nova Transação';
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>{getTitle()}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            {type === TransactionType.Transfer && (
              <>
                <Grid item xs={12}>
                  <FormControl fullWidth>
                    <InputLabel>Remetente</InputLabel>
                    <Select
                      value={formData.senderId || ''}
                      onChange={(e) => setFormData({ ...formData, senderId: e.target.value })}
                      required
                      label="Remetente"
                    >
                      {users.map((user) => (
                        <MenuItem key={user.id} value={user.id}>
                          {user.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12}>
                  <FormControl fullWidth>
                    <InputLabel>Destinatário</InputLabel>
                    <Select
                      value={formData.receiverId || ''}
                      onChange={(e) => setFormData({ ...formData, receiverId: e.target.value })}
                      required
                      label="Destinatário"
                    >
                      {users
                        .filter((user) => user.id !== formData.senderId)
                        .map((user) => (
                          <MenuItem key={user.id} value={user.id}>
                            {user.name}
                          </MenuItem>
                        ))}
                    </Select>
                  </FormControl>
                </Grid>
              </>
            )}

            {(type === TransactionType.Deposit || type === TransactionType.Withdrawal) && (
              <Grid item xs={12}>
                <FormControl fullWidth>
                  <InputLabel>Usuário</InputLabel>
                  <Select
                    value={formData.userId || ''}
                    onChange={(e) => setFormData({ ...formData, userId: e.target.value })}
                    required
                    label="Usuário"
                  >
                    {users.map((user) => (
                      <MenuItem key={user.id} value={user.id}>
                        {user.name}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
            )}

            {type === TransactionType.Payment && (
              <>
                <Grid item xs={12}>
                  <FormControl fullWidth>
                    <InputLabel>Pagador</InputLabel>
                    <Select
                      value={formData.userId || ''}
                      onChange={(e) => setFormData({ ...formData, userId: e.target.value })}
                      required
                      label="Pagador"
                    >
                      {users.map((user) => (
                        <MenuItem key={user.id} value={user.id}>
                          {user.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={12}>
                  <FormControl fullWidth>
                    <InputLabel>Comerciante</InputLabel>
                    <Select
                      value={formData.merchantId || ''}
                      onChange={(e) => setFormData({ ...formData, merchantId: e.target.value })}
                      required
                      label="Comerciante"
                    >
                      {users.map((user) => (
                        <MenuItem key={user.id} value={user.id}>
                          {user.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                </Grid>
              </>
            )}

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Valor"
                type="number"
                required
                value={formData.amount}
                onChange={(e) => setFormData({ ...formData, amount: Number(e.target.value) })}
                InputProps={{
                  startAdornment: <InputAdornment position="start">R$</InputAdornment>,
                }}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Descrição"
                required
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                multiline
                rows={3}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} disabled={loading}>
            Cancelar
          </Button>
          <Button type="submit" variant="contained" disabled={loading}>
            {loading ? 'Processando...' : 'Confirmar'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default TransactionForm; 