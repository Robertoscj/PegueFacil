# PegueFácil Web

Frontend da aplicação PegueFácil, desenvolvido com React, TypeScript, Material-UI e Vite.

## Requisitos

- Node.js 18.x ou superior
- npm 9.x ou superior

## Instalação

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/peguefacil-web.git
cd peguefacil-web
```

2. Instale as dependências:
```bash
npm install
```

3. Configure as variáveis de ambiente:
- Copie o arquivo `.env.development` para `.env.development.local`
- Ajuste as variáveis conforme necessário

## Desenvolvimento

Para iniciar o servidor de desenvolvimento:

```bash
npm run dev
```

O servidor será iniciado em `http://localhost:5173`.

## Scripts Disponíveis

- `npm run dev`: Inicia o servidor de desenvolvimento
- `npm run build`: Gera a build de produção
- `npm run preview`: Visualiza a build de produção localmente
- `npm run lint`: Executa o linter
- `npm run format`: Formata o código com Prettier

## Estrutura do Projeto

```
src/
  ├── components/     # Componentes reutilizáveis
  ├── config/        # Configurações (axios, etc)
  ├── hooks/         # Hooks customizados
  ├── pages/         # Páginas da aplicação
  ├── routes/        # Configuração de rotas
  ├── services/      # Serviços de API
  ├── theme/         # Configuração do tema
  └── main.tsx       # Ponto de entrada
```

## Tecnologias Principais

- [React](https://reactjs.org/)
- [TypeScript](https://www.typescriptlang.org/)
- [Material-UI](https://mui.com/)
- [React Router](https://reactrouter.com/)
- [Axios](https://axios-http.com/)
- [Vite](https://vitejs.dev/)
- [ESLint](https://eslint.org/)
- [Prettier](https://prettier.io/) 