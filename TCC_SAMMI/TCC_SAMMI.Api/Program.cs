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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});
builder.Services.AddDbContext<TCC_SAMMIContext>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthenticateSwagger();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

#region Endpoints de Usu�rios

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
        operation.Description = "Endpoint para obter todos os usu�rios cadastrados";
        operation.Summary = "Listar todos os Usu�rios";
        return operation;
    })
    .WithTags("Usu�rios")
    .RequireAuthorization();

app.MapGet("/usuario/{usuarioId}", (TCC_SAMMIContext context, Guid usuarioId) =>
{
    var usuario = context.UsuariosSet.Find(usuarioId);
    if (usuario is null)
        return Results.BadRequest("Usu�rio n�o Localizado.");

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
        operation.Description = "Endpoint para obter um usu�rio com base no ID informado";
        operation.Summary = "Obter um Usu�rio";
        operation.Parameters[0].Description = "Id do Usu�rio";
        return operation;
    })
    .WithTags("Usu�rios")
    .RequireAuthorization();

app.MapPost("/usuario", (TCC_SAMMIContext context, UsuarioAdicionarRequest usuarioAdicionarRequest) =>
{
    try
    {
        if (usuarioAdicionarRequest.EmailLogin != usuarioAdicionarRequest.EmailLoginConfirmacao)
            return Results.BadRequest("Email de Login n�o Confere.");

        if (usuarioAdicionarRequest.Senha != usuarioAdicionarRequest.SenhaConfirmacao)
            return Results.BadRequest("Senha n�o Confere.");

        if (context.UsuariosSet.Any(p => p.EmailLogin == usuarioAdicionarRequest.EmailLogin))
            return Results.BadRequest("Email j� utilizado para Login em outro Usu�rio.");

        var usuario = new Usuario(
            usuarioAdicionarRequest.Nome,
            usuarioAdicionarRequest.EmailLogin,
            usuarioAdicionarRequest.Senha.EncryptPassword());

        context.UsuariosSet.Add(usuario);
        context.SaveChanges();

        return Results.Created("Created", $"Usu�rio Registrado com Sucesso. {usuario.Id}");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Cadastrar um Usu�rio";
        operation.Summary = "Novo Usu�rio";
        return operation;
    })
    .WithTags("Usu�rios")
    .RequireAuthorization();

app.MapPut("/usuario/alterar-senha", (TCC_SAMMIContext context, UsuarioAtualizarRequest usuarioAtualizarRequest) =>
{
    try
    {
        var usuario = context.UsuariosSet.Find(usuarioAtualizarRequest.Id);
        if (usuario is null)
            return Results.BadRequest("Usu�rio n�o Localizado");

        if (usuarioAtualizarRequest.SenhaAtual.EncryptPassword() == usuario.Senha)
        {
            usuario.AlterarSenha(usuarioAtualizarRequest.SenhaNova.EncryptPassword());
            context.UsuariosSet.Update(usuario);
            context.SaveChanges();

            return Results.Ok("Senha Altera com Sucesso.");
        }

        return Results.BadRequest("Ocorreu um Problema ao Alterar a Senha do Usu�rio.");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
    }
})
    .WithOpenApi(operation =>
    {
        operation.Description = "Endpoint para Alterar a Senha do Usu�rio";
        operation.Summary = "Alterar Senha";
        return operation;
    })
    .WithTags("Usu�rios")
    .RequireAuthorization();

#endregion

#region Autentica��o

app.MapPost("/autenticar", (TCC_SAMMIContext context, UsuarioAutenticarRequest usuarioAutenticarRequest) =>
    {
        var usuario = context.UsuariosSet.FirstOrDefault(p => p.EmailLogin == usuarioAutenticarRequest.EmailLogin && p.Senha == usuarioAutenticarRequest.Senha.EncryptPassword());
        if (usuario is null)
            return Results.BadRequest("N�o foi Poss�vel Efetuar o Login.");

        var claims = new[]
        {
            new Claim("UsuarioId", usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome)
        };

        //Recebe uma inst�ncia da Classe SymmetricSecurityKey
        //armazenando a chave de criptografia usada na cria��o do Token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{ccdc511d-23f0-4a30-995e-ebc3658e901d}"));

        //Recebe um objeto do tipo SigninCredentials contendo a chave de
        //criptografia e o algoritimo de seguran�a empregados na gera��o
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
        operation.Description = "Endpoint para Autenticar um Usu�rio na API";
        operation.Summary = "Autenticar Usu�rio";
        return operation;
    })
    .WithTags("Seguran�a");

#endregion

app.Run();