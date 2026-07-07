# InventorySales.Microservices

### 📌 Desafio Técnico Avanade - Microservices 
Para entender os requisitos e o contexto do projeto: 🔗 [Documentação do Desafio](./docs/EntendendoDesafio.md)

⚙️ **Status:** *em refatoração e aprimoramento contínuo* 🔄

## 🏗 **Arquitetura Proposta:**

Optei por estruturar o projeto com **Arquitetura em Camadas com Domínio Isolado**, garantindo baixo acoplamento e melhor organização dos serviços, para atender a necessidade de *"separação clara entre as responsabilidades de Estoque e Vendas"*, conforme especificado no **Contexto de Negócio.**

A estrutura do projeto foi dividida nas seguintes camadas:

* API
* Application
* Domain
* Infrastructure

## ⚜ Padrões & Princípios

**Repository Pattern** para abstração da camada de persistência, garantindo desacoplamento e maior testabilidade.

**Injeção de Dependência** com controle de ciclo de vida **Scoped**, garantindo uma instância por requisição HTTP.

**Inversão de Controle (IoC)** é utilizada para centralizar o gerenciamento das dependências no container nativo do ASP.NET Core, promovendo desacoplamento entre as camadas.

## 🐯 API Gateway com Ocelot

Implementado como o ponto único de entrada da aplicação, sendo responsável por centralizar, expor e rotear de forma inteligente as requisições destinadas aos microsserviços de **Vendas** e **Estoque**.

 * **SwaggerForOcelot:** para Interface Gráfica do Gateway. Isso facilita o teste e a documentação das APIs de ponta a ponta sem a necessidade de abrir múltiplos painéis.

## 📩 Mensageria com RabbitMQ

Para rodar o RabbitMQ utilizo um **Container** no **Docker**, e acesso a interface através do Navegador.

### 🔄 Validação de Estoque pré-compra (Padrão RPC)
* Para atender à funcionalidade requerida que exige consistência forte (garantir o saldo do produto antes de fechar a venda), foi implementado o Padrão RPC.
 
  * O Microsserviço de Vendas adota um fluxo bloqueante (Request-Response) na camada de aplicação, aguardando o retorno do Microsserviço de Estoque para decidir se confirma ou aborta a operação.
 
### 📨 Notificação de Venda Confirmada
* Após a consolidação do pedido, o sistema utiliza o modelo Event-Driven para atualizar os demais serviços de forma assíncrona.
  * **Publisher (Vendas):** Transforma os dados do evento de venda em JSON, converte para um array de bytes e os publica na `pedido_criado_exchange`. Esse disparo ocorre logo após a persistência do pedido na camada de `Service`.
    
  * **Consumer (Estoque):** Escuta a fila vinculada à exchange. Ao receber os bytes, o consumer executa o método privado `ProcessarEventoAsync`, que abre um escopo temporário para injetar o repositório, realiza a baixa física de cada item no banco de dados e realiza o `BasicAck` para confirmar o processamento com sucesso.


## 💻 Tecnologias Utilizadas

 - SQL Server

 - Ocelot

 - RabbitMQ

 - .NET

 - ASP.NET Core

 - Entity Framework Core

