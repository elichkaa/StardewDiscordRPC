namespace Playground
{
    using System;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var output = new JsonReaderTest();
            var res = output.GetLocationObject("farms", "beach");
            foreach (var r in res)
            {
                Console.WriteLine(r.Key + " " + r.Value);
            }
        }
    }
}
