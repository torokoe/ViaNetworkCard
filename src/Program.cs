using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIANetWorkCard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }


        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        Func<TFirst, TSecond, TThird, TResult> resultSelector)
        {
            using (var enum1 = first.GetEnumerator())
            using (var enum2 = second.GetEnumerator())
            using (var enum3 = third.GetEnumerator())
            {
                while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext())
                {
                    yield return resultSelector(
                        enum1.Current,
                        enum2.Current,
                        enum3.Current);
                }
            }
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        IEnumerable<TFourth> fourth,
        Func<TFirst, TSecond, TThird, TFourth, TResult> resultSelector)
        {
            using (var enum1 = first.GetEnumerator())
            using (var enum2 = second.GetEnumerator())
            using (var enum3 = third.GetEnumerator())
            using (var enum4 = fourth.GetEnumerator())
            {
                while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext() && enum4.MoveNext())
                {
                    yield return resultSelector(
                        enum1.Current,
                        enum2.Current,
                        enum3.Current,
                        enum4.Current);
                }
            }
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        IEnumerable<TFourth> fourth,
        IEnumerable<TFifth> fifth,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> resultSelector)
        {
            using (var enum1 = first.GetEnumerator())
            using (var enum2 = second.GetEnumerator())
            using (var enum3 = third.GetEnumerator())
            using (var enum4 = fourth.GetEnumerator())
            using (var enum5 = fifth.GetEnumerator())
            {
                while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext() && enum4.MoveNext() && enum5.MoveNext())
                {
                    yield return resultSelector(
                        enum1.Current,
                        enum2.Current,
                        enum3.Current,
                        enum4.Current,
                        enum5.Current);
                }
            }
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        IEnumerable<TFourth> fourth,
        IEnumerable<TFifth> fifth,
        IEnumerable<TSixth> sixth,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult> resultSelector)
        {
            using (var enum1 = first.GetEnumerator())
            using (var enum2 = second.GetEnumerator())
            using (var enum3 = third.GetEnumerator())
            using (var enum4 = fourth.GetEnumerator())
            using (var enum5 = fifth.GetEnumerator())
            using (var enum6 = sixth.GetEnumerator())
            {
                while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext() && enum4.MoveNext() && enum5.MoveNext() && enum6.MoveNext())
                {
                    yield return resultSelector(
                        enum1.Current,
                        enum2.Current,
                        enum3.Current,
                        enum4.Current,
                        enum5.Current,
                        enum6.Current);
                }
            }
        }


    }
}
