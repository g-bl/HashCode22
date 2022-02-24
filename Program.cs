using System;
using System.IO;

namespace HCode22
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=========== HCode22 BEGIN ===========");

            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "../../../outputs"));

            _ = new Solver("../../../inputs/a_an_example.in.txt",  "../../../outputs/a.out", ' ');
            _ = new Solver("../../../inputs/b_basic.in.txt",       "../../../outputs/b.out", ' ');
            _ = new Solver("../../../inputs/c_coarse.in.txt",      "../../../outputs/c.out", ' ');
            _ = new Solver("../../../inputs/d_difficult.in.txt",   "../../../outputs/d.out", ' ');
            _ = new Solver("../../../inputs/e_elaborate.in.txt",   "../../../outputs/e.out", ' ');

            File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "../../../Solver.cs"),
                      Path.Combine(Directory.GetCurrentDirectory(), "../../../outputs/Solver.cs"), true);

            Console.WriteLine("=========== HCode22 END =============");
        }
    }
}
