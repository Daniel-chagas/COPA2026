using System;

namespace Copa2026
{
    static class EstadioCrud
    {
        public static void Cadastrar()
        {
            UI.Cabecalho("CADASTRAR ESTÁDIO");

            if (Dados.totalEstadios >= Dados.MAX_ESTADIOS)
            {
                UI.Erro("Limite de estádios atingido.");
                UI.Pausar();
                return;
            }

            string nome       = UI.LerString("Nome do estádio : ");
            string cidade     = UI.LerString("Cidade          : ");
            string pais       = UI.LerString("País            : ");
            int    capacidade = UI.LerInt   ("Capacidade      : ", 1, 200000);

            int idx = Dados.totalEstadios;
            Dados.estadios[idx].Id         = Dados.ProximoIdEstadio();
            Dados.estadios[idx].Nome       = nome;
            Dados.estadios[idx].Cidade     = cidade;
            Dados.estadios[idx].Pais       = pais;
            Dados.estadios[idx].Capacidade = capacidade;
            Dados.estadios[idx].Ativo      = true;
            Dados.totalEstadios++;

            UI.Sucesso($"Estádio '{nome}' cadastrado com ID {Dados.estadios[idx].Id}.");
            UI.Pausar();
        }

        public static void Listar()
        {
            UI.Cabecalho("ESTÁDIOS CADASTRADOS");
            bool algum = false;
            Console.WriteLine($"{"ID",-5} {"Nome",-35} {"Cidade",-20} {"País",-15} {"Cap.",-8}");
            Console.WriteLine(new string('-', 85));
            for (int i = 0; i < Dados.totalEstadios; i++)
            {
                if (!Dados.estadios[i].Ativo) continue;
                var e = Dados.estadios[i];
                Console.WriteLine($"{e.Id,-5} {e.Nome,-35} {e.Cidade,-20} {e.Pais,-15} {e.Capacidade,-8}");
                algum = true;
            }
            if (!algum) UI.Info("Nenhum estádio cadastrado.");
            UI.Pausar();
        }

        public static void Alterar()
        {
            UI.Cabecalho("ALTERAR ESTÁDIO");
            int id = UI.LerInt("ID do estádio : ", 1);
            int idx = Dados.IndexEstadio(id);
            if (idx < 0) { UI.Erro("Estádio não encontrado."); UI.Pausar(); return; }

            var e = Dados.estadios[idx];
            Console.WriteLine($"Atual: {e.Nome} | {e.Cidade} | {e.Pais} | Cap: {e.Capacidade}");

            string nome = UI.LerString("Novo nome (ENTER manter) : ", false);
            if (nome.Length > 0) Dados.estadios[idx].Nome = nome;

            string cidade = UI.LerString("Nova cidade (ENTER manter) : ", false);
            if (cidade.Length > 0) Dados.estadios[idx].Cidade = cidade;

            string pais = UI.LerString("Novo país (ENTER manter) : ", false);
            if (pais.Length > 0) Dados.estadios[idx].Pais = pais;

            Console.Write("Nova capacidade (0 para manter) : ");
            if (int.TryParse(Console.ReadLine(), out int cap) && cap > 0)
                Dados.estadios[idx].Capacidade = cap;

            UI.Sucesso("Estádio alterado.");
            UI.Pausar();
        }

        public static void Excluir()
        {
            UI.Cabecalho("EXCLUIR ESTÁDIO");
            int id = UI.LerInt("ID do estádio : ", 1);
            int idx = Dados.IndexEstadio(id);
            if (idx < 0) { UI.Erro("Estádio não encontrado."); UI.Pausar(); return; }

            Console.Write($"Confirma exclusão de '{Dados.estadios[idx].Nome}'? (S/N) : ");
            string conf = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (conf == "S")
            {
                Dados.estadios[idx].Ativo = false;
                UI.Sucesso("Estádio excluído.");
            }
            else UI.Info("Cancelado.");
            UI.Pausar();
        }
    }
}
