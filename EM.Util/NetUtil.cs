using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;
using System.Windows.Forms;
using System.Threading;

namespace EM.Util
{
    public class NetUtil
    {
        public const int MAX_BASE = 36;
        public const string BASE36_ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyz";      //16 up to f

        /// <summary>
        /// - returns a function with same signature as the original but it will cache the results
        /// - when calling the returned function a second time  with the same set of parametters it 
        ///   will not compute the result again but instead will return it from a cache
        /// </summary>
        static Func<T, Res> memoize<T, Res>(Func<T, Res> func)
        {
            Dictionary<T, Res> cache = new Dictionary<T, Res>();
            return delegate(T arg)
            {
                 
                Res result = default(Res);

                if (cache.ContainsKey(arg))
                {
                    result = cache[arg];
                }
                else
                {
                    result = func(arg);
                    cache[arg] = result;
                }
                return result;
            };
        }

        
        /// <summary>
        /// The tuple (x-x%y)/y    and 
        ///            x%y
        /// Invariant: div*y + mod == x.
        /// </summary>
        public class DivMod
        {
            public int mod;
            public int div;

            public DivMod(int x, int y)
            {
                mod = x % y;
                div = (int)((x - mod) / y);
            }
        }
       
        
        /// <summary>
        /// see what a decimal (base 10) looks like in a base between 2 and 36
        /// </summary>
        public static string baseFromZec(int nr, int theBase)
        {
            if (theBase < 2 || theBase > MAX_BASE) throw new NotImplementedException("only bases from 2 to " + MAX_BASE.ToString());
            EList<string> res = new EList<string>();
            DivMod dm = new DivMod(nr, theBase);
            while (dm.div >= theBase)
            {
                res.Insert(0, BASE36_ALPHABET.Substring(dm.mod, 1));
                dm = new DivMod(dm.div, theBase);
            }
            res.Insert(0, BASE36_ALPHABET.Substring(dm.mod, 1));
            res.Insert(0, BASE36_ALPHABET.Substring(dm.div, 1));
            return res.join("");
        }

        /// <summary>
        /// see what a number in a base between 2 and 36 looks like in decimal (base 10)
        /// </summary>
        public static double zecFromBase(string nr, int theBase)
        {
            if (theBase < 2 || theBase > MAX_BASE) throw new NotImplementedException("only bases from 2 to " + MAX_BASE.ToString());
            nr = nr.ToLower();
            EList<int> bb = new EList<int>();
            for (int i = 0; i < nr.Length; i++ )
            {
                bb.Insert(0, BASE36_ALPHABET.IndexOf(nr[i]));                
            }
            
            double sum = 0;
            for (int i = 0; i < bb.Count; i++)
            {
                sum += Math.Pow(theBase, i) * bb[i];
            }
            
            return sum;
        }

    }

}
