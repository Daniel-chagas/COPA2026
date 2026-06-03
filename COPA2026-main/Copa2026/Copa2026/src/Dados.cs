namespace Copa2026
{
    // ─── Estruturas ────────────────────────────────────────────────────────────

    struct Selecao
    {
        public int    Id;
        public string Nome;
        public string Grupo;   // A até L
        public bool   Ativo;
    }

    struct Estadio
    {
        public int    Id;
        public string Nome;
        public string Cidade;
        public string Pais;
        public int    Capacidade;
        public bool   Ativo;
    }

    struct Jogo
    {
        public int    Id;
        public string Fase;                  // Grupo, 32avos, Oitavas, Quartas, Semifinal, 3Lugar, Final
        public string Grupo;                 // A-L  (apenas fase de grupos)
        public string Data;                  // dd/MM/yyyy
        public int    IdEstadio;
        public int    IdTimeA;
        public int    IdTimeB;
        public int    GolsA;
        public int    GolsB;
        public bool   Realizado;
        public int    IdVencedorPenaltis;    // mata-mata empatado
        public bool   Ativo;
    }

    // ─── Repositório global ────────────────────────────────────────────────────

    static class Dados
    {
        public const int MAX_SELECOES = 48;
        public const int MAX_ESTADIOS = 16;
        public const int MAX_JOGOS    = 200;   // grupos + mata-mata

        public static Selecao[] selecoes = new Selecao[MAX_SELECOES];
        public static Estadio[] estadios = new Estadio[MAX_ESTADIOS];
        public static Jogo[]    jogos    = new Jogo[MAX_JOGOS];

        public static int totalSelecoes = 0;
        public static int totalEstadios = 0;
        public static int totalJogos    = 0;

        // Matriz de classificação: [índice seleção, coluna]
        // Col: 0=J  1=V  2=E  3=D  4=GP  5=GC  6=SG  7=PTS
        public static int[,] tabela = new int[MAX_SELECOES, 8];

        public static void InicializarVetores()
        {
            for (int i = 0; i < MAX_SELECOES; i++) selecoes[i] = new Selecao();
            for (int i = 0; i < MAX_ESTADIOS; i++) estadios[i] = new Estadio();
            for (int i = 0; i < MAX_JOGOS;    i++) jogos[i]    = new Jogo();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        public static int IndexSelecao(int id)
        {
            for (int i = 0; i < totalSelecoes; i++)
                if (selecoes[i].Id == id && selecoes[i].Ativo) return i;
            return -1;
        }

        public static int IndexEstadio(int id)
        {
            for (int i = 0; i < totalEstadios; i++)
                if (estadios[i].Id == id && estadios[i].Ativo) return i;
            return -1;
        }

        public static int IndexJogo(int id)
        {
            for (int i = 0; i < totalJogos; i++)
                if (jogos[i].Id == id && jogos[i].Ativo) return i;
            return -1;
        }

        public static int ProximoIdSelecao()
        {
            int max = 0;
            for (int i = 0; i < totalSelecoes; i++)
                if (selecoes[i].Id > max) max = selecoes[i].Id;
            return max + 1;
        }

        public static int ProximoIdEstadio()
        {
            int max = 0;
            for (int i = 0; i < totalEstadios; i++)
                if (estadios[i].Id > max) max = estadios[i].Id;
            return max + 1;
        }

        public static int ProximoIdJogo()
        {
            int max = 0;
            for (int i = 0; i < totalJogos; i++)
                if (jogos[i].Id > max) max = jogos[i].Id;
            return max + 1;
        }

        public static string NomeSelecao(int id)
        {
            int idx = IndexSelecao(id);
            return idx >= 0 ? selecoes[idx].Nome : $"ID {id}";
        }

        public static string NomeEstadio(int id)
        {
            int idx = IndexEstadio(id);
            return idx >= 0 ? estadios[idx].Nome : $"ID {id}";
        }

        public static bool GrupoValido(string g)
        {
            string[] grupos = { "A","B","C","D","E","F","G","H","I","J","K","L" };
            foreach (var gr in grupos)
                if (gr.Equals(g, System.StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static int SelecoesPorGrupo(string grupo)
        {
            int count = 0;
            for (int i = 0; i < totalSelecoes; i++)
                if (selecoes[i].Ativo &&
                    selecoes[i].Grupo.Equals(grupo, System.StringComparison.OrdinalIgnoreCase))
                    count++;
            return count;
        }
    }
}
