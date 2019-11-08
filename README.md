# Controle de Fluxo para Veículos
Sistema para controle do fluxo de entrada e saída de veículos por automatização.

## Ferramentas:

* Microcontrolador **Arduino** (_e componentes_) para capturar a foto da placa 
* Serviço API (_Inteligência Artificial_) para o reconhecimento da placa na imagem
* Sistema WEB em **ASP.NET Core** para o controle e análise dos dados
* Banco de dados **SQLServer** para armazenamento dos dados

Este protótipo fara uso de uma placa Arduino e do componente ESP32-CAM para obter as fotos da placa do Ônibus, após isso a imagem é enviada ao servidor e processada com o recurso de IA para reconhecimento de texto nas imagens capturadas, o servidor processa a placa e faz a tarifação automática. Além disso estará disponível análises e estatísticas de tais operações.

#### Links Utéis:
* [Serviço de Email](https://app-smtp.sendinblue.com/log/#151144f3-9b77-4dfb-aed6-954ed2a1ad7b)
* [Plate Recognition API](https://www.openalpr.com/cloud-api.html?gclid=EAIaIQobChMIk5Td7qrE5QIViYSRCh0Z7AJDEAAYASABEgLCoPD_BwE)

## API's:

#### Usuário
* ##### [POST] Add
  ###### URI: "/API/User/Add"

  #### Request:
  ##### Body:
  ```javascript 
      {
        Name: "Nome Exemplo",
        Registration: 123456,
        Email: "email@exemplo.com",
        Type: 1 // enum 0 = Transparência, 1 = Operador, 2 = Gerente, 3 = Administrador
      }
  ````
  #### Responses:
  
  Código | Retorno
  :-------:|----------
  201 | Criado com sucesso
  424 | Falha ao enviar email
  304 | Não cirado
  500 | Falha
  
* ##### [GET] Get
  ###### URI: "/API/User/Get/{id}"

  #### Request:
  ##### Query:
  ```javascript
      "/API/User/Get/10" // URI + id do usuário
  ````
  #### Responses:
  
  Código | Retorno | Retorno 2 |
  :-------:|-------|-----------|
  200 | { User } | _null_ |
  500 | Falha ao obter usuário | |
  
* ##### [GET] Load
  ###### URI: "/API/User/Load"

  #### Request:
  ##### Query:
  ```javascript
      "/API/User/Load" // URI
  ````
  
  #### Responses:
  
  Código | Retorno | Retorno 2 |
  :-------:|-------|-----------|
  200 | [{ User }, ...] | [ ] |
  500 | Falha ao carregar usuários | |
  
* ##### [PATCH] Change
  ###### URI: "/API/User/Change/{id}"

  #### Request:
  ##### Query:
  ```javascript
      "/API/User/Change/123" // URI + userId
  ````
  
  ##### Body:
  ```javascript
      {
        Name: "Nome Exemplo Atualizado",
        Registration: 654321,
        Email: "email@exemplo.com",
        Type: 1 // enum 0 = Transparência, 1 = Operador, 2 = Gerente, 3 = Administrador
      }
  ````

  #### Responses:

  Código | Retorno 
  :-------:|-------
  200 | Alterado com sucesso
  304 | Não alterado
  500 | Falha ao alterar usuário

* ##### [POST] GetToken
  ###### URI: "/API/User/GetToken"

  #### Request:
  ##### Query:
  ```javascript
  "abc23-1234-abcf-1234" // token
  ````
  
  #### Responses:
  
  Código | Retorno 
  :-------:|-------
  200 | { Token }
  406 | Token inválido
  500 | Falha ao obter token
  
* ##### [POST] DefinePassword
  ###### URI: "/API/User/DefinePassword/{token}"

  #### Request:
  ##### Query:
  ```javascript
      "/API/User/DefinePassword/abc23-1234-abcf-1234" // URI + token
  ````
  ##### Body:
  ```javascript
      "senha12345" // senha
  ````
  
  #### Responses:
  
  Código | Retorno 
  :-------:|-------
  202 | Senha definida
  406 | Token inválido
  500 | Falha ao definir senha

* ##### [DELETE] Remove
  ###### URI: "/API/User/Remove/{id}"
  
  #### Request:
  ##### Query:
  ```javascript
      "/API/User/Remove/123" // URI + userId
  ````
  
  #### Responses:

  Código | Retorno 
  :-------:|-------
  200 | Removido
  304 | Não Removido
  500 | Falha ao remover
