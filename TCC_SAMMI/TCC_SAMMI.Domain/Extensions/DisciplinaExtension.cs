using TCC_SAMMI.Domain.Enumerators;

namespace TCC_SAMMI.Domain.Extensions
{
    public static class DisciplinaExtension
    {
        public static TipoJogo[] JogosDisponiveis(this Disciplina disciplina)
        {
            switch (disciplina)
            {
                case Disciplina.Portugues:
                    return [
                        TipoJogo.Generico
                    ];
                case Disciplina.Matematica:
                    return [
                        TipoJogo.Adicao,
                        TipoJogo.Subtracao,
                        TipoJogo.Multiplicacao,
                        TipoJogo.Divisao
                    ];
                default: throw new Exception("Disciplina não mapeada");
            }
        }
    }
}
