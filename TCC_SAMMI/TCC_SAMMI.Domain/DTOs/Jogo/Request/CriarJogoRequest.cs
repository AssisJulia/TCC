using TCC_SAMMI.Domain.Enumerators;

namespace TCC_SAMMI.Domain.DTOs.Jogo.Request
{
    public class CriarJogoRequest
    {
        public Disciplina Disciplina { get; set; }
        public TipoJogo TipoJogo { get; set; }
        public Guid UsuarioId { get; set; }
    }
}
