using System;
using System.IO;
using System.Text;

namespace Copa2026
{
    static class CsvManager
    {
        static readonly string DIR = "csv";

        static string Caminho(string arquivo) => Path.Combine(DIR, arquivo);

        static void GarantirDiretorio()
        {
            if (!Directory.Exists(DIR)) Directory.CreateDirectory(DIR);
        }

        // ── SALVAR ────────────────────────────────────────────────────────────

        public static void SalvarSelecoes()
        {
            GarantirDiretorio();
            var sb = new StringBuilder();
            sb.AppendLine("id;nome;grupo;ativo");
            for (int i = 0; i < Dados.totalSelecoes; i++)
            {
                var s = Dados.selecoes[i];
                sb.AppendLine($"{s.Id};{s.Nome};{s.Grupo};{s.Ativo.ToString().ToLower()}");
            }
            File.WriteAllText(Caminho("selecoes.csv"), sb.ToString(), Encoding.UTF8);
        }

        public static void SalvarEstadios()
        {
            GarantirDiretorio();
            var sb = new StringBuilder();
            sb.AppendLine("id;nome;cidade;pais;capacidade;ativo");
            for (int i = 0; i < Dados.totalEstadios; i++)
            {
                var e = Dados.estadios[i];
                sb.AppendLine($"{e.Id};{e.Nome};{e.Cidade};{e.Pais};{e.Capacidade};{e.Ativo.ToString().ToLower()}");
            }
            File.WriteAllText(Caminho("estadios.csv"), sb.ToString(), Encoding.UTF8);
        }

        public static void SalvarJogos()
        {
            GarantirDiretorio();
            var sb = new StringBuilder();
            sb.AppendLine("id;fase;grupo;data;idEstadio;idTimeA;idTimeB;golsA;golsB;realizado;idVencedorPenaltis;ativo");
            for (int i = 0; i < Dados.totalJogos; i++)
            {
                var j = Dados.jogos[i];
                sb.AppendLine($"{j.Id};{j.Fase};{j.Grupo};{j.Data};{j.IdEstadio};{j.IdTimeA};{j.IdTimeB};" +
                              $"{j.GolsA};{j.GolsB};{j.Realizado.ToString().ToLower()};{j.IdVencedorPenaltis};{j.Ativo.ToString().ToLower()}");
            }
            File.WriteAllText(Caminho("jogos.csv"), sb.ToString(), Encoding.UTF8);
        }

        public static void SalvarClassificacao()
        {
            GarantirDiretorio();
            Classificacao.GerarClassificacao();
            var sb = new StringBuilder();
            sb.AppendLine("grupo;posicao;selecao;j;v;e;d;gp;gc;sg;pts");

            string[] grupos = { "A","B","C","D","E","F","G","H","I","J","K","L" };
            foreach (var g in grupos)
            {
                int[] ids = new int[4];
                int n = 0;
                for (int i = 0; i < Dados.totalSelecoes; i++)
                    if (Dados.selecoes[i].Ativo && Dados.selecoes[i].Grupo.Equals(g, StringComparison.OrdinalIgnoreCase))
                        if (n < 4) ids[n++] = i;

                for (int a = 0; a < n - 1; a++)
                    for (int b = a + 1; b < n; b++)
                        if (Classificacao.CompararSelecoes(ids[b], ids[a]) < 0)
                        { int tmp = ids[a]; ids[a] = ids[b]; ids[b] = tmp; }

                for (int p = 0; p < n; p++)
                {
                    int i = ids[p];
                    sb.AppendLine($"{g};{p + 1};{Dados.selecoes[i].Nome};" +
                                  $"{Dados.tabela[i,0]};{Dados.tabela[i,1]};{Dados.tabela[i,2]};" +
                                  $"{Dados.tabela[i,3]};{Dados.tabela[i,4]};{Dados.tabela[i,5]};" +
                                  $"{Dados.tabela[i,6]};{Dados.tabela[i,7]}");
                }
            }
            File.WriteAllText(Caminho("classificacao.csv"), sb.ToString(), Encoding.UTF8);
        }

        public static void SalvarMataMata()
        {
            GarantirDiretorio();
            var sb = new StringBuilder();
            sb.AppendLine("id;fase;data;estadio;timeA;golsA;golsB;timeB;vencedor;realizado");

            string[] fases = { "32avos","Oitavas","Quartas","Semifinal","3Lugar","Final" };
            foreach (var fase in fases)
            {
                for (int i = 0; i < Dados.totalJogos; i++)
                {
                    var j = Dados.jogos[i];
                    if (!j.Ativo || j.Fase != fase) continue;

                    string venc = "";
                    if (j.Realizado)
                    {
                        if (j.GolsA > j.GolsB) venc = Dados.NomeSelecao(j.IdTimeA);
                        else if (j.GolsB > j.GolsA) venc = Dados.NomeSelecao(j.IdTimeB);
                        else venc = Dados.NomeSelecao(j.IdVencedorPenaltis) + " (pen)";
                    }

                    sb.AppendLine($"{j.Id};{j.Fase};{j.Data};{Dados.NomeEstadio(j.IdEstadio)};" +
                                  $"{Dados.NomeSelecao(j.IdTimeA)};{j.GolsA};{j.GolsB};" +
                                  $"{Dados.NomeSelecao(j.IdTimeB)};{venc};{j.Realizado}");
                }
            }
            File.WriteAllText(Caminho("mata_mata.csv"), sb.ToString(), Encoding.UTF8);
        }

        public static void GerarRelatorioFinal()
        {
            GarantirDiretorio();
            var sb = new StringBuilder();
            sb.AppendLine("fase;grupo;data;estadio;timeA;golsA;golsB;timeB;vencedor");

            string[] todas = { "Grupo","32avos","Oitavas","Quartas","Semifinal","3Lugar","Final" };
            foreach (var fase in todas)
            {
                for (int i = 0; i < Dados.totalJogos; i++)
                {
                    var j = Dados.jogos[i];
                    if (!j.Ativo || j.Fase != fase || !j.Realizado) continue;

                    string venc;
                    if (j.GolsA > j.GolsB) venc = Dados.NomeSelecao(j.IdTimeA);
                    else if (j.GolsB > j.GolsA) venc = Dados.NomeSelecao(j.IdTimeB);
                    else if (j.IdVencedorPenaltis > 0) venc = Dados.NomeSelecao(j.IdVencedorPenaltis) + " (pen)";
                    else venc = "Empate";

                    sb.AppendLine($"{j.Fase};{j.Grupo};{j.Data};{Dados.NomeEstadio(j.IdEstadio)};" +
                                  $"{Dados.NomeSelecao(j.IdTimeA)};{j.GolsA};{j.GolsB};" +
                                  $"{Dados.NomeSelecao(j.IdTimeB)};{venc}");
                }
            }
            File.WriteAllText(Caminho("relatorio_final.csv"), sb.ToString(), Encoding.UTF8);
            UI.Sucesso("relatorio_final.csv gerado na pasta csv/.");
        }

        public static void SalvarTudo()
        {
            SalvarSelecoes();
            SalvarEstadios();
            SalvarJogos();
            SalvarClassificacao();
            SalvarMataMata();
            UI.Sucesso("Todos os dados salvos na pasta csv/.");
        }

        // ── CARREGAR ──────────────────────────────────────────────────────────

        public static void CarregarTudo()
        {
            CarregarSelecoes();
            CarregarEstadios();
            CarregarJogos();
        }

        static void CarregarSelecoes()
        {
            string path = Caminho("selecoes.csv");
            if (!File.Exists(path)) return;
            try
            {
                Dados.totalSelecoes = 0;
                string[] linhas = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 1; i < linhas.Length; i++)
                {
                    string l = linhas[i].Trim();
                    if (l.Length == 0) continue;
                    string[] p = l.Split(';');
                    if (p.Length < 4) continue;
                    if (Dados.totalSelecoes >= Dados.MAX_SELECOES) break;
                    int idx = Dados.totalSelecoes;
                    Dados.selecoes[idx].Id    = int.Parse(p[0]);
                    Dados.selecoes[idx].Nome  = p[1];
                    Dados.selecoes[idx].Grupo = p[2];
                    Dados.selecoes[idx].Ativo = p[3].Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.totalSelecoes++;
                }
            }
            catch { /* arquivo corrompido — ignora */ }
        }

        static void CarregarEstadios()
        {
            string path = Caminho("estadios.csv");
            if (!File.Exists(path)) return;
            try
            {
                Dados.totalEstadios = 0;
                string[] linhas = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 1; i < linhas.Length; i++)
                {
                    string l = linhas[i].Trim();
                    if (l.Length == 0) continue;
                    string[] p = l.Split(';');
                    if (p.Length < 6) continue;
                    if (Dados.totalEstadios >= Dados.MAX_ESTADIOS) break;
                    int idx = Dados.totalEstadios;
                    Dados.estadios[idx].Id         = int.Parse(p[0]);
                    Dados.estadios[idx].Nome       = p[1];
                    Dados.estadios[idx].Cidade     = p[2];
                    Dados.estadios[idx].Pais       = p[3];
                    Dados.estadios[idx].Capacidade = int.Parse(p[4]);
                    Dados.estadios[idx].Ativo      = p[5].Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.totalEstadios++;
                }
            }
            catch { }
        }

        static void CarregarJogos()
        {
            string path = Caminho("jogos.csv");
            if (!File.Exists(path)) return;
            try
            {
                Dados.totalJogos = 0;
                string[] linhas = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 1; i < linhas.Length; i++)
                {
                    string l = linhas[i].Trim();
                    if (l.Length == 0) continue;
                    string[] p = l.Split(';');
                    if (p.Length < 12) continue;
                    if (Dados.totalJogos >= Dados.MAX_JOGOS) break;
                    int idx = Dados.totalJogos;
                    Dados.jogos[idx].Id                 = int.Parse(p[0]);
                    Dados.jogos[idx].Fase               = p[1];
                    Dados.jogos[idx].Grupo              = p[2];
                    Dados.jogos[idx].Data               = p[3];
                    Dados.jogos[idx].IdEstadio          = int.Parse(p[4]);
                    Dados.jogos[idx].IdTimeA            = int.Parse(p[5]);
                    Dados.jogos[idx].IdTimeB            = int.Parse(p[6]);
                    Dados.jogos[idx].GolsA              = int.Parse(p[7]);
                    Dados.jogos[idx].GolsB              = int.Parse(p[8]);
                    Dados.jogos[idx].Realizado          = p[9].Equals("true",  StringComparison.OrdinalIgnoreCase);
                    Dados.jogos[idx].IdVencedorPenaltis = int.Parse(p[10]);
                    Dados.jogos[idx].Ativo              = p[11].Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.totalJogos++;
                }
            }
            catch { }
        }
    }
}
