using System;

namespace Copa2026
{
    static class MataMata
    {
        static string[] grupos = { "A","B","C","D","E","F","G","H","I","J","K","L" };

        // Verifica se todos os jogos de grupo foram realizados
        static bool GruposFinalizados()
        {
            for (int i = 0; i < Dados.totalJogos; i++)
            {
                var j = Dados.jogos[i];
                if (j.Ativo && j.Fase == "Grupo" && !j.Realizado) return false;
            }
            return true;
        }

        // Conta jogos de grupo cadastrados
        static int JogosGrupoCadastrados()
        {
            int c = 0;
            for (int i = 0; i < Dados.totalJogos; i++)
                if (Dados.jogos[i].Ativo && Dados.jogos[i].Fase == "Grupo") c++;
            return c;
        }

        public static void GerarFaseDe32()
        {
            UI.Cabecalho("GERAR FASE DE 32 (MATA-MATA)");

            if (JogosGrupoCadastrados() == 0)
            {
                UI.Erro("Cadastre os jogos da fase de grupos primeiro.");
                UI.Pausar();
                return;
            }
            if (!GruposFinalizados())
            {
                UI.Erro("Todos os jogos da fase de grupos devem estar realizados antes de gerar o mata-mata.");
                UI.Pausar();
                return;
            }

            // Verifica se já existe mata-mata cadastrado
            for (int i = 0; i < Dados.totalJogos; i++)
                if (Dados.jogos[i].Ativo && Dados.jogos[i].Fase == "32avos")
                {
                    UI.Erro("Fase de 32 já foi gerada. Registre os resultados para avançar.");
                    UI.Pausar();
                    return;
                }

            Classificacao.GerarClassificacao();

            // Monta os 24 classificados diretos (1° e 2° de cada grupo)
            int[] classificados = new int[32]; // índices no vetor de seleções
            int nc = 0;

            foreach (var g in grupos)
            {
                int p1 = Classificacao.ObterClassificado(g, 0);
                int p2 = Classificacao.ObterClassificado(g, 1);
                if (p1 >= 0) classificados[nc++] = p1;
                if (p2 >= 0) classificados[nc++] = p2;
            }

            // 8 melhores terceiros
            int[] terceiros = new int[12];
            int nt = 0;
            foreach (var g in grupos)
            {
                int p3 = Classificacao.ObterClassificado(g, 2);
                if (p3 >= 0) terceiros[nt++] = p3;
            }
            // Ordena
            for (int a = 0; a < nt - 1; a++)
                for (int b = a + 1; b < nt; b++)
                    if (Classificacao.CompararSelecoes(terceiros[b], terceiros[a]) < 0)
                    { int tmp = terceiros[a]; terceiros[a] = terceiros[b]; terceiros[b] = tmp; }

            for (int p = 0; p < 8 && p < nt; p++)
                classificados[nc++] = terceiros[p];

            if (nc < 32)
            {
                UI.Erro($"Apenas {nc} seleções classificadas. Cadastre seleções suficientes em todos os grupos.");
                UI.Pausar();
                return;
            }

            // Chaveamento simplificado: pares (0x1, 2x3, ... 30x31)
            string dataBase = "28/06/2026";
            for (int par = 0; par < 16; par++)
            {
                int ia = classificados[par * 2];
                int ib = classificados[par * 2 + 1];

                if (Dados.totalJogos >= Dados.MAX_JOGOS) break;

                int idx = Dados.totalJogos;
                Dados.jogos[idx].Id                 = Dados.ProximoIdJogo();
                Dados.jogos[idx].Fase               = "32avos";
                Dados.jogos[idx].Grupo              = "";
                Dados.jogos[idx].Data               = dataBase;
                Dados.jogos[idx].IdEstadio          = Dados.totalEstadios > 0 ? Dados.estadios[par % Dados.totalEstadios].Id : 1;
                Dados.jogos[idx].IdTimeA            = Dados.selecoes[ia].Id;
                Dados.jogos[idx].IdTimeB            = Dados.selecoes[ib].Id;
                Dados.jogos[idx].GolsA              = 0;
                Dados.jogos[idx].GolsB              = 0;
                Dados.jogos[idx].Realizado          = false;
                Dados.jogos[idx].IdVencedorPenaltis = 0;
                Dados.jogos[idx].Ativo              = true;
                Dados.totalJogos++;
            }

            UI.Sucesso("16 jogos da Fase de 32 gerados! Registre os placares para avançar.");
            UI.Pausar();
        }

        // Gera próxima fase com os vencedores da fase anterior
        public static void AvancarFase(string faseAtual, string proxFase)
        {
            UI.Cabecalho($"AVANÇAR: {faseAtual} → {proxFase}");

            // Coleta vencedores da fase atual
            int[] vencedores = new int[16];
            int nv = 0;
            bool algumNaoRealizado = false;

            for (int i = 0; i < Dados.totalJogos; i++)
            {
                var j = Dados.jogos[i];
                if (!j.Ativo || j.Fase != faseAtual) continue;
                if (!j.Realizado) { algumNaoRealizado = true; break; }

                int venc;
                if (j.GolsA > j.GolsB) venc = j.IdTimeA;
                else if (j.GolsB > j.GolsA) venc = j.IdTimeB;
                else venc = j.IdVencedorPenaltis;

                if (nv < 16) vencedores[nv++] = venc;
            }

            if (algumNaoRealizado)
            {
                UI.Erro($"Registre todos os resultados de {faseAtual} antes de avançar.");
                UI.Pausar();
                return;
            }

            // Verifica se próxima fase já existe
            for (int i = 0; i < Dados.totalJogos; i++)
                if (Dados.jogos[i].Ativo && Dados.jogos[i].Fase == proxFase)
                {
                    UI.Erro($"Fase {proxFase} já foi gerada.");
                    UI.Pausar();
                    return;
                }

            string[] datas = {
                "04/07/2026","04/07/2026","05/07/2026","05/07/2026",
                "06/07/2026","06/07/2026","07/07/2026","07/07/2026"
            };
            if (proxFase == "Quartas")
                datas = new[] { "09/07/2026","09/07/2026","10/07/2026","11/07/2026" };
            if (proxFase == "Semifinal")
                datas = new[] { "14/07/2026","15/07/2026" };
            if (proxFase == "3Lugar")
                datas = new[] { "18/07/2026" };
            if (proxFase == "Final")
                datas = new[] { "19/07/2026" };

            int pares = nv / 2;
            for (int par = 0; par < pares; par++)
            {
                if (Dados.totalJogos >= Dados.MAX_JOGOS) break;

                string data = par < datas.Length ? datas[par] : datas[datas.Length - 1];
                int estadioId = Dados.totalEstadios > 0 ? Dados.estadios[par % Dados.totalEstadios].Id : 1;

                int idx = Dados.totalJogos;
                Dados.jogos[idx].Id                 = Dados.ProximoIdJogo();
                Dados.jogos[idx].Fase               = proxFase;
                Dados.jogos[idx].Grupo              = "";
                Dados.jogos[idx].Data               = data;
                Dados.jogos[idx].IdEstadio          = estadioId;
                Dados.jogos[idx].IdTimeA            = vencedores[par * 2];
                Dados.jogos[idx].IdTimeB            = vencedores[par * 2 + 1];
                Dados.jogos[idx].GolsA              = 0;
                Dados.jogos[idx].GolsB              = 0;
                Dados.jogos[idx].Realizado          = false;
                Dados.jogos[idx].IdVencedorPenaltis = 0;
                Dados.jogos[idx].Ativo              = true;
                Dados.totalJogos++;
            }

            // Disputa de 3° lugar (semifinal)
            if (proxFase == "Final" && nv >= 4)
            {
                // Perdedores da semi
                int[] perdedores = new int[2];
                int np = 0;
                for (int i = 0; i < Dados.totalJogos; i++)
                {
                    var j = Dados.jogos[i];
                    if (!j.Ativo || j.Fase != "Semifinal" || !j.Realizado) continue;
                    int perd;
                    if (j.GolsA > j.GolsB) perd = j.IdTimeB;
                    else if (j.GolsB > j.GolsA) perd = j.IdTimeA;
                    else perd = (j.IdVencedorPenaltis == j.IdTimeA) ? j.IdTimeB : j.IdTimeA;
                    if (np < 2) perdedores[np++] = perd;
                }

                // Verifica se disputa do 3° já existe
                bool jaExiste = false;
                for (int i = 0; i < Dados.totalJogos; i++)
                    if (Dados.jogos[i].Ativo && Dados.jogos[i].Fase == "3Lugar") { jaExiste = true; break; }

                if (!jaExiste && np == 2 && Dados.totalJogos < Dados.MAX_JOGOS)
                {
                    int idx3 = Dados.totalJogos;
                    Dados.jogos[idx3].Id                 = Dados.ProximoIdJogo();
                    Dados.jogos[idx3].Fase               = "3Lugar";
                    Dados.jogos[idx3].Data               = "18/07/2026";
                    Dados.jogos[idx3].IdEstadio          = Dados.totalEstadios > 0 ? Dados.estadios[0].Id : 1;
                    Dados.jogos[idx3].IdTimeA            = perdedores[0];
                    Dados.jogos[idx3].IdTimeB            = perdedores[1];
                    Dados.jogos[idx3].Realizado          = false;
                    Dados.jogos[idx3].IdVencedorPenaltis = 0;
                    Dados.jogos[idx3].Ativo              = true;
                    Dados.totalJogos++;
                }
            }

            UI.Sucesso($"Fase {proxFase} gerada com {pares} jogo(s)!");
            UI.Pausar();
        }

        public static void MostrarChave()
        {
            UI.Cabecalho("CHAVE DO MATA-MATA");
            string[] fases = { "32avos","Oitavas","Quartas","Semifinal","3Lugar","Final" };
            string[] nomes = { "FASE DE 32","OITAVAS DE FINAL","QUARTAS DE FINAL","SEMIFINAL","DISPUTA 3° LUGAR","FINAL" };

            for (int f = 0; f < fases.Length; f++)
            {
                bool algum = false;
                for (int i = 0; i < Dados.totalJogos; i++)
                    if (Dados.jogos[i].Ativo && Dados.jogos[i].Fase == fases[f]) { algum = true; break; }

                if (!algum) continue;

                Console.WriteLine($"\n── {nomes[f]} ──");
                for (int i = 0; i < Dados.totalJogos; i++)
                {
                    var j = Dados.jogos[i];
                    if (!j.Ativo || j.Fase != fases[f]) continue;
                    JogoCrud.ImprimirJogo(j);
                }
            }

            // Campeão
            MostrarCampeao();

            UI.Pausar();
        }

        public static void MostrarCampeao()
        {
            for (int i = 0; i < Dados.totalJogos; i++)
            {
                var j = Dados.jogos[i];
                if (!j.Ativo || j.Fase != "Final" || !j.Realizado) continue;

                int campeaoId;
                if (j.GolsA > j.GolsB) campeaoId = j.IdTimeA;
                else if (j.GolsB > j.GolsA) campeaoId = j.IdTimeB;
                else campeaoId = j.IdVencedorPenaltis;

                Console.WriteLine();
                Console.WriteLine(new string('*', 50));
                Console.WriteLine($"  CAMPEÃO DA COPA 2026: {Dados.NomeSelecao(campeaoId).ToUpper()}  ");
                Console.WriteLine(new string('*', 50));
                return;
            }
            Console.WriteLine("\n[A final ainda não foi disputada.]");
        }
    }
}
