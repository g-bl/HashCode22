using System;

namespace HCode22
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=========== HCode22 BEGIN ===========");

            _ = new Solver("inputs/a_an_example.in.txt",  "a.out", ' ');
            _ = new Solver("inputs/b_basic.in.txt",       "b.out", ' ');
            _ = new Solver("inputs/c_coarse.in.txt",      "c.out", ' ');
            _ = new Solver("inputs/d_difficult.in.txt",   "d.out", ' ');
            _ = new Solver("inputs/e_elaborate.in.txt",   "e.out", ' ');

            Console.WriteLine("=========== HCode22 END =============");
        }
    }
}
