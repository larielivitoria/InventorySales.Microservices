# InventorySales.Microservices

### 📌 Desafio Técnico Avanade - Microservices 
Para entender os requisitos e o contexto do projeto: 🔗 [Documentação do Desafio](./docs/EntendendoDesafio.md)

## 🚧 Projeto em Evolução 🚧

🏗 **Arquitetura estruturada com separação por camadas:**

 - API

 - Application

 - Domain

 - Infrastructure

🎯 **Objetivo:** consolidar conceitos de **Arquitetura Limpa,** aplicando boas práticas de organização e desacoplamento, além de implementar comunicação assíncrona e segurança robusta.

#### ⚙️ Status: em refatoração e aprimoramento contínuo 🔄

## 🏗 Padrões & Princípios

**Repository Pattern** para abstração da camada de persistência, garantindo desacoplamento e maior testabilidade.

**Injeção de Dependência** com controle de ciclo de vida **Scoped**, garantindo uma instância por requisição HTTP.

**Inversão de Controle (IoC)** é utilizada para centralizar o gerenciamento das dependências no container nativo do ASP.NET Core, promovendo desacoplamento entre as camadas.

## 🔀 API Gateway (Ocelot)

Implementado para centralizar o acesso aos microserviços de Estoque e Vendas, atuando como ponto único de entrada e roteamento das requisições.

## 📩 Mensageria com RabbitMQ (Em evolução)

Estrutura inicial configurada utilizando RabbitMQ para viabilizar comunicação assíncrona baseada em eventos entre os microserviços.
Atualmente em fase de aprimoramento.

## 💻 Tecnologias Utilizadas

 - SQL Server

 - API Gateway (Ocelot)

 - RabbitMQ

 - .NET

 - ASP.NET Core

 - Entity Framework Core

 - Swagger
