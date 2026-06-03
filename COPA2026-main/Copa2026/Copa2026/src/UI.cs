using System;

namespace Copa2026
{
    static class UI
    {
        public static void Cabecalho(string titulo)
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"  {titulo}");
            Console.WriteLine(new string('=', 50));
        }

        public static void Sucesso(string msg)  => Console.WriteLine($"\n[OK] {msg}");
        public static void Erro(string msg)     => Console.WriteLine($"\n[ERRO] {msg}");
        public static void Info(string msg)     => Console.WriteLine($"  {msg}");

        public static void Pausar()
        {
            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }

        public static string LerString(string prompt, bool obrigatorio = true)
        {
            while (true)
            {
                Console.Write(prompt);
                string valor = Console.ReadLine()?.Trim() ?? "";
                if (!obrigatorio || valor.Length > 0) return valor;
                Erro("Campo obrigatório. Tente novamente.");
            }
        }

        public static int LerInt(string prompt, int min = 0, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max)
                    return v;
                Erro($"Informe um número entre {min} e {max}.");
            }
        }

        public static string LerData(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string s = Console.ReadLine()?.Trim() ?? "";
                if (DateTime.TryParseExact(s, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out _))
                    return s;
                Erro("Data inválida. Use o formato dd/MM/yyyy.");
            }
        }
    }
}
