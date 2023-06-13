using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper_ClassLibrary
{
    public class ParticipantModel
    {
        public int Id { get; set; }
        public int YearOfParticipating { get; set; }
        public string Country { get; set; }
        public string Artist { get; set; }
        public string Song { get; set; }
        public bool QualifiedForFinal { get; set; }

    }
}
