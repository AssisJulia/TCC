using TCC_SAMMI.Domain.Enumerators;

namespace TCC_SAMMI.Domain.DTOs.Jogo.Response
{
    internal class JogoAgregadoResponse
    {
        public Guid JogoId { get; set; }
        public int Pontuacao { get; set; }
        public Disciplina Disciplina { get; set; }
        public TipoJogo TipoJogo { get; set; }
    }
}
