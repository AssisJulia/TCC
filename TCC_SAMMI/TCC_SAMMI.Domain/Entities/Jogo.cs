using TCC_SAMMI.Domain.Entities.Base;
using TCC_SAMMI.Domain.Enumerators;

namespace TCC_SAMMI.Domain.Entities
{
    public class Jogo : EntityBase
    {
        public int Pontuacao { get; private set; }
        public DateTime Data { get; private set; }
        public Disciplina Disciplina { get; private set; }
        public TipoJogo TipoJogo { get; private set; }

        public Guid UsuarioId { get; private set; }
        public virtual Usuario Usuario { get; private set; }

        protected Jogo() { }

        public Jogo(Disciplina disciplina, TipoJogo tipoJogo, Guid usuarioId) : this(0, disciplina, tipoJogo, usuarioId)
        {

        }

        public Jogo(int pontuacao, Disciplina disciplina, TipoJogo tipoJogo, Guid usuarioId)
        {
            Pontuacao = pontuacao;
            Disciplina = disciplina;
            TipoJogo = tipoJogo;
            UsuarioId = usuarioId;
            Data = DateTime.Now;
        }

        public void AtualizaPontuacao(int novaPontuacao)
        {
            Pontuacao = novaPontuacao;
        }
    }
}
