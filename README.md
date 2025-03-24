# PegueFácil - Sistema de Gestão de Carteiras Digitais

## 📋 Sobre o Projeto

PegueFácil é uma aplicação web moderna para gestão de carteiras digitais, transações e usuários. O sistema oferece uma interface intuitiva e responsiva para gerenciar operações financeiras digitais de forma eficiente e segura.

## 🎯 Regras de Negócio

### Usuários
- Diferentes níveis de acesso: Administrador e Usuário comum
- Autenticação obrigatória para acesso ao sistema
- Gerenciamento de perfil e configurações pessoais
- Bloqueio/desbloqueio de contas por administradores

### Carteiras
- Cada usuário pode ter múltiplas carteiras
- Saldo atualizado em tempo real
- Histórico completo de transações
- Status da carteira: Ativa, Bloqueada, Em análise
- Limites de transação configuráveis

### Transações
- Tipos: Depósito, Saque, Transferência
- Validação de saldo antes da transação
- Registro de data, hora e participantes
- Status: Pendente, Concluída, Cancelada, Falha
- Notificações em tempo real

### Relatórios
- Geração de extratos por período
- Análise de fluxo de caixa
- Métricas de uso do sistema
- Exportação em diferentes formatos

## 🚀 Tecnologias Utilizadas

### Frontend
- **React 18** - Biblioteca JavaScript para construção de interfaces
- **TypeScript** - Superset JavaScript com tipagem estática
- **Vite** - Build tool e dev server
- **Material-UI (MUI)** - Framework de componentes React
- **React Router** - Roteamento e navegação
- **Notistack** - Sistema de notificações
- **Recharts** - Biblioteca de gráficos
- **Date-fns** - Manipulação de datas

### Gerenciamento de Estado
- **React Context API** - Gerenciamento de estado global
- **Custom Hooks** - Lógica reutilizável

### Estilização
- **Emotion** - CSS-in-JS
- **Material Icons** - Conjunto de ícones
- **Tema personalizado** - Design system próprio

## 🏗️ Estrutura do Projeto

```
peguefacil-web/
├── src/
│   ├── components/     # Componentes reutilizáveis
│   ├── contexts/      # Contextos React
│   ├── pages/         # Páginas da aplicação
│   ├── services/      # Serviços e APIs
│   ├── theme/         # Configuração do tema
│   ├── routes/        # Configuração de rotas
│   └── utils/         # Funções utilitárias
```

## 📦 Funcionalidades Implementadas

### Dashboard
- Visão geral do sistema
- Gráficos de transações
- Indicadores principais
- Atividades recentes

### Gestão de Usuários
- CRUD completo de usuários
- Atribuição de permissões
- Histórico de atividades
- Gestão de status

### Gestão de Carteiras
- Criação e edição de carteiras
- Monitoramento de saldo
- Histórico de transações
- Controle de status

### Transações
- Registro de operações
- Filtros avançados
- Validações em tempo real
- Histórico detalhado

### Sistema de Autenticação
- Login seguro
- Proteção de rotas
- Gerenciamento de sessão
- Recuperação de senha

## 🔧 Como Executar

1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/peguefacil.git
```

2. Instale as dependências
```bash
cd peguefacil-web
npm install
```

3. Execute o projeto
```bash
npm run dev
```

4. Acesse no navegador
```
http://localhost:5173
```

## 🔐 Credenciais de Teste

- **Email**: admin@peguefacil.com
- **Senha**: admin

## 🎨 Tema e Design

O sistema utiliza um tema personalizado com as cores da marca PegueFácil, garantindo uma experiência visual consistente e profissional. A interface é totalmente responsiva e adaptável a diferentes tamanhos de tela.

## 🔄 Próximas Atualizações

- [ ] Implementação de backend real
- [ ] Integração com APIs de pagamento
- [ ] Sistema de notificações por email
- [ ] Módulo de relatórios avançados
- [ ] Suporte a múltiplos idiomas

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes. 