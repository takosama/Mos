using Rin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Intrinsics;
using System.Runtime.InteropServices;
using   System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;
using Rin.rMos;

namespace atcoder
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            abc242p.g();
        }
    }


    class abc242p
    {
        unsafe class MyMos : IMosFunctions<int, int>
        {
            MyArray<byte> array;
            byte* _ptr;
            public MyMos(int size)
            {
                array = new MyArray<byte>(size + 1);
                _ptr = array.ptr;
            }
            public int ComputeFoldL_L(Span<int> arr, int result)
            {
                for (int i = 0; i < arr.Length; i++)
                    result += (++_ptr[arr[i]] & 0x1) ^ 0x1;
                return result;
            }
            public int ComputeFoldL_R(Span<int> arr, int result)
            {
                for (int i = arr.Length - 1; i >= 0; i--)
                    result -= (++_ptr[arr[i]] & 0x1);
                return result;
            }
            public int ComputeFoldR_L(Span<int> arr, int result)
            {
                for (int i = arr.Length - 1; i >= 0; i--)
                    result -= (++_ptr[arr[i]] & 0x1);
                return result;
            }
            public int ComputeFoldR_R(Span<int> arr, int result)
            {
                for (int i = 0; i < arr.Length; i++)
                    result += (++_ptr[arr[i]] & 0x1) ^ 0x1;
                return result;
            }
        }

        unsafe public static void g()
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

            MyMos function = new MyMos(color.Length);

            var array = new MyArray<byte>(color.Length + 1);
            var _ptr = array.ptr;

            Mos<int, int> mos = new Mos<int, int>(color, querys, function);

            var result = mos.RunFold();
            Console.WriteLine(string.Join("\n", result));
        }
    }
}
