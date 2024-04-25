using System.Security.Claims;
using System.Text;
using TCC_SAMMI.CrossCutting;
using System.Text.Json.Serialization;
using TCC_SAMMI.Data.Context;
using TCC_SAMMI.Domain.DTOs.Usuario.Response;
using TCC_SAMMI.Domain.DTOs.Usuario.Request;
using TCC_SAMMI.Domain.Entities;
using TCC_SAMMI.Domain.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TCC_SAMMI.Domain.DTOs.Jogo.Request;
using TCC_SAMMI.Domain.DTOs.Jogo.Response;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});
builder.Services.AddDbContext<TCC_SAMMIContext>();
//builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthenticateSwagger();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

#region Endpoints de Usuários

app.MapGet("/usuario", (TCC_SAMMIContext context) =>
{
    var usuarios = context.UsuariosSet.Select(usuario => new UsuarioListarResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome
    });

    return Results.Ok(usuarios);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter todos os usuários cadastrados";
        operation.Summary = "Listar todos os Usuários";
        return operation;
    })
    .WithTags("Usuários")
    .RequireAuthorization();

app.MapGet("/usuario/{usuarioId}", (TCC_SAMMIContext context, Guid usuarioId) =>
{
    var usuario = context.UsuariosSet.Find(usuarioId);
    if (usuario is null)
        return Results.BadRequest("Usuário não Localizado.");

    var usuarioDto = new UsuarioObterResponse
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        EmailLogin = usuario.EmailLogin
    };

    return Results.Ok(usuarioDto);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter um usuário com base no ID informado";
        operation.Summary = "Obter um Usuário";
        operation.Parameters[0].Description = "Id do Usuário";
        return operation;
    })
    .WithTags("Usuários")
    .RequireAuthorization();

app.MapPost("/usuario", (TCC_SAMMIContext context, UsuarioAdicionarRequest usuarioAdicionarRequest) =>
{
    try
    {
        if (usuarioAdicionarRequest.EmailLogin != usuarioAdicionarRequest.EmailLoginConfirmacao)
            return Results.BadRequest("Email de Login não Confere.");

        if (usuarioAdicionarRequest.Senha != usuarioAdicionarRequest.SenhaConfirmacao)
            return Results.BadRequest("Senha não Confere.");

        if (context.UsuariosSet.Any(p => p.EmailLogin == usuarioAdicionarRequest.EmailLogin))
            return Results.BadRequest("Email já utilizado para Login em outro Usuário.");

        var usuario = new Usuario(
            usuarioAdicionarRequest.Nome,
            usuarioAdicionarRequest.EmailLogin,
            usuarioAdicionarRequest.Senha.EncryptPassword());

        context.UsuariosSet.Add(usuario);
        context.SaveChanges();

        return Results.Created("Created", $"Usuário Registrado com Sucesso. {usuario.Id}");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Cadastrar um Usuário";
        operation.Summary = "Novo Usuário";
        return operation;
    })
    .WithTags("Usuários");
//.RequireAuthorization();

app.MapPut("/usuario/alterar-senha", (TCC_SAMMIContext context, UsuarioAtualizarRequest usuarioAtualizarRequest) =>
{
    try
    {
        var usuario = context.UsuariosSet.Find(usuarioAtualizarRequest.Id);
        if (usuario is null)
            return Results.BadRequest("Usuário não Localizado");

        if (usuarioAtualizarRequest.SenhaAtual.EncryptPassword() == usuario.Senha)
        {
            usuario.AlterarSenha(usuarioAtualizarRequest.SenhaNova.EncryptPassword());
            context.UsuariosSet.Update(usuario);
            context.SaveChanges();

            return Results.Ok("Senha Alterada com Sucesso.");
        }

        return Results.BadRequest("Ocorreu um Problema ao Alterar a Senha do Usuário.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Alterar a Senha do Usuário";
        operation.Summary = "Alterar Senha";
        return operation;
    })
    .WithTags("Usuários")
    .RequireAuthorization();

#endregion

#region Endpoints de Jogo
app.MapPost("/jogo", (TCC_SAMMIContext context, CriarJogoRequest request) =>
    {
        try
        {
            //TODO: pegar o id do usuario do token e remover do request
            var jogo = new Jogo(request.Disciplina, request.TipoJogo, request.UsuarioId);

            context.JogoSet.Add(jogo);
            context.SaveChanges();

            return Results.Created("Created", $"Jogo Registrado com Sucesso. {jogo.Id}");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
).WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Iniciar um Jogo";
        operation.Summary = "Novo Jogo";
        return operation;
    }
).WithTags("Jogos")
.RequireAuthorization();

app.MapGet("/jogo", (TCC_SAMMIContext context) =>
{
    //TODO: obter token do usuario e filtrar pelos jogos relacionados a ele
    var jogos = context.JogoSet.Select(jogo => new JogoResponse
    {
        Data = jogo.Data,
        TipoJogo = jogo.TipoJogo,
        Disciplina = jogo.Disciplina,
        JogoId = jogo.Id,
        Pontuacao = jogo.Pontuacao
    });

    return Results.Ok(jogos);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint todas as partidas jogada pelo usuário";
        operation.Summary = "Listar jogos do usuário";
        return operation;
    })
    .WithTags("Jogos")
    .RequireAuthorization();

app.MapGet("/jogo/{jogoId}", (TCC_SAMMIContext context, Guid jogoId) =>
{
    var jogo = context.JogoSet.Find(jogoId);
    if (jogo is null)
        return Results.BadRequest("Jogo não Localizado.");

    var response = new JogoResponse
    {
        Data = jogo.Data,
        TipoJogo = jogo.TipoJogo,
        Disciplina = jogo.Disciplina,
        JogoId = jogo.Id,
        Pontuacao = jogo.Pontuacao
    };

    return Results.Ok(response);
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para obter um jogo do usuário com base no ID informado";
        operation.Summary = "Obter o jogo de um usuário";
        operation.Parameters[0].Description = "Id do Jogo";
        return operation;
    })
    .WithTags("Jogos")
    .RequireAuthorization();


#endregion

#region Autenticação

app.MapPost("/autenticar", (TCC_SAMMIContext context, UsuarioAutenticarRequest usuarioAutenticarRequest) =>
    {
        var usuario = context.UsuariosSet.FirstOrDefault(p => p.EmailLogin == usuarioAutenticarRequest.EmailLogin && p.Senha == usuarioAutenticarRequest.Senha.EncryptPassword());
        if (usuario is null)
            return Results.BadRequest("Não foi Possível Efetuar o Login.");

        var claims = new[]
        {
            new Claim("UsuarioId", usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome)
        };

        //Recebe uma instância da Classe SymmetricSecurityKey
        //armazenando a chave de criptografia usada na criação do Token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{ccdc511d-23f0-4a30-995e-ebc3658e901d}"));

        //Recebe um objeto do tipo SigninCredentials contendo a chave de
        //criptografia e o algoritimo de seguran�a empregados na geração
        //de assinaturas digitais para tokens
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "etec.pwIII",
            audience: "etec.pwIII",
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return Results.Ok(new
        {
            UsuarioId = usuario.Id,
            UsuarioNome = usuario.Nome,
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
        });
    })
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Autenticar um Usuário na API";
        operation.Summary = "Autenticar Usuário";
        return operation;
    })
    .WithTags("Segurança");

#endregion

app.Run();