namespace TCC_SAMMI.Domain.DTOs.Usuario.Request;

public class UsuarioAtualizarRequest
{
    public Guid Id { get; set; }
    public string SenhaAtual { get; set; }
    public string SenhaNova { get; set; }
}