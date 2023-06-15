using HtmlAgilityPack;
using Microsoft.Data.SqlClient;
using System.Data;
using WebScraper_ClassLibrary;

namespace WebScraper_AllParticipants_2008to2022;

public class Program
{
    public static async Task Main(string[] args)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            Console.WriteLine("All participants (1) or final results (2) or update qualified (3)?");
            string choiceStr = Console.ReadLine();

            int choice = int.Parse(choiceStr);

            if (choice == 1)
            {
                for (int i = 2008; i <= 2023; i++)
                {
                    string url = $"https://en.wikipedia.org/wiki/Eurovision_Song_Contest_{i}";
                    HtmlDocument doc = GetDocument(url);

                    Console.WriteLine(i);

                    await WriteParticipatingCountriesToSqlDbAsync(doc, i);
                    Console.WriteLine('\n');
                }
            }

            else if (choice == 2)
            {
                for (int i = 2008; i <= 2023; i++)
                {
                    if (i != 2020)
                    {
                        string url = $"https://en.wikipedia.org/wiki/Eurovision_Song_Contest_{i}";
                        HtmlDocument doc = GetDocument(url);

                        Console.WriteLine(i);

                        await WriteFinalResultsToSqlDbAsync(doc, i);
                        Console.WriteLine('\n');
                    }
                }
            }
            else if (choice == 3)
            {
                await WriteQualifiedFieldToSqlDbAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static HtmlDocument GetDocument(string url)
    {
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(url);
        return doc;
    }

    public static async Task WriteParticipatingCountriesToSqlDbAsync(HtmlDocument doc, int year)
    {
        var participatingCountriesSection = doc.DocumentNode.SelectSingleNode("//span[contains(@id,'Semi-final_1')]");

        var firstSemi = participatingCountriesSection.SelectSingleNode("following::table[1]");
        var secondSemi = firstSemi.SelectSingleNode("following::table[1]");

        List<HtmlNode> tables = new()
        {
            firstSemi,
            secondSemi
        };

        if (firstSemi != null && secondSemi != null)
        {

            using (SqlConnection connection = new SqlConnection("MyConnectionString"))
            {

                foreach (var table in tables)
                {
                    var rows = table.Descendants("tr").Skip(1);

                    foreach (var row in rows)
                    {
                        var cells = row.Descendants("td").ToList();

                        ParticipantModel p = new ParticipantModel();

                        p.Country = cells[0].InnerText.Trim().Substring(6);
                        p.Artist = cells[1].InnerText.Trim();
                        p.Song = cells[2].InnerText.Trim();
                        p.YearOfParticipating = year;

                        SqlCommand cmd = new SqlCommand("dbo.spParticipant_Insert", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@yearOfParticipating", p.YearOfParticipating);
                        cmd.Parameters.AddWithValue("@country", p.Country);
                        cmd.Parameters.AddWithValue("@artist", p.Artist);
                        cmd.Parameters.AddWithValue("@song", p.Song);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                    }
                }
            }
        }
    }

    public static async Task WriteFinalResultsToSqlDbAsync(HtmlDocument doc, int year)
    {
        var finalsSection = doc.DocumentNode.SelectSingleNode("//span[contains(@id,'Final')]");
        var finalsTable = finalsSection.SelectSingleNode("following::table[1]");

        var rows = finalsTable.Descendants("tr").Skip(1);

        foreach (var row in rows)
        {
            var cells = row.Descendants("td").ToList();

            FinalParticipantModel p = new FinalParticipantModel();

            string country = cells[0].InnerText.Trim().Substring(6);

            string artist = cells[1].InnerText.Trim();
            string song = cells[2].InnerText.Trim();

            string place = cells[5].InnerText.Trim();

            if (cells[5].InnerText.Trim().IndexOf('&') != -1)
            {
                p.Place = int.Parse(cells[5].InnerText.Trim().Substring(0, cells[5].InnerText.Trim().IndexOf('&')));
            }
            else p.Place = int.Parse(cells[5].InnerText.Trim());
            p.Score = int.Parse(cells[4].InnerText.Trim());
            p.YearOfParticipating = year;

            using (SqlConnection connection = new SqlConnection("Server=WFJJ9PQ3\\MSSQLSERVER3;Database=eurovisiondb;Integrated Security=True;TrustServerCertificate=True"))
            {
                SqlCommand cmd = new SqlCommand("dbo.spFinalResults_Insert", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@country", country);
                cmd.Parameters.AddWithValue("@yearOfParticipating", p.YearOfParticipating);
                cmd.Parameters.AddWithValue("@artist", artist);
                cmd.Parameters.AddWithValue("@song", song);
                cmd.Parameters.AddWithValue("@score", p.Score);
                cmd.Parameters.AddWithValue("@place", p.Place);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public static async Task WriteQualifiedFieldToSqlDbAsync()
    {
        using (SqlConnection connection = new SqlConnection("Server=WFJJ9PQ3\\MSSQLSERVER3;Database=eurovisiondb;Integrated Security=True;TrustServerCertificate=True"))
        {
            SqlCommand cmd = new SqlCommand("dbo.spAllParticipants_UpdateQualifiedField", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}