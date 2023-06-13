using DataAccessProject.Data.ParticipantData;
using DataAccessProject.Models;
using DataAccessProject.Globals;
using HtmlAgilityPack;
using Microsoft.Data.SqlClient;

namespace DataAccess.TypesOfDataAccess
{
    public class DatabaseProcessor
    {
        private IParticipantData _participantData;
        public DatabaseProcessor(IParticipantData participantData) 
        {
            _participantData = participantData;
        }

        public static async Task InsertParticipants (HtmlDocument doc, int year, IParticipantData _participantData)
        {
            var participatingCountriesSection = doc.DocumentNode.SelectSingleNode("//span[contains(@id,'Semi-final_1')]");

            // Find the next heading element after the Participating countries section
            var firstSemi = participatingCountriesSection.SelectSingleNode("following::table[1]");
            var secondSemi = firstSemi.SelectSingleNode("following::table[1]");

            // Use HtmlAgilityPack to select all the semi-final tables between the Participating countries section and the next heading
            List<HtmlNode> tables = new()
        {
            firstSemi,
            secondSemi
        };


            // Check if tables is null before trying to loop through it
            if (firstSemi != null && secondSemi != null)
            {

                using (SqlConnection connection = new SqlConnection(GlobalVariables.ConnectionStringSql))
                {

                    // Loop through the tables
                    foreach (var table in tables)
                    {

                        // Select all rows in the table except for the header row
                        var rows = table.Descendants("tr").Skip(1);

                        // Loop through the rows and extract the participating countries and their songs
                        foreach (var row in rows)
                        {
                            var cells = row.Descendants("td").ToList();

                            ParticipantModel p = new ParticipantModel();

                            p.Country = cells[0].InnerText.Trim().Substring(6);
                            p.Artist = cells[1].InnerText.Trim();
                            p.Song = cells[2].InnerText.Trim();
                            p.YearOfParticipating = year;


                            await _participantData.InsertParticipant(p);

                            //Console.WriteLine("{0} - {1} - {2}", country, singer, song);
                        }
                    }
                }
            }
        }
    }
}
