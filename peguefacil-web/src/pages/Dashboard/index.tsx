import { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Grid,
  Typography,
  useTheme,
  Skeleton,
} from '@mui/material';
import {
  AccountBalance as AccountBalanceIcon,
  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon,
  SwapHoriz as SwapHorizIcon,
} from '@mui/icons-material';
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  BarChart,
  Bar,
  Legend,
  PieChart,
  Pie,
  Cell,
} from 'recharts';

interface DashboardData {
  totalTransactions: number;
  totalBalance: number;
  totalInflow: number;
  totalOutflow: number;
  chartData: {
    date: string;
    inflow: number;
    outflow: number;
  }[];
}

const mockData: DashboardData = {
  totalTransactions: 156,
  totalBalance: 25000.0,
  totalInflow: 35000.0,
  totalOutflow: 10000.0,
  chartData: [
    {
      date: '01/03',
      inflow: 5000,
      outflow: 2000,
    },
    {
      date: '08/03',
      inflow: 8000,
      outflow: 3000,
    },
    {
      date: '15/03',
      inflow: 12000,
      outflow: 2500,
    },
    {
      date: '22/03',
      inflow: 10000,
      outflow: 2500,
    },
  ],
};

const mockTransactionData = [
  { date: '2024-01', value: 65000 },
  { date: '2024-02', value: 78000 },
  { date: '2024-03', value: 89000 },
  { date: '2024-04', value: 95000 },
  { date: '2024-05', value: 102000 },
  { date: '2024-06', value: 115000 },
  { date: '2024-07', value: 125430 },
];

const mockBalanceData = [
  { month: 'Jan', entrada: 45000, saida: 32000 },
  { month: 'Fev', entrada: 52000, saida: 38000 },
  { month: 'Mar', entrada: 61000, saida: 42000 },
  { month: 'Abr', entrada: 58000, saida: 45000 },
  { month: 'Mai', entrada: 72000, saida: 48000 },
  { month: 'Jun', entrada: 85000, saida: 52000 },
  { month: 'Jul', entrada: 91110, saida: 55680 },
];

const mockTransactionTypeData = [
  { name: 'Depósito', value: 35 },
  { name: 'Transferência', value: 45 },
  { name: 'Pagamento', value: 15 },
  { name: 'Saque', value: 5 },
];

const COLORS = ['#2563eb', '#4f46e5', '#16a34a', '#dc2626'];

const Dashboard = () => {
  const theme = useTheme();
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<DashboardData>(mockData);

  useEffect(() => {
    // Implementar chamada à API
  }, []);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  const DashboardCard = ({
    title,
    value,
    icon: Icon,
    trend,
    trendValue,
    color,
  }: {
    title: string;
    value: number;
    icon: React.ElementType;
    trend: 'up' | 'down';
    trendValue: number;
    color: string;
  }) => (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Box
            sx={{
              backgroundColor: `${color}15`,
              borderRadius: 2,
              p: 1,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            <Icon sx={{ color }} />
          </Box>
          <Typography
            variant="subtitle2"
            color="text.secondary"
            sx={{ ml: 1, flexGrow: 1 }}
          >
            {title}
          </Typography>
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              color: trend === 'up' ? 'success.main' : 'error.main',
            }}
          >
            {trend === 'up' ? <TrendingUpIcon /> : <TrendingDownIcon />}
            <Typography variant="caption" sx={{ ml: 0.5 }}>
              {trendValue}%
            </Typography>
          </Box>
        </Box>
        {loading ? (
          <Skeleton variant="text" width={120} height={40} />
        ) : (
          <Typography variant="h4" sx={{ fontWeight: 600 }}>
            {formatCurrency(value)}
          </Typography>
        )}
      </CardContent>
    </Card>
  );

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>

      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Total de Transações
              </Typography>
              <Typography variant="h5">
                {data.totalTransactions}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Saldo Total
              </Typography>
              <Typography variant="h5">
                {formatCurrency(data.totalBalance)}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Total de Entradas
              </Typography>
              <Typography variant="h5" sx={{ color: theme.palette.success.main }}>
                {formatCurrency(data.totalInflow)}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Total de Saídas
              </Typography>
              <Typography variant="h5" sx={{ color: theme.palette.error.main }}>
                {formatCurrency(data.totalOutflow)}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Fluxo de Caixa
          </Typography>
          <Box sx={{ height: 400 }}>
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart
                data={data.chartData}
                margin={{
                  top: 10,
                  right: 30,
                  left: 0,
                  bottom: 0,
                }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" />
                <YAxis />
                <Tooltip
                  formatter={(value: number) => formatCurrency(value)}
                />
                <Area
                  type="monotone"
                  dataKey="inflow"
                  stackId="1"
                  stroke={theme.palette.success.main}
                  fill={theme.palette.success.light}
                  name="Entradas"
                />
                <Area
                  type="monotone"
                  dataKey="outflow"
                  stackId="1"
                  stroke={theme.palette.error.main}
                  fill={theme.palette.error.light}
                  name="Saídas"
                />
              </AreaChart>
            </ResponsiveContainer>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default Dashboard; 