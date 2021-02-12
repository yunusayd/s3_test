using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string accessKey = ConfigurationManager.AppSettings["accessKey"];
            string secretKey = ConfigurationManager.AppSettings["secretKey"];
            string endpoint = ConfigurationManager.AppSettings["endpoint"];

            S3Client client = new S3Client(accessKey, secretKey, endpoint);

            string jsonObj = @"{
                                ""glossary"": {
                                    ""title"": ""example glossary"",
		                            ""GlossDiv"": {
                                        ""title"": ""S"",
			                            ""GlossList"": {
                                            ""GlossEntry"": {
                                                ""ID"": ""SGML"",
					                            ""SortAs"": ""SGML"",
					                            ""GlossTerm"": ""Standard Generalized Markup Language"",
					                            ""Acronym"": ""SGML"",
					                            ""Abbrev"": ""ISO 8879:1986"",
					                            ""GlossDef"": {
                                                    ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
						                            ""GlossSeeAlso"": [""GML"", ""XML""]
                                                },
					                            ""GlossSee"": ""markup""
                                            }
                                        }
                                    }
                                }
                            }";
            Console.WriteLine("===================================================================================");
            Console.WriteLine("Listing buckets");
            var list = client.ListBuckets();
            foreach (var bucket in list)
            {
                Console.WriteLine(bucket.BucketName);
            }
            Console.WriteLine("===================================================================================");
            Console.Write("Test Count: ");
            var scount = Console.ReadLine();
            Stopwatch sw = new Stopwatch();
            double totalPutMs = 0;
            double totalReadMs = 0;
            if (Int32.TryParse(scount, out int count))
            {
                do
                {
                    var guid = Guid.NewGuid();
                    sw.Reset();
                    sw.Start();
                    client.Put("Yunus-Test", guid.ToString(), jsonObj, null);
                    sw.Stop();
                    totalPutMs += sw.Elapsed.TotalMilliseconds;
                    //Console.WriteLine("Obj Put:" + guid.ToString() + " in " + sw.Elapsed.TotalMilliseconds.ToString("###,###.#####") + " ms");
                    sw.Reset();
                    sw.Start();
                    string readObj = client.Get("Yunus-Test", guid.ToString());
                    sw.Stop();
                    totalReadMs += sw.Elapsed.TotalMilliseconds;
                    Console.WriteLine("Obj Read: " + guid.ToString() + " in " + sw.Elapsed.TotalMilliseconds.ToString("###,###.#####") + " ms");
                    count--;
                } while (count > 0);
            }
            Console.WriteLine("Total Put cost:" + totalPutMs.ToString("###,###.####") + " ms");
            Console.WriteLine("Total Get cost:" + totalReadMs.ToString("###,###.####") + " ms");
            Console.ReadKey();
        }
    }
}

