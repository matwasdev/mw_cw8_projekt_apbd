using Microsoft.Data.SqlClient;
using mw_cw8_proj.Models.DTOs;

namespace mw_cw8_proj.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Encrypt=False;";

    public async Task<List<TripDTO>> GetTripsAsync()
    {
        var trips = new List<TripDTO>();

        var cmdText = @"select T.IdTrip as TIdTrip, T.Name as TName,Description,DateFrom,DateTo,MaxPeople,C.IdCountry as CIdCountry, C.Name  as CName from Trip T join Country_Trip CT on CT.IdTrip=T.IdTrip
                        Join Country C on C.IdCountry = CT.IdCountry;";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(cmdText, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                int idTripOrdinal = reader.GetOrdinal("TIdTrip");
                int idNameOrdinal = reader.GetOrdinal("TName");
                int descriptionOrdinal = reader.GetOrdinal("Description");
                int dateFromOrdinal = reader.GetOrdinal("DateFrom");
                int dateToOrdinal = reader.GetOrdinal("DateTo");
                int maxPeopleOrdinal = reader.GetOrdinal("MaxPeople");
                int idCountryOrdinal = reader.GetOrdinal("CIdCountry");
                int cNameOrdinal = reader.GetOrdinal("CName");
                
                
                var tripDict = new Dictionary<int, TripDTO>();
                
                while (await reader.ReadAsync())
                {
                    var trip = new TripDTO();
                    int idTrip = reader.GetInt32(idTripOrdinal);
                    
                    if (!tripDict.ContainsKey(idTrip))
                    {
                        trip = new TripDTO()
                        {
                            IdTrip = idTrip,
                            Name = reader.GetString(idNameOrdinal),
                            Description = reader.GetString(descriptionOrdinal),
                            DateFrom = reader.GetDateTime(dateFromOrdinal),
                            DateTo = reader.GetDateTime(dateToOrdinal),
                            MaxPeople = reader.GetInt32(maxPeopleOrdinal),
                            Countries = new List<CountryDTO>()
                        };
                        tripDict.Add(idTrip, trip);
                        trips.Add(trip);
                    }

                    if (tripDict.ContainsKey(idTrip))
                    {
                        tripDict[idTrip].Countries.Add(new CountryDTO(){IdCountry = reader.GetInt32(idCountryOrdinal), Name = reader.GetString(cNameOrdinal)});
                    }
                }
            }

        }
        return trips;
    }
}