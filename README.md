# api-ponto-venda
Api RESTful para o aplicativo de ponto de venda da Contosso S.A. Desenvolvida em c# utilizando o padrão WebApi.

Utiliza o entity framework 6 no formato code-first para conectar à uma base de dados MySQL.

# como contribuir
Para poder compilar o projeto e contribuir, você precisa seguir alguns passos importantes:

### Instale o MySQL
Baixe o instalador do site da Oracle: https://www.mysql.com/
 
Você pode nomear a instância como preferir. Ao definir a senha do usuário root, utilize 12345 para não precisar alterar o web.config da aplicação.

### Instale o Visual Studio
Baixe o instalador do site da Microsoft: https://visualstudio.microsoft.com/

Não há configuração específica para esta etapa.

### Crie o schema para o banco de dados
No MySQL WorkBench, conecte à sua instância local e crie um novo schema chamado ponto-venda.

### Crie a estrutura de dados no schema
Com o projeto aberto no Visual Studio, acesse o Package Manager Console (Tools > NuGet Package Manager > Package Manager Console) e digite o seguinte comando: 

```
update-database
```

Este comando irá criar toda a estrutura de dados no schema de forma automática utilizando as migrações do Entity Framework.

### Desenvolva!
Divirta-se! Seu ambiente está pronto para produzir.