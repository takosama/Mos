using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rin;
namespace atcoder
{

    internal class Program
    {
        public static void Main(string[] args)
        {
            g();
        }

        public static void g()
        {
            var _ = int.Parse(Console.ReadLine());

            var color = Console.ReadLine().Split(" ").Select(x => int.Parse(x)).ToArray();
            var num = int.Parse(Console.ReadLine());

            (int id, int l, int r)[] querys = new (int id, int l, int r)[num];
            for (int i = 0; i < num; i++)
            {
                var tmp = Console.ReadLine().Split(" ").Select(x => int.Parse(x)).ToArray();
                querys[i].l = tmp[0] - 1;
                querys[i].r = tmp[1] - 1;
                querys[i].id = i;
            }

            int[] CountArray = new int[color.Length + 1];
            Mos<int, int> mos = new Mos<int, int>(color, querys);
            Func<int, int, int> fp = (input, result) =>
            {
                CountArray[input]++;
                if (Math.Abs(CountArray[input] % 2) == 0)
                    return result + 1;
                else
                    return result;
            };
            Func<int, int, int> fm = (input, result) =>
            {
                CountArray[input]--;
                if (Math.Abs(CountArray[input] % 2) == 1)
                    return result - 1;
                else
                    return result;
            };

            var result = mos.Run(fm, fp, fp, fm);
            Console.WriteLine(string.Join("\n", result));
        }

    }

}
   
