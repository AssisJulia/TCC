using TCC_SAMMI.Domain.Entities.Base;

namespace TCC_SAMMI.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; private set; }
    public string EmailLogin { get; private set; }
    public string Senha { get; private set; }

    protected Usuario() { }

    public Usuario(
        string nome, 
        string emailLogin, 
        string senha)
    {
        Nome = nome;
        EmailLogin = emailLogin;
        Senha = senha;
    }

    public void AlterarSenha(string senha)
    {
        Senha = senha;
    }
}