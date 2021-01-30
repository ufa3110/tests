using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestRSHB
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ввод массива целых чисел:");
            Console.WriteLine("Чтобы закончить ввод, введите некорректное число");

            Program p = new Program();

            var list = new List<int> { 1, 7, 9, 3, 4 };

            Console.WriteLine("Введите первое число");
            list = new List<int>();
            while (int.TryParse(Console.ReadLine(), out var input))
            {
                Console.WriteLine("Введите следующее число");
                list.Add(input);
            }

            Console.WriteLine("Введите число, которое необходимо искать среди сумм слагаемых");
            int.TryParse(Console.ReadLine(), out var value);

            Console.WriteLine("Введите число слагаемых");
            int.TryParse(Console.ReadLine(), out var termCount);

            if (value != 0 && termCount > 2 && list.Count() > 0)
            {
                p.unstatic(list, termCount, value);
            }

        }

        void unstatic(List<int> list, int termCount, int value)
        {
            var sums = Sums(list, termCount).ToList();

            var result = sums?.Where(_=>_.summ == value).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                while (result.Count(p => p.Equals(result[i])) > 1)
                {
                    result.RemoveAt(i);
                }
            }

            if (result != null)
            {
                Console.WriteLine($"Число {value} можно представить в виде суммы из {termCount} слагаемых как:");
                foreach (var item in result)
                {
                    StringBuilder strbldr = new StringBuilder();
                    foreach (var val in item.intCollection)
                    {
                        strbldr.Append($"{val} + ");
                    }
                    strbldr.Remove(strbldr.Length - 2, 2);
                    Console.WriteLine(strbldr.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputList">входящая коллекция</param>
        /// <param name="k">количество слагаемых</param>
        /// <returns></returns>
        List<ResultCollectionItem> Sums(List<int> inputList,  int k, List<int> removedValues = null)
        {
            if (removedValues == null)
            {
                removedValues = new List<int>();
            }
            var resultCollection = new List<ResultCollectionItem>();

            //вырезаем по одному слагаемому, считаем суммы
            for (int i = 0; i < inputList.Count(); i++)
            {
                int kk = k;
                if (kk > 1)
                {
                    var buffList = new List<int>(inputList);
                    var newRemovedValues = new List<int>();
                    newRemovedValues.Add(buffList[i]);
                    newRemovedValues.AddRange(removedValues);
                    buffList.RemoveAt(i);
                    kk--;
                    resultCollection.AddRange(Sums(buffList, kk, newRemovedValues).DistinctBy(_=>_.intCollection));
                }
                else
                {
                    foreach (var item in inputList)
                    {
                        var summItem = new ResultCollectionItem();
                        summItem.intCollection = new List<int>();

                        summItem.intCollection.AddRange(removedValues);
                        summItem.intCollection.Add(item);

                        summItem.summ += removedValues.Sum() + item;

                        resultCollection.Add(summItem);
                    }
                }
            }
            return resultCollection;
        }

        class ResultCollectionItem : IEquatable<ResultCollectionItem>
        {
            public int summ { get; set; }

            public List<int> intCollection { get; set; }

            public bool Equals(ResultCollectionItem other)
            {
                if (other == null)
                {
                    return false;
                }

                bool result = true;

                foreach (var item in intCollection)
                {
                    if (! other.intCollection.Any(_=>_.Equals(item)))
                    {
                        result = false;
                    }
                }

                return result;
            }
        }
        
    }

}
