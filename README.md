# InventorySales.Microservices

## 🚧 Projeto em Evolução 🚧
#### Microserviço desenvolvido no desafio técnico da Avanade.

🏗 **Arquitetura estruturada com separação por camadas:**

 - API

 - Application

 - Domain

 - Infrastructure

🎯 **Objetivo:** consolidar conceitos de Domain-Driven Design (DDD) e arquitetura limpa, aplicando boas práticas de organização e desacoplamento.

#### ⚙️ Status: em refatoração e aprimoramento contínuo 🔄

## 🏗 Arquitetura e Padrões

O projeto foi estruturado seguindo os princípios de **DDD (Domain-Driven Design)**, promovendo clara separação de responsabilidades entre as camadas.

Foi aplicado o **Repository Pattern** para abstração da persistência, centralizando operações e evitando replicação de código.

A aplicação utiliza **Injeção de Dependência** com controle de ciclo de vida **Scoped**, garantindo uma instância por requisição HTTP.

A **Inversão de Controle (IoC)** é utilizada para centralizar o gerenciamento das dependências no container nativo do ASP.NET Core, promovendo desacoplamento entre as camadas.

## 🔀 API Gateway (Ocelot)

Implementado para centralizar o acesso aos microserviços de Estoque e Vendas, atuando como ponto único de entrada e roteamento das requisições.

## 📩 Mensageria com RabbitMQ (Em evolução)

Estrutura inicial configurada utilizando RabbitMQ para viabilizar comunicação assíncrona baseada em eventos entre os microserviços.
Atualmente em fase de aprimoramento.

## 💻 Tecnologias Utilizadas

 - .NET

 - ASP.NET Core

 - Entity Framework Core

 - Microsoft SQL Server

 - API Gateway (Ocelot)

 - RabbitMQ

 - Swagger
