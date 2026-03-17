using TestAvankor.Domain.Models;
using TestAvankor.Domain.Services;
using TestAvankor.Infrastructure.XbrlParser;

namespace TestAvankor.App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var parser = new XbrlParser();
            var analyzer = new InstanceAnalysisService();

            // Путь до папки Data относительно исполняемого файла
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data");
            dataPath = Path.GetFullPath(dataPath);

            var report1Path = Path.Combine(dataPath, "report1.xbrl");
            var report2Path = Path.Combine(dataPath, "report2.xbrl");

            if (!File.Exists(report1Path) || !File.Exists(report2Path))
            {
                Console.WriteLine("Не найдены файлы report1.xbrl и/или report2.xbrl в папке Data.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Парсинг отчетов...");
            var instance1 = await parser.ParseAsync(report1Path);
            var instance2 = await parser.ParseAsync(report2Path);

            Console.WriteLine();
            Console.WriteLine("Проверка повторяющихся контекстов в каждом файле:");
            PrintDuplicateContexts(analyzer, instance1, "report1.xbrl");
            PrintDuplicateContexts(analyzer, instance2, "report2.xbrl");

            Console.WriteLine();
            Console.WriteLine("Сравнение фактов между отчетами:");
            PrintFactDifferences(analyzer, instance1, instance2, "report1.xbrl", "report2.xbrl");

            Console.WriteLine();
            Console.WriteLine("Объединение отчетов...");
            var mergedPath = Path.Combine(dataPath, "merged-report.xbrl");
            XbrlMerger.MergeFiles(report1Path, report2Path, mergedPath, Console.Out);
            Console.WriteLine($"Объединенный отчет сохранен в файле: {mergedPath}");

            Console.WriteLine();
            Console.WriteLine("Готово. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Поиск повторяющихся контекстов (по Id) и вывод в консоль.
        /// </summary>
        private static void PrintDuplicateContexts(InstanceAnalysisService analyzer, Instance instance, string reportName)
        {
            var duplicates = analyzer.FindDuplicateContexts(instance);

            if (duplicates.Count == 0)
            {
                Console.WriteLine($"  {reportName}: повторяющиеся контексты не найдены.");
                return;
            }

            Console.WriteLine($"  {reportName}: найдены повторяющиеся контексты:");
            foreach (var group in duplicates)
            {
                Console.WriteLine($"    Id={group.Id}, количество={group.Count}");
            }
        }

        /// <summary>
        /// Сравнение фактов между двумя отчетами и вывод результата в консоль.
        /// </summary>
        private static void PrintFactDifferences(InstanceAnalysisService analyzer, Instance first, Instance second, string firstName, string secondName)
        {
            var comparison = analyzer.CompareFacts(first, second);

            Console.WriteLine($"  Факты, присутствующие в {firstName}, но отсутствующие в {secondName}: {comparison.MissingInSecond.Count}");
            foreach (var id in comparison.MissingInSecond.Take(20))
                Console.WriteLine($"    {id}");
            if (comparison.MissingInSecond.Count > 20)
                Console.WriteLine("    ...");

            Console.WriteLine($"  Факты, новые в {secondName} относительно {firstName}: {comparison.NewInSecond.Count}");
            foreach (var id in comparison.NewInSecond.Take(20))
                Console.WriteLine($"    {id}");
            if (comparison.NewInSecond.Count > 20)
                Console.WriteLine("    ...");

            Console.WriteLine($"  Факты с различающимися значениями: {comparison.ValueDifferences.Count}");
            foreach (var diff in comparison.ValueDifferences.Take(20))
            {
                Console.WriteLine(
                    $"    {diff.Id}: {firstName} Value={diff.First.Value}, {secondName} Value={diff.Second.Value}, " +
                    $"ContextRef: {diff.First.ContextRef} / {diff.Second.ContextRef}, UnitRef: {diff.First.UnitRef} / {diff.Second.UnitRef}");
            }
            if (comparison.ValueDifferences.Count > 20)
                Console.WriteLine("    ...");
        }
    }
}
