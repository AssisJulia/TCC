namespace TCC_SAMMI.Domain.DTOs.Jogo.Request
{
    public class AtualizarPontuacaoRequest
    {
        public Guid JogoId { get; set; }
        public int Pontuacao { get; set; }
        
    }
}
