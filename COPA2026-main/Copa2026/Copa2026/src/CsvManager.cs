using System;
using System.IO;
using System.Text;

namespace Copa2026
{
    static class CsvManager
    {
        // Pasta "csv" sempre ao lado do executável — resolve o problema de diretório relativo
        static readonly string DIR = Path.Combine(AppContext.BaseDirectory, "csv");

        static string Caminho(string arquivo) => Path.Combine(DIR, arquivo);

        static void GarantirDiretorio()
        {
            try
            {
                if (!Directory.Exists(DIR))
                    Directory.CreateDirectory(DIR);
            }
            catch (Exception ex)
            {
                UI.Erro($"Não foi possível criar a pasta csv/: {ex.Message}");
            }
        }

        // ── REORDENAR IDs ─────────────────────────────────────────────────────
        // Compacta os IDs em memória antes de salvar, mantendo sequência 1,2,3...
        // Atualiza referências cruzadas nos jogos para manter consistência.

        static void ReordenarIds()
        {
            // ── 1. Compactar vetor de seleções (remove inativos) e renumerar ──
            int novoTotal = 0;
            for (int i = 0; i < Dados.totalSelecoes; i++)
            {
                if (!Dados.selecoes[i].Ativo) continue;
                int novoId = novoTotal + 1;

                // Atualiza referências nos jogos antes de mudar o ID
                int idAntigo = Dados.selecoes[i].Id;
                if (idAntigo != novoId)
                {
                    for (int k = 0; k < Dados.totalJogos; k++)
                    {
                        if (!Dados.jogos[k].Ativo) continue;
                        if (Dados.jogos[k].IdTimeA == idAntigo) Dados.jogos[k].IdTimeA = novoId;
                        if (Dados.jogos[k].IdTimeB == idAntigo) Dados.jogos[k].IdTimeB = novoId;
                        if (Dados.jogos[k].IdVencedorPenaltis == idAntigo) Dados.jogos[k].IdVencedorPenaltis = novoId;
                    }
                    Dados.selecoes[i].Id = novoId;
                }

                // Move para a posição compactada
                if (i != novoTotal)
                {
                    Dados.selecoes[novoTotal] = Dados.selecoes[i];
                    Dados.selecoes[i] = new Selecao();
                }
                novoTotal++;
            }
            Dados.totalSelecoes = novoTotal;

            // ── 2. Compactar estádios e renumerar ──
            novoTotal = 0;
            for (int i = 0; i < Dados.totalEstadios; i++)
            {
                if (!Dados.estadios[i].Ativo) continue;
                int novoId = novoTotal + 1;
                int idAntigo = Dados.estadios[i].Id;

                if (idAntigo != novoId)
                {
                    for (int k = 0; k < Dados.totalJogos; k++)
                        if (Dados.jogos[k].Ativo && Dados.jogos[k].IdEstadio == idAntigo)
                            Dados.jogos[k].IdEstadio = novoId;

                    Dados.estadios[i].Id = novoId;
                }

                if (i != novoTotal)
                {
                    Dados.estadios[novoTotal] = Dados.estadios[i];
                    Dados.estadios[i] = new Estadio();
                }
                novoTotal++;
            }
            Dados.totalEstadios = novoTotal;

            // ── 3. Compactar jogos e renumerar ──
            novoTotal = 0;
            for (int i = 0; i < Dados.totalJogos; i++)
            {
                if (!Dados.jogos[i].Ativo) continue;
                Dados.jogos[i].Id = novoTotal + 1;

                if (i != novoTotal)
                {
                    Dados.jogos[novoTotal] = Dados.jogos[i];
                    Dados.jogos[i] = new Jogo();
                }
                novoTotal++;
            }
            Dados.totalJogos = novoTotal;
        }

        // ── SALVAR ────────────────────────────────────────────────────────────

        public static void SalvarSelecoes()
        {
            GarantirDiretorio();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("id;nome;grupo;ativo");
                for (int i = 0; i < Dados.totalSelecoes; i++)
                {
                    var s = Dados.selecoes[i];
                    if (!s.Ativo) continue;
                    sb.AppendLine($"{s.Id};{s.Nome};{s.Grupo};true");
                }
                File.WriteAllText(Caminho("selecoes.csv"), sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex) { UI.Erro($"Erro ao salvar seleções: {ex.Message}"); }
        }

        public static void SalvarEstadios()
        {
            GarantirDiretorio();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("id;nome;cidade;pais;capacidade;ativo");
                for (int i = 0; i < Dados.totalEstadios; i++)
                {
                    var e = Dados.estadios[i];
                    if (!e.Ativo) continue;
                    sb.AppendLine($"{e.Id};{e.Nome};{e.Cidade};{e.Pais};{e.Capacidade};true");
                }
                File.WriteAllText(Caminho("estadios.csv"), sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex) { UI.Erro($"Erro ao salvar estádios: {ex.Message}"); }
        }

        public static void SalvarJogos()
        {
            GarantirDiretorio();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("id;fase;grupo;data;idEstadio;idTimeA;idTimeB;golsA;golsB;realizado;idVencedorPenaltis;ativo");
                for (int i = 0; i < Dados.totalJogos; i++)
                {
                    var j = Dados.jogos[i];
                    if (!j.Ativo) continue;
                    sb.AppendLine($"{j.Id};{j.Fase};{j.Grupo};{j.Data};{j.IdEstadio};{j.IdTimeA};{j.IdTimeB};" +
                                  $"{j.GolsA};{j.GolsB};{j.Realizado.ToString().ToLower()};{j.IdVencedorPenaltis};true");
                }
                File.WriteAllText(Caminho("jogos.csv"), sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex) { UI.Erro($"Erro ao salvar jogos: {ex.Message}"); }
        }

        public static void SalvarClassificacao()
        {
            GarantirDiretorio();
            try
            {
                Classificacao.GerarClassificacao();
                var sb = new StringBuilder();
                sb.AppendLine("grupo;posicao;selecao;j;v;e;d;gp;gc;sg;pts");

                string[] grupos = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
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
                                      $"{Dados.tabela[i, 0]};{Dados.tabela[i, 1]};{Dados.tabela[i, 2]};" +
                                      $"{Dados.tabela[i, 3]};{Dados.tabela[i, 4]};{Dados.tabela[i, 5]};" +
                                      $"{Dados.tabela[i, 6]};{Dados.tabela[i, 7]}");
                    }
                }
                File.WriteAllText(Caminho("classificacao.csv"), sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex) { UI.Erro($"Erro ao salvar classificação: {ex.Message}"); }
        }

        public static void SalvarMataMata()
        {
            GarantirDiretorio();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("id;fase;data;estadio;timeA;golsA;golsB;timeB;vencedor;realizado");

                string[] fases = { "32avos", "Oitavas", "Quartas", "Semifinal", "3Lugar", "Final" };
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
            catch (Exception ex) { UI.Erro($"Erro ao salvar mata-mata: {ex.Message}"); }
        }

        public static void GerarRelatorioFinal()
        {
            GarantirDiretorio();
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("fase;grupo;data;estadio;timeA;golsA;golsB;timeB;vencedor");

                string[] todas = { "Grupo", "32avos", "Oitavas", "Quartas", "Semifinal", "3Lugar", "Final" };
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
                UI.Sucesso($"relatorio_final.csv gerado em: {Caminho("relatorio_final.csv")}");
            }
            catch (Exception ex) { UI.Erro($"Erro ao gerar relatório: {ex.Message}"); }
        }

        public static void SalvarTudo()
        {
            // Reordena IDs em memória antes de gravar — garante sequência 1,2,3...
            // e atualiza todas as referências cruzadas entre jogos, seleções e estádios.
            ReordenarIds();

            SalvarSelecoes();
            SalvarEstadios();
            SalvarJogos();
            SalvarClassificacao();
            SalvarMataMata();
            UI.Sucesso($"Todos os dados salvos em: {DIR}");
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

            int carregados = 0, erros = 0;
            try
            {
                Dados.totalSelecoes = 0;
                string[] linhas = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 1; i < linhas.Length; i++)
                {
                    string l = linhas[i].Trim();
                    if (l.Length == 0) continue;
                    string[] p = l.Split(';');
                    if (p.Length < 4) { erros++; continue; }
                    if (Dados.totalSelecoes >= Dados.MAX_SELECOES) break;

                    if (!int.TryParse(p[0], out int id)) { erros++; continue; }

                    int idx = Dados.totalSelecoes;
                    Dados.selecoes[idx].Id = id;
                    Dados.selecoes[idx].Nome = p[1];
                    Dados.selecoes[idx].Grupo = p[2];
                    Dados.selecoes[idx].Ativo = p[3].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.totalSelecoes++;
                    carregados++;
                }
            }
            catch (Exception ex) { UI.Erro($"Erro ao carregar seleções: {ex.Message}"); return; }

            Console.WriteLine($"  [CSV] Seleções: {carregados} carregada(s)" +
                              (erros > 0 ? $", {erros} linha(s) ignorada(s)" : "") + ".");
        }

        static void CarregarEstadios()
        {
            string path = Caminho("estadios.csv");
            if (!File.Exists(path)) return;

            int carregados = 0, erros = 0;
            try
            {
                Dados.totalEstadios = 0;
                string[] linhas = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 1; i < linhas.Length; i++)
                {
                    string l = linhas[i].Trim();
                    if (l.Length == 0) continue;
                    string[] p = l.Split(';');
                    if (p.Length < 6) { erros++; continue; }
                    if (Dados.totalEstadios >= Dados.MAX_ESTADIOS) break;

                    if (!int.TryParse(p[0], out int id)) { erros++; continue; }
                    if (!int.TryParse(p[4], out int cap)) { erros++; continue; }

                    int idx = Dados.totalEstadios;
                    Dados.estadios[idx].Id = id;
                    Dados.estadios[idx].Nome = p[1];
                    Dados.estadios[idx].Cidade = p[2];
                    Dados.estadios[idx].Pais = p[3];
                    Dados.estadios[idx].Capacidade = cap;
                    Dados.estadios[idx].Ativo = p[5].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.totalEstadios++;
                    carregados++;
                }
            }
            catch (Exception ex) { UI.Erro($"Erro ao carregar estádios: {ex.Message}"); return; }

            Console.WriteLine($"  [CSV] Estádios: {carregados} carregado(s)" +
                              (erros > 0 ? $", {erros} linha(s) ignorada(s)" : "") + ".");
        }

        static void CarregarJogos()
        {
            string path = Caminho("jogos.csv");
            if (!File.Exists(path)) return;

            int carregados = 0, erros = 0;
            try
            {
                Dados.totalJogos = 0;
                string[] linhas = File.ReadAllLines(path, Encoding.UTF8);
                for (int i = 1; i < linhas.Length; i++)
                {
                    string l = linhas[i].Trim();
                    if (l.Length == 0) continue;
                    string[] p = l.Split(';');
                    if (p.Length < 12) { erros++; continue; }
                    if (Dados.totalJogos >= Dados.MAX_JOGOS) break;

                    if (!int.TryParse(p[0], out int id)) { erros++; continue; }
                    if (!int.TryParse(p[4], out int idEst)) { erros++; continue; }
                    if (!int.TryParse(p[5], out int idA)) { erros++; continue; }
                    if (!int.TryParse(p[6], out int idB)) { erros++; continue; }
                    if (!int.TryParse(p[7], out int golsA)) { erros++; continue; }
                    if (!int.TryParse(p[8], out int golsB)) { erros++; continue; }
                    if (!int.TryParse(p[10], out int idVenc)) { erros++; continue; }

                    int idx = Dados.totalJogos;
                    Dados.jogos[idx].Id = id;
                    Dados.jogos[idx].Fase = p[1];
                    Dados.jogos[idx].Grupo = p[2];
                    Dados.jogos[idx].Data = p[3];
                    Dados.jogos[idx].IdEstadio = idEst;
                    Dados.jogos[idx].IdTimeA = idA;
                    Dados.jogos[idx].IdTimeB = idB;
                    Dados.jogos[idx].GolsA = golsA;
                    Dados.jogos[idx].GolsB = golsB;
                    Dados.jogos[idx].Realizado = p[9].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.jogos[idx].IdVencedorPenaltis = idVenc;
                    Dados.jogos[idx].Ativo = p[11].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                    Dados.totalJogos++;
                    carregados++;
                }
            }
            catch (Exception ex) { UI.Erro($"Erro ao carregar jogos: {ex.Message}"); return; }

            Console.WriteLine($"  [CSV] Jogos: {carregados} carregado(s)" +
                              (erros > 0 ? $", {erros} linha(s) ignorada(s)" : "") + ".");
        }
    }
}