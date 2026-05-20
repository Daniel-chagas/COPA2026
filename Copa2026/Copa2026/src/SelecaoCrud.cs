using System;

namespace Copa2026
{
    static class SelecaoCrud
    {
        public static void Cadastrar()
        {
            UI.Cabecalho("CADASTRAR SELEÇÃO");

            if (Dados.totalSelecoes >= Dados.MAX_SELECOES)
            {
                UI.Erro("Limite de 48 seleções atingido.");
                UI.Pausar();
                return;
            }

            string nome = UI.LerString("Nome da seleção : ");

            string grupo;
            while (true)
            {
                grupo = UI.LerString("Grupo (A-L)      : ").ToUpper();
                if (!Dados.GrupoValido(grupo))  { UI.Erro("Grupo inválido. Use A até L."); continue; }
                if (Dados.SelecoesPorGrupo(grupo) >= 4) { UI.Erro("Grupo já tem 4 seleções."); continue; }
                break;
            }

            int idx = Dados.totalSelecoes;
            Dados.selecoes[idx].Id    = Dados.ProximoIdSelecao();
            Dados.selecoes[idx].Nome  = nome;
            Dados.selecoes[idx].Grupo = grupo;
            Dados.selecoes[idx].Ativo = true;
            Dados.totalSelecoes++;

            UI.Sucesso($"Seleção '{nome}' cadastrada com ID {Dados.selecoes[idx].Id}.");
            UI.Pausar();
        }

        public static void Listar()
        {
            UI.Cabecalho("SELEÇÕES CADASTRADAS");
            bool alguma = false;
            Console.WriteLine($"{"ID",-5} {"Nome",-25} {"Grupo",-6}");
            Console.WriteLine(new string('-', 38));
            for (int i = 0; i < Dados.totalSelecoes; i++)
            {
                if (!Dados.selecoes[i].Ativo) continue;
                var s = Dados.selecoes[i];
                Console.WriteLine($"{s.Id,-5} {s.Nome,-25} {s.Grupo,-6}");
                alguma = true;
            }
            if (!alguma) UI.Info("Nenhuma seleção cadastrada.");
            UI.Pausar();
        }

        public static void Alterar()
        {
            UI.Cabecalho("ALTERAR SELEÇÃO");
            int id = UI.LerInt("ID da seleção : ", 1);
            int idx = Dados.IndexSelecao(id);
            if (idx < 0) { UI.Erro("Seleção não encontrada."); UI.Pausar(); return; }

            Console.WriteLine($"Seleção atual: {Dados.selecoes[idx].Nome} | Grupo: {Dados.selecoes[idx].Grupo}");

            string nome = UI.LerString("Novo nome (ENTER para manter) : ", false);
            if (nome.Length > 0) Dados.selecoes[idx].Nome = nome;

            string grupo;
            while (true)
            {
                grupo = UI.LerString("Novo grupo A-L (ENTER para manter) : ", false).ToUpper();
                if (grupo.Length == 0) break;
                if (!Dados.GrupoValido(grupo)) { UI.Erro("Grupo inválido."); continue; }
                // verifica limite do grupo (excluindo a própria seleção)
                int count = 0;
                for (int i = 0; i < Dados.totalSelecoes; i++)
                    if (i != idx && Dados.selecoes[i].Ativo &&
                        Dados.selecoes[i].Grupo == grupo) count++;
                if (count >= 4) { UI.Erro("Grupo já tem 4 seleções."); continue; }
                Dados.selecoes[idx].Grupo = grupo;
                break;
            }

            UI.Sucesso("Seleção alterada.");
            UI.Pausar();
        }

        public static void Excluir()
        {
            UI.Cabecalho("EXCLUIR SELEÇÃO");
            int id = UI.LerInt("ID da seleção : ", 1);
            int idx = Dados.IndexSelecao(id);
            if (idx < 0) { UI.Erro("Seleção não encontrada."); UI.Pausar(); return; }

            Console.Write($"Confirma exclusão de '{Dados.selecoes[idx].Nome}'? (S/N) : ");
            string conf = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (conf == "S")
            {
                Dados.selecoes[idx].Ativo = false;
                UI.Sucesso("Seleção excluída (exclusão lógica).");
            }
            else UI.Info("Operação cancelada.");
            UI.Pausar();
        }
    }
}
