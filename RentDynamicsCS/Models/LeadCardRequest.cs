using System;
using Newtonsoft.Json;
using RentDynamicsCS.Converters;
using RentDynamicsCS.Models.Enums;

namespace RentDynamicsCS.Models
{
    public class LeadCardRequest
    {
        public int AdSourceId { get; }
        public DateTime? AppointmentDate { get; }
        public double? Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public CommunicationType? CommunicationTypeId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime MoveDate { get; }

        public string PhoneNumber { get; }
        public string Note { get; }

        public LeadCardRequest(
            string email,
            string firstName,
            string lastName,
            int adSourceId,
            string phoneNumber,
            DateTime moveDate,
            int bedrooms,
            string note,
            double? bathrooms = null,
            DateTime? appointmentDate = null,
            CommunicationType? communicationTypeId = null)
        {
            AdSourceId = adSourceId;
            AppointmentDate = appointmentDate;
            Bathrooms = bathrooms;
            Bedrooms = bedrooms;
            CommunicationTypeId = communicationTypeId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            MoveDate = moveDate;
            PhoneNumber = phoneNumber;
            Note = note;
        }
    }
}