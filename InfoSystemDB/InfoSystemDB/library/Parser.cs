using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace InfoSystemDB
{
    public class Parser
    {
        private ListBox listbox;
        private List<ParsedAddress> result = new List<ParsedAddress>();

        public Parser(ListBox lb)
        {
            listbox = lb;

            Run();
        }

        private void Run()
        {
            for (int i = 0; i <= 7; i++)
            {
                result.AddRange(Parsing(i));
            }

            result.RemoveAt(1);
            result.RemoveAt(1);
            
            for (int i = 0; i < 6; i++)
            {
                result.RemoveAt(result.Count - 1);
            }
            
            foreach (var i in result)
            {
                string str = $"{i.Office}\nАдреса: {i.Address}\n{i.Schedule}\n";

                listbox.Items.Add(str);
            }
        }

        private List<ParsedAddress> Parsing(int page)
        {
            List<ParsedAddress> result = new List<ParsedAddress>();

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler
                       {
                           AllowAutoRedirect = false,
                           AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip |
                                                    DecompressionMethods.None
                       })
                {
                    using (var client = new HttpClient())
                    {
                        string url = $"https://delengine.com/uk/kharkiv-23043/novaposhta?page={page}";

                        using (HttpResponseMessage response = client.GetAsync(url).Result)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var html = response.Content.ReadAsStringAsync().Result;

                                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                doc.LoadHtml(html);

                                var tiles = doc.DocumentNode.SelectNodes(
                                    ".//div[@id='settlement-table-container']//div[@id='departments-list']//div[@class='col']");


                                foreach (var tile in tiles)
                                {
                                    var tileInfo = tile.SelectSingleNode(".//div[@class='col-11']");


                                    string[] officeNum = tileInfo.SelectSingleNode(
                                        ".//div[@class='d-flex flex-column']" +
                                        "//div[@class='h4 mb-0']//a").InnerText.Split(' ');

                                    string office = $"Відділення №{officeNum[2]}";

                                    var address = tileInfo.SelectSingleNode(".//div[@class='d-flex flex-wrap']")
                                        .ChildNodes[3].InnerHtml.Split('\n')[1];

                                    while (address[0] == ' ')
                                    {
                                        address = address.Substring(1, address.Length - 1);
                                    }

                                    while (address.Contains("&quot;"))
                                    {
                                        address = address.Replace("&quot;", "\"");
                                    }

                                    var schedule = tileInfo.SelectSingleNode(".//div[@class='department-schedule']")
                                        .InnerText.Split('\n')[1];

                                    while (schedule[0] == ' ')
                                    {
                                        schedule = schedule.Substring(1, schedule.Length - 1);
                                    }


                                    result.Add(new ParsedAddress(office, address, schedule));
                                }

                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

            return null;
        }
    }
}