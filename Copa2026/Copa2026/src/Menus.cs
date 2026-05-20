using System;

namespace Copa2026
{
    static class Menus
    {
        public static bool MenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║       SISTEMA COPA DO MUNDO 2026         ║");
            Console.WriteLine("╠══════════════════════════════════════════╣");
            Console.WriteLine("║  1  - Gerenciar seleções                 ║");
            Console.WriteLine("║  2  - Gerenciar estádios                 ║");
            Console.WriteLine("║  3  - Gerenciar jogos                    ║");
            Console.WriteLine("║  4  - Registrar resultado de jogo        ║");
            Console.WriteLine("║  5  - Gerar tabela dos grupos            ║");
            Console.WriteLine("║  6  - Melhores terceiros colocados       ║");
            Console.WriteLine("║  7  - Gerar fase de 32 (mata-mata)       ║");
            Console.WriteLine("║  8  - Avançar fase do mata-mata          ║");
            Console.WriteLine("║  9  - Mostrar chave e campeão            ║");
            Console.WriteLine("║  10 - Gerar relatório final (CSV)        ║");
            Console.WriteLine("║  11 - Salvar todos os dados em CSV       ║");
            Console.WriteLine("║  12 - Carregar dados do CSV              ║");
            Console.WriteLine("║  0  - Sair                               ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.Write("Escolha uma opção: ");

            string entrada = Console.ReadLine()?.Trim() ?? "";

            switch (entrada)
            {
                case "1":  MenuSelecoes(); break;
                case "2":  MenuEstadios(); break;
                case "3":  MenuJogos(); break;
                case "4":  JogoCrud.RegistrarPlacar(); break;
                case "5":  Classificacao.MostrarTodosGrupos(); break;
                case "6":  Classificacao.MostrarMelhoresTerceiros(); break;
                case "7":  MataMata.GerarFaseDe32(); break;
                case "8":  MenuAvancarFase(); break;
                case "9":  MataMata.MostrarChave(); break;
                case "10": RelatorioMenu(); break;
                case "11": CsvManager.SalvarTudo(); UI.Pausar(); break;
                case "12":
                    Dados.InicializarVetores();
                    Dados.totalSelecoes = 0;
                    Dados.totalEstadios = 0;
                    Dados.totalJogos    = 0;
                    CsvManager.CarregarTudo();
                    UI.Sucesso("Dados carregados.");
                    UI.Pausar();
                    break;
                case "0": return false;
                default: UI.Erro("Opção inválida."); UI.Pausar(); break;
            }
            return true;
        }

        static void MenuSelecoes()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("\n====== SELEÇÕES ======");
                Console.WriteLine("1 - Cadastrar seleção");
                Console.WriteLine("2 - Listar seleções");
                Console.WriteLine("3 - Alterar seleção");
                Console.WriteLine("4 - Excluir seleção");
                Console.WriteLine("0 - Voltar");
                Console.Write("Opção: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": SelecaoCrud.Cadastrar(); break;
                    case "2": SelecaoCrud.Listar();    break;
                    case "3": SelecaoCrud.Alterar();   break;
                    case "4": SelecaoCrud.Excluir();   break;
                    case "0": loop = false;             break;
                    default:  UI.Erro("Inválido.");     break;
                }
            }
        }

        static void MenuEstadios()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("\n====== ESTÁDIOS ======");
                Console.WriteLine("1 - Cadastrar estádio");
                Console.WriteLine("2 - Listar estádios");
                Console.WriteLine("3 - Alterar estádio");
                Console.WriteLine("4 - Excluir estádio");
                Console.WriteLine("0 - Voltar");
                Console.Write("Opção: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": EstadioCrud.Cadastrar(); break;
                    case "2": EstadioCrud.Listar();    break;
                    case "3": EstadioCrud.Alterar();   break;
                    case "4": EstadioCrud.Excluir();   break;
                    case "0": loop = false;             break;
                    default:  UI.Erro("Inválido.");     break;
                }
            }
        }

        static void MenuJogos()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("\n====== JOGOS ======");
                Console.WriteLine("1 - Cadastrar jogo");
                Console.WriteLine("2 - Listar jogos");
                Console.WriteLine("3 - Alterar jogo");
                Console.WriteLine("4 - Excluir jogo");
                Console.WriteLine("5 - Registrar placar");
                Console.WriteLine("0 - Voltar");
                Console.Write("Opção: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": JogoCrud.Cadastrar();        break;
                    case "2": JogoCrud.Listar();           break;
                    case "3": JogoCrud.Alterar();          break;
                    case "4": JogoCrud.Excluir();          break;
                    case "5": JogoCrud.RegistrarPlacar();  break;
                    case "0": loop = false;                 break;
                    default:  UI.Erro("Inválido.");         break;
                }
            }
        }

        static void MenuAvancarFase()
        {
            Console.Clear();
            Console.WriteLine("\n====== AVANÇAR FASE DO MATA-MATA ======");
            Console.WriteLine("1 - 32avos   → Oitavas");
            Console.WriteLine("2 - Oitavas  → Quartas");
            Console.WriteLine("3 - Quartas  → Semifinal");
            Console.WriteLine("4 - Semifinal → Final + 3° lugar");
            Console.WriteLine("0 - Voltar");
            Console.Write("Opção: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": MataMata.AvancarFase("32avos",   "Oitavas");   break;
                case "2": MataMata.AvancarFase("Oitavas",  "Quartas");   break;
                case "3": MataMata.AvancarFase("Quartas",  "Semifinal"); break;
                case "4": MataMata.AvancarFase("Semifinal","Final");     break;
                case "0": break;
                default:  UI.Erro("Inválido."); UI.Pausar(); break;
            }
        }

        static void RelatorioMenu()
        {
            Console.Clear();
            Console.WriteLine("\n====== RELATÓRIOS ======");
            Console.WriteLine("1 - Relatório final em CSV (relatorio_final.csv)");
            Console.WriteLine("2 - Salvar classificação (classificacao.csv)");
            Console.WriteLine("3 - Salvar mata-mata (mata_mata.csv)");
            Console.WriteLine("0 - Voltar");
            Console.Write("Opção: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": CsvManager.GerarRelatorioFinal(); UI.Pausar(); break;
                case "2": CsvManager.SalvarClassificacao(); UI.Sucesso("classificacao.csv salvo."); UI.Pausar(); break;
                case "3": CsvManager.SalvarMataMata(); UI.Sucesso("mata_mata.csv salvo."); UI.Pausar(); break;
                case "0": break;
                default: UI.Erro("Inválido."); UI.Pausar(); break;
            }
        }
    }
}
