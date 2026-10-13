# Regressões de gerenciamento e sessão

Execute `dotnet run --project tests/Adopetz.Api.RegressionTests` para verificar geração e validade de refresh tokens, rejeição de configurações ausentes ou inválidas, status HTTP de sessão expirada, limites de idade, isolamento de leitura e escrita por ONG, o filtro da consulta paginada e datas de vacinação. O runner encerra com erro se alguma verificação falhar.

Esses testes usam os validators, handlers e a consulta EF reais. Não conectam ao banco, não precisam de credenciais nem enviam fotos. A compilação da consulta EF é interceptada antes de abrir a conexão.
