using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rin
{
    public static class ObjectExtension
    {
        public static T DeepClone<T>(this T src)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var binaryFormatter
                  = new System.Runtime.Serialization
                        .Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, src); // シリアライズ
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
            }
        }
    }

    class Mos<T, U>
    {
        T[] _arr;
        int _r;
        int _l;
        U result;
        bool IsFirst = true;
        List<(int id, int l, int r)>[] _mosquerys;
        int _QuerysLength = 0;
        Func<T, U, U> _L_MoveL;
        Func<T, U, U> _L_MoveR;
        Func<T, U, U> _R_MoveL;
        Func<T, U, U> _R_MoveR;
        public Mos(T[] arr, (int id, int l, int r)[] querys)
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
        }

        /// <summary>
        /// in (T input,U result) 
        /// out U
        /// </summary>
        /// <param name="L_MoveL"></param>
        /// <param name="L_MoveR"></param>
        /// <param name="R_MoveL"></param>
        /// <param name="R_MoveR"></param>
        /// <returns></returns>
        public U[] Run(Func<T, U, U> L_MoveL, Func<T, U, U> L_MoveR, Func<T, U, U> R_MoveL, Func<T, U, U> R_MoveR)
        {
            _L_MoveL = L_MoveL;
            _L_MoveR = L_MoveR;
            _R_MoveL = R_MoveL;
            _R_MoveR = R_MoveR;
            var rtn = new U[_QuerysLength];
            foreach (var qs in _mosquerys)
                foreach (var q in qs)
                {
                    SetLR(q.l, q.r);
                    rtn[q.id] = this.result;
                }
            return rtn;
        }

        /// <summary>
        /// in T input,U result 
        /// out U
        /// </summary>
        /// <param name="L_MoveL"></param>
        /// <param name="L_MoveR"></param>
        /// <param name="R_MoveL"></param>
        /// <param name="R_MoveR"></param>
        /// <returns></returns>
        public U[] Run_ResultDeepClone(Func<T, U, U> L_MoveL, Func<T, U, U> L_MoveR, Func<T, U, U> R_MoveL, Func<T, U, U> R_MoveR)
        {
            _L_MoveL = L_MoveL;
            _L_MoveR = L_MoveR;
            _R_MoveL = R_MoveL;
            _R_MoveR = R_MoveR;
            var rtn = new U[_QuerysLength];
            foreach (var qs in _mosquerys)
                foreach (var q in qs)
                {
                    SetLR(q.l, q.r);
                    rtn[q.id] = this.result.DeepClone();
                }
            return rtn;
        }

        void SetLR(int l, int r)
        {
            if (IsFirst)
            {
                _l = l;
                _r = l - 1;
                SetRBit(r);
                _r = r;
                IsFirst = false;
            }
            else
            {
                SetRBit(r);
                SetLBit(l);

                _l = l;
                _r = r;
            }
        }

        void SetLBit(int l)
        {
            if (l > _l)
                for (int i = _l; i < l; i++)
                    result = _L_MoveL(_arr[i], result);
            else if (l < _l)
                for (int i = _l - 1; i >= l; i--)
                    result = _L_MoveR(_arr[i], result);
        }
        void SetRBit(int r)
        {
            if (r > _r)
                for (int i = _r + 1; i <= r; i++)
                    result = _R_MoveL(_arr[i], result);
            else if (r < _r)
                for (int i = _r; i > r; i--)
                    result = _R_MoveR(_arr[i], result);
        }
    }
}
