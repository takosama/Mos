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



namespace Rin.rMos
{
    interface IMosFunctions<T, U>
    {
        public U ComputeFoldL_L(Span<T> arr, U result);
        public U ComputeFoldL_R(Span<T> arr, U result);
        public U ComputeFoldR_L(Span<T> arr, U result);
        public U ComputeFoldR_R(Span<T> arr, U result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">arr</typeparam>
    /// <typeparam name="U">result</typeparam>
    unsafe class Mos<T, U>
        where T : unmanaged
        where U : unmanaged
    {
        T[] _arr;
        int _r;
        int _l;
        U result;
        bool IsFirst = true;
        List<(int id, int l, int r)>[] _mosquerys;
        int _QuerysLength = 0;
        IMosFunctions<T, U> Function;
        public Mos(T[] arr, (int id, int l, int r)[] querys, IMosFunctions<T, U> functions)
        {
            this._arr = arr;

            querys = querys.OrderBy(x => x.l).ToArray();
            _QuerysLength = querys.Length;

            int size = 1 + (int)Math.Sqrt(querys.Length);

            _mosquerys = new List<(int id, int l, int r)>[size + 1];
            for (int i = 0; i < _mosquerys.Length; i++)
                _mosquerys[i] = new List<(int id, int l, int r)>();
            for (int i = 0; i < querys.Length; i++)
                _mosquerys[Math.Min(size, querys[i].l / size)].Add(querys[i]);

            for (int i = 0; i < _mosquerys.Length; i++)
                if (i % 2 == 0)
                    _mosquerys[i] = _mosquerys[i].OrderBy(x => x.r).ToList();
                else
                    _mosquerys[i] = _mosquerys[i].OrderByDescending(x => x.r).ToList();

            Function = functions;
        }

        public U[] RunFold()
        {
            var rtn = new U[_QuerysLength];
            var span = this._arr.AsSpan();
            foreach (var qs in _mosquerys)
                foreach (var q in qs)
                {
                    SetLRFold(q.l, q.r, span);
                    rtn[q.id] = this.result;
                }
            return rtn;
        }

        void SetLRFold(int l, int r, Span<T> arr)
        {
            if (IsFirst)
            {
                _l = l;
                _r = l - 1;
                SetRBitFold(r, arr);
                _r = r;
                IsFirst = false;
            }
            else
            {
                SetRBitFold(r, arr);
                SetLBitFold(l, arr);

                _l = l;
                _r = r;
            }
        }


        void SetLBitFold(int l, Span<T> arr)
        {
            if (l > _l)
                result = Function.ComputeFoldL_R(arr.Slice(_l, l - _l), result);
            else
                result = Function.ComputeFoldL_L(arr.Slice(l, _l - l), result);
        }
        void SetRBitFold(int r, Span<T> arr)
        {
            if (r > _r)
                result = Function.ComputeFoldR_R(arr.Slice(_r + 1, r - _r), result);
            else
                result = Function.ComputeFoldR_L(arr.Slice(r + 1, _r - r), result);
        }

    }
}
