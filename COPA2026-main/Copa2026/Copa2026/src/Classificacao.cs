using System;

namespace Copa2026
{
    static class Classificacao
    {
        // Gera tabela do zero com base nos jogos realizados
        public static void GerarClassificacao()
        {
            // Zera matriz
            for (int i = 0; i < Dados.MAX_SELECOES; i++)
                for (int c = 0; c < 8; c++)
                    Dados.tabela[i, c] = 0;

            // Percorre jogos de grupo realizados
            for (int k = 0; k < Dados.totalJogos; k++)
            {
                var j = Dados.jogos[k];
                if (!j.Ativo || !j.Realizado || j.Fase != "Grupo") continue;

                int ia = Dados.IndexSelecao(j.IdTimeA);
                int ib = Dados.IndexSelecao(j.IdTimeB);
                if (ia < 0 || ib < 0) continue;

                // Jogos
                Dados.tabela[ia, 0]++;
                Dados.tabela[ib, 0]++;

                // Gols
                Dados.tabela[ia, 4] += j.GolsA;
                Dados.tabela[ia, 5] += j.GolsB;
                Dados.tabela[ib, 4] += j.GolsB;
                Dados.tabela[ib, 5] += j.GolsA;

                // Resultado
                if (j.GolsA > j.GolsB)
                { Dados.tabela[ia, 1]++; Dados.tabela[ib, 3]++; Dados.tabela[ia, 7] += 3; }
                else if (j.GolsB > j.GolsA)
                { Dados.tabela[ib, 1]++; Dados.tabela[ia, 3]++; Dados.tabela[ib, 7] += 3; }
                else
                { Dados.tabela[ia, 2]++; Dados.tabela[ib, 2]++; Dados.tabela[ia, 7]++; Dados.tabela[ib, 7]++; }

                // Saldo de gols
                Dados.tabela[ia, 6] = Dados.tabela[ia, 4] - Dados.tabela[ia, 5];
                Dados.tabela[ib, 6] = Dados.tabela[ib, 4] - Dados.tabela[ib, 5];
            }
        }

        public static void MostrarTodosGrupos()
        {
            UI.Cabecalho("TABELA DE CLASSIFICAÇÃO — GRUPOS");
            GerarClassificacao();

            string[] grupos = { "A","B","C","D","E","F","G","H","I","J","K","L" };
            foreach (var g in grupos)
            {
                MostrarGrupo(g);
                Console.WriteLine();
            }
            UI.Pausar();
        }

        public static void MostrarGrupo(string grupo)
        {
            // Coleta seleções do grupo
            int[] ids = new int[4];
            int n = 0;
            for (int i = 0; i < Dados.totalSelecoes; i++)
            {
                if (Dados.selecoes[i].Ativo &&
                    Dados.selecoes[i].Grupo.Equals(grupo, StringComparison.OrdinalIgnoreCase))
                {
                    if (n < 4) ids[n++] = i;
                }
            }
            if (n == 0) return;

            // Ordena (bubble sort pelos critérios de desempate)
            for (int a = 0; a < n - 1; a++)
                for (int b = a + 1; b < n; b++)
                    if (CompararSelecoes(ids[b], ids[a]) < 0)
                    { int tmp = ids[a]; ids[a] = ids[b]; ids[b] = tmp; }

            Console.WriteLine($"\nGRUPO {grupo}");
            Console.WriteLine($"{"Pos",-4} {"Seleção",-22} {"J",2} {"V",2} {"E",2} {"D",2} {"GP",3} {"GC",3} {"SG",4} {"PTS",4}");
            Console.WriteLine(new string('-', 55));
            for (int p = 0; p < n; p++)
            {
                int i = ids[p];
                Console.WriteLine($"{p + 1}°   {Dados.selecoes[i].Nome,-22} " +
                                  $"{Dados.tabela[i,0],2} {Dados.tabela[i,1],2} {Dados.tabela[i,2],2} " +
                                  $"{Dados.tabela[i,3],2} {Dados.tabela[i,4],3} {Dados.tabela[i,5],3} " +
                                  $"{Dados.tabela[i,6],4} {Dados.tabela[i,7],4}");
            }
        }

        // Retorna < 0 se A é melhor que B
        public static int CompararSelecoes(int idxA, int idxB)
        {
            int pts  = Dados.tabela[idxB, 7] - Dados.tabela[idxA, 7]; if (pts  != 0) return pts;
            int vit  = Dados.tabela[idxB, 1] - Dados.tabela[idxA, 1]; if (vit  != 0) return vit;
            int sg   = Dados.tabela[idxB, 6] - Dados.tabela[idxA, 6]; if (sg   != 0) return sg;
            int gp   = Dados.tabela[idxB, 4] - Dados.tabela[idxA, 4]; if (gp   != 0) return gp;
            return string.Compare(Dados.selecoes[idxA].Nome, Dados.selecoes[idxB].Nome, StringComparison.Ordinal);
        }

        // Retorna índice da seleção no vetor de seleções pela posição no grupo
        // posicao: 0=1°, 1=2°, 2=3°
        public static int ObterClassificado(string grupo, int posicao)
        {
            int[] ids = new int[4];
            int n = 0;
            for (int i = 0; i < Dados.totalSelecoes; i++)
                if (Dados.selecoes[i].Ativo && Dados.selecoes[i].Grupo.Equals(grupo, StringComparison.OrdinalIgnoreCase))
                    if (n < 4) ids[n++] = i;

            for (int a = 0; a < n - 1; a++)
                for (int b = a + 1; b < n; b++)
                    if (CompararSelecoes(ids[b], ids[a]) < 0)
                    { int tmp = ids[a]; ids[a] = ids[b]; ids[b] = tmp; }

            return posicao < n ? ids[posicao] : -1;
        }

        public static void MostrarMelhoresTerceiros()
        {
            UI.Cabecalho("MELHORES TERCEIROS COLOCADOS");
            GerarClassificacao();

            string[] grupos = { "A","B","C","D","E","F","G","H","I","J","K","L" };
            int[] terceiros = new int[12];
            int nt = 0;
            foreach (var g in grupos)
            {
                int idx = ObterClassificado(g, 2);
                if (idx >= 0) terceiros[nt++] = idx;
            }

            // Ordena
            for (int a = 0; a < nt - 1; a++)
                for (int b = a + 1; b < nt; b++)
                    if (CompararSelecoes(terceiros[b], terceiros[a]) < 0)
                    { int tmp = terceiros[a]; terceiros[a] = terceiros[b]; terceiros[b] = tmp; }

            Console.WriteLine($"{"Pos",-4} {"Seleção",-22} {"Grupo",-6} {"PTS",4} {"V",2} {"SG",4} {"GP",3}");
            Console.WriteLine(new string('-', 48));
            for (int p = 0; p < nt; p++)
            {
                int i = terceiros[p];
                string classificado = p < 8 ? " ← CLASSIFICADO" : "";
                Console.WriteLine($"{p + 1}°   {Dados.selecoes[i].Nome,-22} {Dados.selecoes[i].Grupo,-6} " +
                                  $"{Dados.tabela[i,7],4} {Dados.tabela[i,1],2} {Dados.tabela[i,6],4} {Dados.tabela[i,4],3}{classificado}");
            }
            UI.Pausar();
        }
    }
}
