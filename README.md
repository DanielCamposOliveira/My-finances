[MyFinances — Gestão Financeira Inteligente.md](https://github.com/user-attachments/files/31670487/MyFinances.Gestao.Financeira.Inteligente.md)
# My-finances

Sistema completo para controle e previsão de finanças pessoais, integrando uma interface reativa em **Angular** a uma API REST construída em **.NET (Minimal APIs)** e banco de dados **PostgreSQL**.

### 📌 O Problema

O controle financeiro tradicional por planilhas ou cadernos sofre frequentemente com:
- Falta de clareza sobre parcelamentos futuros e impacto de compras a longo prazo.
- Dificuldade de gerenciar despesas fixas recorrentes com valores variáveis mês a mês (como energia e água).
- Ausência de métricas visuais de consolidação anual (saldo x dívidas).
- Risco de inadimplência por falta de rastreio de parcelas abertas ou atrasadas.

### 💡 A Solução (Casos de Uso)

O **MyFinances** automatiza e centraliza o fluxo financeiro através dos seguintes fluxos principais:

- **Autenticação & Segurança de Dados:** Cadastro e login de usuários via interface dedicada, com comunicação protegida por tokens **JWT** em todos os endpoints sensíveis.
- **Gestão de Lançamentos Parcelados:** Criação de despesas/receitas divididas, com cálculo automático e projeção de vencimentos mensais consecutivos.
- **Automação de Contas Recorrentes (Contas Fixas):** Contratos de despesas recorrentes que geram faturas mensais _on-demand_, permitindo o ajuste fino do valor de cada fatura sem alterar a regra base.
- **Rastreabilidade por Categorias e Tags:** Associação $N:N$ entre lançamentos/contas e marcadores dinâmicos (ex: _Alimentação_, _Pix_, _Transporte_) para filtragem ágil.
- **Cockpit Financeiro & Consolidação Anual:** Dashboard analítico com visão de saldo atual, previsões a receber, despesas pendentes/atrasadas e gráfico de curva anual de receitas vs. despesas.

### 🛠️ Stack Tecnológica

|**Camada**|**Tecnologia**|**Destaques de Implementação**|
|---|---|---|
|**Frontend**|Angular|SPA componentizada, formulários reativos, modais dinâmicos, Dark/Light Mode.|
|**Backend**|.NET (Minimal APIs)|C#, Arquitetura em camadas (Endpoints, Services, Repositories), Injeção de Dependência.|
|**Persistência**|PostgreSQL + EF Core|Modelagem relacional, tabelas associativas ($N:N$), consultas otimizadas com `AsNoTracking`.|
|**Segurança**|Autenticação JWT|Proteção de rotas, DTOs imutáveis via C# Records, sanitização de contratos.|

## 🏛️ Padrão Arquitetural e Fluxo de Execução

A aplicação divide estritamente as responsabilidades em 4 camadas principais:

```
[ Requisição HTTP ] (Bearer JWT)
         │
         ▼
 1. Camada de Endpoints (Minimal APIs)
    ↳ Roteamento, autorização, binding de DTOs e padronização de respostas HTTP.
         │
         ▼
 2. Camada de Serviços (Service Layer)
    ↳ Regras de negócio, cálculos matemáticos, validações e orquestração de domínio.
         │
         ▼
 3. Camada de Repositórios (Repository Pattern)
    ↳ Consultas LINQ com AsNoTracking(), persistência e isolamento do ORM.
         │
         ▼
 4. Camada de Acesso a Dados (AppDbContext & EF Core)
    ↳ Mapeamento relacional com Fluent API, chaves estrangeiras e persistência no PostgreSQL.
```

## 💡 Destaques de Engenharia e Regras de Domínio

1. **Desacoplamento por DTOs (Data Transfer Objects):**
    
    - Uso de C# `record` types para assegurar contratos de entrada e saída imutáveis.
        
    - Entidades de domínio e tabelas do banco nunca são expostas diretamente para o cliente.
        
2. **Cálculo e Geração Automática de Parcelamento:**
    
    - Ao cadastrar um `Lançamento`, a camada de serviço calcula automaticamente o valor fracionado (`ValorTotal / QtdParcelas`) e persiste todas as parcelas com seus respectivos vencimentos mensais sequenciais.
        
3. **Automação On-Demand para Contas Fixas:**
    
    - Contas recorrentes (como aluguel e energia) não sobrecarregam a base com registros infinitos com antecedência.
        
    - A fatura do mês é gerada dinamicamente sob demanda a partir do `ValorBase`, ajustando automaticamente o dia de vencimento em meses mais curtos (ex: 28/29 de fevereiro).
        
4. **Respostas Padronizadas (HTTP Status Codes):**
    
    - Retornos explícitos utilizando `Results.Ok()`, `Results.Created()`, `Results.BadRequest()` e `Results.Problem()`.


### Login

![[Pasted image 20260831225155.png | 900]]

### Parcelas

![[Pasted image 20260831225253.png | 800]]

### Cadastro Conta Fixa

![[Pasted image 20260831225344.png | 800]]

### Cadastro Categoria

![[Pasted image 20260831225434.png | 800]]
