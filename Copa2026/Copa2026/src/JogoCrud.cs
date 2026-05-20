using System;

namespace Copa2026
{
    static class JogoCrud
    {
        public static void Cadastrar()
        {
            UI.Cabecalho("CADASTRAR JOGO");

            if (Dados.totalJogos >= Dados.MAX_JOGOS)
            {
                UI.Erro("Limite de jogos atingido.");
                UI.Pausar();
                return;
            }

            string[] fasesValidas = { "Grupo", "32avos", "Oitavas", "Quartas", "Semifinal", "3Lugar", "Final" };
            string fase;
            while (true)
            {
                Console.WriteLine("Fases: Grupo | 32avos | Oitavas | Quartas | Semifinal | 3Lugar | Final");
                fase = UI.LerString("Fase do jogo  : ");
                bool ok = false;
                foreach (var f in fasesValidas)
                    if (f.Equals(fase, StringComparison.OrdinalIgnoreCase)) { fase = f; ok = true; break; }
                if (ok) break;
                UI.Erro("Fase inválida.");
            }

            string grupo = "";
            if (fase == "Grupo")
            {
                while (true)
                {
                    grupo = UI.LerString("Grupo (A-L) : ").ToUpper();
                    if (Dados.GrupoValido(grupo)) break;
                    UI.Erro("Grupo inválido.");
                }
            }

            string data = UI.LerData("Data (dd/MM/yyyy) : ");

            // Estádio
            EstadioCrud.Listar();
            int idEstadio = UI.LerInt("ID do estádio     : ", 1);
            if (Dados.IndexEstadio(idEstadio) < 0) { UI.Erro("Estádio não encontrado."); UI.Pausar(); return; }

            // Times
            SelecaoCrud.Listar();
            int idA = UI.LerInt("ID Time A          : ", 1);
            if (Dados.IndexSelecao(idA) < 0) { UI.Erro("Seleção A não encontrada."); UI.Pausar(); return; }
            int idB = UI.LerInt("ID Time B          : ", 1);
            if (Dados.IndexSelecao(idB) < 0) { UI.Erro("Seleção B não encontrada."); UI.Pausar(); return; }
            if (idA == idB) { UI.Erro("Uma seleção não pode jogar contra si mesma."); UI.Pausar(); return; }

            int idx = Dados.totalJogos;
            Dados.jogos[idx].Id                 = Dados.ProximoIdJogo();
            Dados.jogos[idx].Fase               = fase;
            Dados.jogos[idx].Grupo              = grupo;
            Dados.jogos[idx].Data               = data;
            Dados.jogos[idx].IdEstadio          = idEstadio;
            Dados.jogos[idx].IdTimeA            = idA;
            Dados.jogos[idx].IdTimeB            = idB;
            Dados.jogos[idx].GolsA              = 0;
            Dados.jogos[idx].GolsB              = 0;
            Dados.jogos[idx].Realizado          = false;
            Dados.jogos[idx].IdVencedorPenaltis = 0;
            Dados.jogos[idx].Ativo              = true;
            Dados.totalJogos++;

            UI.Sucesso($"Jogo ID {Dados.jogos[idx].Id} cadastrado: {Dados.NomeSelecao(idA)} x {Dados.NomeSelecao(idB)}.");
            UI.Pausar();
        }

        public static void Listar()
        {
            UI.Cabecalho("JOGOS CADASTRADOS");
            bool algum = false;
            for (int i = 0; i < Dados.totalJogos; i++)
            {
                if (!Dados.jogos[i].Ativo) continue;
                ImprimirJogo(Dados.jogos[i]);
                algum = true;
            }
            if (!algum) UI.Info("Nenhum jogo cadastrado.");
            UI.Pausar();
        }

        public static void ImprimirJogo(Jogo j)
        {
            string placar = j.Realizado
                ? $"{j.GolsA} x {j.GolsB}"
                : "Não realizado";
            string pen = (j.Realizado && j.GolsA == j.GolsB && j.IdVencedorPenaltis > 0)
                ? $" (Pen: {Dados.NomeSelecao(j.IdVencedorPenaltis)})"
                : "";
            Console.WriteLine($"[{j.Id,3}] {j.Fase,-10} {j.Grupo,-3} {j.Data,-12} " +
                              $"{Dados.NomeSelecao(j.IdTimeA),-20} {placar,-7} {Dados.NomeSelecao(j.IdTimeB),-20}" +
                              $" | {Dados.NomeEstadio(j.IdEstadio)}{pen}");
        }

        public static void Alterar()
        {
            UI.Cabecalho("ALTERAR JOGO");
            int id = UI.LerInt("ID do jogo : ", 1);
            int idx = Dados.IndexJogo(id);
            if (idx < 0) { UI.Erro("Jogo não encontrado."); UI.Pausar(); return; }

            string data = UI.LerData("Nova data (dd/MM/yyyy) : ");
            Dados.jogos[idx].Data = data;

            int idEst = UI.LerInt("Novo ID de estádio (0 manter) : ", 0);
            if (idEst > 0 && Dados.IndexEstadio(idEst) >= 0)
                Dados.jogos[idx].IdEstadio = idEst;

            UI.Sucesso("Jogo alterado.");
            UI.Pausar();
        }

        public static void Excluir()
        {
            UI.Cabecalho("EXCLUIR JOGO");
            int id = UI.LerInt("ID do jogo : ", 1);
            int idx = Dados.IndexJogo(id);
            if (idx < 0) { UI.Erro("Jogo não encontrado."); UI.Pausar(); return; }

            Console.Write($"Confirma exclusão do jogo {id}? (S/N) : ");
            string conf = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (conf == "S")
            {
                Dados.jogos[idx].Ativo = false;
                UI.Sucesso("Jogo excluído.");
            }
            else UI.Info("Cancelado.");
            UI.Pausar();
        }

        public static void RegistrarPlacar()
        {
            UI.Cabecalho("REGISTRAR PLACAR");
            int id = UI.LerInt("ID do jogo : ", 1);
            int idx = Dados.IndexJogo(id);
            if (idx < 0) { UI.Erro("Jogo não encontrado."); UI.Pausar(); return; }

            var j = Dados.jogos[idx];
            Console.WriteLine($"Jogo: {Dados.NomeSelecao(j.IdTimeA)} x {Dados.NomeSelecao(j.IdTimeB)}");

            int golsA = UI.LerInt($"Gols {Dados.NomeSelecao(j.IdTimeA),-20}: ", 0, 99);
            int golsB = UI.LerInt($"Gols {Dados.NomeSelecao(j.IdTimeB),-20}: ", 0, 99);

            Dados.jogos[idx].GolsA     = golsA;
            Dados.jogos[idx].GolsB     = golsB;
            Dados.jogos[idx].Realizado = true;

            // Empate no mata-mata → pede pênaltis
            bool mataMata = j.Fase != "Grupo";
            if (mataMata && golsA == golsB)
            {
                Console.WriteLine($"Empate no mata-mata! Quem venceu nos pênaltis?");
                Console.WriteLine($"  1 - {Dados.NomeSelecao(j.IdTimeA)}");
                Console.WriteLine($"  2 - {Dados.NomeSelecao(j.IdTimeB)}");
                int opcao = UI.LerInt("Escolha (1 ou 2) : ", 1, 2);
                Dados.jogos[idx].IdVencedorPenaltis = opcao == 1 ? j.IdTimeA : j.IdTimeB;
            }
            else
            {
                Dados.jogos[idx].IdVencedorPenaltis = 0;
            }

            UI.Sucesso("Resultado registrado.");
            UI.Pausar();
        }
    }
}
