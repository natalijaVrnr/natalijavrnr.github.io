using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp_DataAccess
{
    public class RequestBody
    {
        [Required]
        public string CallerUserId { get; set; }
    }

    public class GetParticipantsByCountryAndYearRequest : RequestBody
    {
        public string Country { get; set;}
        public string Year { get; set;}
    }

    public class GetParticipantByIdRequest : RequestBody 
    {
        [Required]
        public string Id { get; set; } 
    }
}
