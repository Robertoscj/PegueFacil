import { useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Typography,
  IconButton,
  Chip,
  useTheme,
  Avatar,
  LinearProgress,
  Menu,
  MenuItem,
} from '@mui/material';
import {
  Add as AddIcon,
  MoreVert as MoreVertIcon,
  AccountBalance as AccountBalanceIcon,
  Block as BlockIcon,
  LockOpen as LockOpenIcon,
} from '@mui/icons-material';

interface Wallet {
  id: string;
  userId: string;
  userName: string;
  balance: number;
  isBlocked: boolean;
  lastTransaction: string;
  transactionCount: number;
}

const mockWallets: Wallet[] = [
  {
    id: '1',
    userId: 'user1',
    userName: 'João Silva',
    balance: 15000.0,
    isBlocked: false,
    lastTransaction: '2024-03-15',
    transactionCount: 45,
  },
  {
    id: '2',
    userId: 'user2',
    userName: 'Maria Santos',
    balance: 8500.0,
    isBlocked: true,
    lastTransaction: '2024-03-14',
    transactionCount: 32,
  },
  {
    id: '3',
    userId: 'user3',
    userName: 'Pedro Souza',
    balance: 25000.0,
    isBlocked: false,
    lastTransaction: '2024-03-15',
    transactionCount: 67,
  },
];

export default function Wallets() {
  const theme = useTheme();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedWallet, setSelectedWallet] = useState<string | null>(null);

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>, walletId: string) => {
    setAnchorEl(event.currentTarget);
    setSelectedWallet(walletId);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedWallet(null);
  };

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase();
  };

  return (
    <Box>
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" fontWeight="500" color="text.primary">
          Carteiras
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          sx={{ borderRadius: 2, textTransform: 'none' }}
        >
          Nova Carteira
        </Button>
      </Box>

      <Grid container spacing={3}>
        {mockWallets.map((wallet) => (
          <Grid item xs={12} sm={6} md={4} key={wallet.id}>
            <Card
              sx={{
                height: '100%',
                position: 'relative',
                '&:hover': {
                  boxShadow: theme.shadows[4],
                },
              }}
            >
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Avatar
                      sx={{
                        bgcolor: theme.palette.primary.main,
                        width: 48,
                        height: 48,
                      }}
                    >
                      {getInitials(wallet.userName)}
                    </Avatar>
                    <Box>
                      <Typography variant="h6" gutterBottom>
                        {wallet.userName}
                      </Typography>
                      <Chip
                        icon={wallet.isBlocked ? <BlockIcon /> : <AccountBalanceIcon />}
                        label={wallet.isBlocked ? 'Bloqueada' : 'Ativa'}
                        size="small"
                        color={wallet.isBlocked ? 'error' : 'success'}
                        sx={{ fontWeight: 500 }}
                      />
                    </Box>
                  </Box>
                  <IconButton
                    size="small"
                    onClick={(e) => handleMenuClick(e, wallet.id)}
                    aria-label="mais opções"
                  >
                    <MoreVertIcon />
                  </IconButton>
                </Box>

                <Box sx={{ mt: 3 }}>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Saldo Atual
                  </Typography>
                  <Typography variant="h5" color="text.primary" gutterBottom>
                    {new Intl.NumberFormat('pt-BR', {
                      style: 'currency',
                      currency: 'BRL',
                    }).format(wallet.balance)}
                  </Typography>
                </Box>

                <Box sx={{ mt: 2 }}>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Última Transação
                  </Typography>
                  <Typography variant="body1">
                    {new Date(wallet.lastTransaction).toLocaleDateString('pt-BR')}
                  </Typography>
                </Box>

                <Box sx={{ mt: 2 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Volume de Transações
                    </Typography>
                    <Typography variant="body2" color="primary">
                      {wallet.transactionCount}
                    </Typography>
                  </Box>
                  <LinearProgress
                    variant="determinate"
                    value={(wallet.transactionCount / 100) * 100}
                    sx={{ height: 6, borderRadius: 3 }}
                  />
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        onClick={handleMenuClose}
      >
        <MenuItem>Ver Detalhes</MenuItem>
        <MenuItem>Ver Transações</MenuItem>
        <MenuItem>
          {mockWallets.find((w) => w.id === selectedWallet)?.isBlocked ? (
            <>
              <LockOpenIcon fontSize="small" sx={{ mr: 1 }} />
              Desbloquear Carteira
            </>
          ) : (
            <>
              <BlockIcon fontSize="small" sx={{ mr: 1 }} />
              Bloquear Carteira
            </>
          )}
        </MenuItem>
      </Menu>
    </Box>
  );
} 