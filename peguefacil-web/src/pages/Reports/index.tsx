import { useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  Grid,
  Typography,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  useTheme,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
} from '@mui/material';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  LineChart,
  Line,
  PieChart,
  Pie,
  Cell,
} from 'recharts';

interface TransactionData {
  date: string;
  deposits: number;
  withdrawals: number;
  transfers: number;
  total: number;
}

interface TransactionTypeData {
  type: string;
  value: number;
}

const mockTransactionData: TransactionData[] = [
  {
    date: '01/03/2024',
    deposits: 50000,
    withdrawals: 20000,
    transfers: 15000,
    total: 85000,
  },
  {
    date: '02/03/2024',
    deposits: 45000,
    withdrawals: 25000,
    transfers: 18000,
    total: 88000,
  },
  {
    date: '03/03/2024',
    deposits: 55000,
    withdrawals: 22000,
    transfers: 20000,
    total: 97000,
  },
  {
    date: '04/03/2024',
    deposits: 48000,
    withdrawals: 28000,
    transfers: 16000,
    total: 92000,
  },
  {
    date: '05/03/2024',
    deposits: 52000,
    withdrawals: 24000,
    transfers: 19000,
    total: 95000,
  },
];

const mockTransactionTypes: TransactionTypeData[] = [
  { type: 'Depósitos', value: 250000 },
  { type: 'Saques', value: 119000 },
  { type: 'Transferências', value: 88000 },
];

const COLORS = ['#0088FE', '#00C49F', '#FFBB28'];

export default function Reports() {
  const theme = useTheme();
  const [period, setPeriod] = useState('week');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const handleChangePage = (event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  return (
    <Box>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" fontWeight="500" color="text.primary">
          Relatórios
        </Typography>
      </Box>

      <Box sx={{ mb: 3 }}>
        <FormControl size="small" sx={{ minWidth: 200 }}>
          <InputLabel>Período</InputLabel>
          <Select value={period} label="Período" onChange={(e) => setPeriod(e.target.value)}>
            <MenuItem value="day">Último dia</MenuItem>
            <MenuItem value="week">Última semana</MenuItem>
            <MenuItem value="month">Último mês</MenuItem>
            <MenuItem value="year">Último ano</MenuItem>
          </Select>
        </FormControl>
      </Box>

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Volume de Transações
              </Typography>
              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={mockTransactionData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="date" />
                    <YAxis />
                    <Tooltip formatter={(value) => formatCurrency(Number(value))} />
                    <Legend />
                    <Bar dataKey="deposits" name="Depósitos" fill="#0088FE" />
                    <Bar dataKey="withdrawals" name="Saques" fill="#00C49F" />
                    <Bar dataKey="transfers" name="Transferências" fill="#FFBB28" />
                  </BarChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Distribuição por Tipo
              </Typography>
              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={mockTransactionTypes}
                      dataKey="value"
                      nameKey="type"
                      cx="50%"
                      cy="50%"
                      outerRadius={80}
                      label={(entry) => `${entry.type}: ${formatCurrency(entry.value)}`}
                    >
                      {mockTransactionTypes.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value) => formatCurrency(Number(value))} />
                    <Legend />
                  </PieChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Evolução do Volume Total
              </Typography>
              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={mockTransactionData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="date" />
                    <YAxis />
                    <Tooltip formatter={(value) => formatCurrency(Number(value))} />
                    <Legend />
                    <Line
                      type="monotone"
                      dataKey="total"
                      name="Volume Total"
                      stroke={theme.palette.primary.main}
                      strokeWidth={2}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Detalhamento de Transações
              </Typography>
              <TableContainer component={Paper}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Data</TableCell>
                      <TableCell align="right">Depósitos</TableCell>
                      <TableCell align="right">Saques</TableCell>
                      <TableCell align="right">Transferências</TableCell>
                      <TableCell align="right">Total</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {mockTransactionData.map((row) => (
                      <TableRow key={row.date} hover>
                        <TableCell>{row.date}</TableCell>
                        <TableCell align="right">{formatCurrency(row.deposits)}</TableCell>
                        <TableCell align="right">{formatCurrency(row.withdrawals)}</TableCell>
                        <TableCell align="right">{formatCurrency(row.transfers)}</TableCell>
                        <TableCell align="right">{formatCurrency(row.total)}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
                <TablePagination
                  component="div"
                  count={mockTransactionData.length}
                  page={page}
                  onPageChange={handleChangePage}
                  rowsPerPage={rowsPerPage}
                  onRowsPerPageChange={handleChangeRowsPerPage}
                  labelRowsPerPage="Linhas por página:"
                  labelDisplayedRows={({ from, to, count }) =>
                    `${from}-${to} de ${count !== -1 ? count : `mais de ${to}`}`
                  }
                />
              </TableContainer>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
} 