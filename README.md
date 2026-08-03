<div align="center" >
  <img src="assets/BootcampAvanade.png" width="100"/>

 # Bootcamp Avanade - Back-end com .NET e IA
 
</div>

### 📌 Desafio Técnico - Microservices 
Para entender os requisitos e o contexto do projeto: 🔗 [Documentação do Desafio](./docs/EntendendoDesafio.md)

⚙️ **Status:** *concluído* ✅

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

## 🔐 Autenticação com JWT + RBAC & Zero-Trust

Assim como pedido no **Diagrama de Arquitetura** da  [Documentação do Desafio](./docs/EntendendoDesafio.md),
a Autenticação JWT foi implementada dentro do Gateway (Auth).

Para atender aos **Critérios de Aceitação** que exigem *"permissões específicas para cada ação"*, escolhi o **Modelo de Segurança RBAC** (Controle de Acesso Baseado em Papéis), estruturado com as seguintes *Roles*:

* `Cliente`: Role padrão para novos cadastros públicos (self-service).
* `Estoquista`: Acesso às operações do microsserviço de Estoque (`POST` e `GET`).
* `Gerente`: Permissões de criação/promoção de colaboradores e acesso completo aos Microsserviços.
* `Admin`: Restrito ao *Data Seeding* inicial para inicialização segura do ambiente.

* 🚫 **Evitando Over-engineering**⚙️🔀🌀
  * Optei por separar o `Auth` em **Pastas convencionais** ao invés de **Class Library**
    evitando complexidade desnecessária, sem fechar as portas para o futuro.

* 👁️‍🗨️ **Aplicando Encapsulamento**
  * Classe `Usuario` respeita os Princípios de Encapsulamento, com `private set` nas propriedades e 
    disponibiliza **Métodos** para alteração controlada.

  * Escolhi o `PasswordHasher` Nativo do .NET para a geração de **hash de senhas**, encapsulando-o diretamente no construtor e nos métodos da entidade `Usuario`, garantindo que nenhuma senha seja exposta ou persistida em **texto puro** no Banco de Dados.

### 🕵️‍♂️ **Modelo de Segurança Híbrido com Ocelot**
Para não deixar os Microsserviços expostos a qualquer ataque, aplicamos a estratégia de **Defesa em Profundidade** com duas camadas de proteção:

* 🛡️ **Segurança de Borda (Gateway — Autenticação)**
   * Funciona como a **primeira barreira de segurança**. 
   * Intercepta a requisição e valida a integridade do token JWT. Se o token for inválido, a requisição é barrada antes de tocar os Microsserviços.

* 🔐 **Rede Interna (Microsserviços — Autorização)**
   * Implementa a filosofia **Zero-Trust** ("Nunca confie, sempre verifique").
   * Os Microsserviços de *Estoque* e *Vendas* não confiam cegamente no Gateway. Eles revalidam o token e aplicam as regras de **RBAC (Role-Based Access Control)**, garantindo que apenas usuários com as *Roles/permissões* corretas executem ações específicas.

#### ⏭️ Fluxo de Registro 🪪
  * **Cadastro Público (Self-Service)**
    * Registro aberto com definição automática da *Role* padrão de Clientes.
  * **Cadastro Interno (Backoffice) restrito a Admin ou Gerente** 
    * `CadastroDeFuncionario`: Permite a criação de novos colaboradores, definindo Email, Senha e Role Específica (`Estoquista` / `Gerente`).
    * `PromoverFuncionario`: Método para alteração controlada de Role.

### 🥚 Dilema do "Ovo e da Galinha" 🐔

<div align="center" >
 
> "Se para criar um Gerente, eu preciso de um Gerente... Como o primeiro nasce?" 🤔
 
</div>

Depois de rir muito com essa frase e quebrar a cabeça pesquisando, optei por utilizar a combinação de **Variáveis de Ambiente** com **Data Seeding** garantindo Segurança de Verdade, Automação e Independência de Ambiente.

  * **Configuração Segura (`appsettings.json` / `Env`):**
    * `SeedUser`: Armazena o e-mail, senha e role padrão do usuário inicial (podendo ser sobrescrito via Variáveis de Ambiente em produção).
* **Automação Idempotente (`DbSeeder`):**
  * No *Auth/Data*, o método estático `SeedAdminAsync` consome os dados de seeding para criar o primeiro usuário de forma automática, verificando se ele já existe para evitar duplicações a cada inicialização da API.


## 💻 Tecnologias Utilizadas

* .NET 9.0 | ASP.NET Core
* Ocelot | SwaggerForOcelot
* JWT (JSON Web Token)
* RabbitMQ
* SQL Server | Entity Framework Core

