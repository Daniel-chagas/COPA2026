using System;

namespace Copa2026
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding  = System.Text.Encoding.UTF8;

            Dados.InicializarVetores();
            CsvManager.CarregarTudo();

            bool executando = true;
            while (executando)
            {
                executando = Menus.MenuPrincipal();
            }
        }
    }
}
