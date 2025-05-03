using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using mw_cw8_proj.Exceptions;
using mw_cw8_proj.Models.DTOs;

namespace mw_cw8_proj.Services;

public class ClientsService : IClientsService
{
    
    private readonly string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Encrypt=False;";

    public async Task<List<TripPaymentDTO>> GetClientTripsAsync(int id)
    {
        var clientTrips = new List<TripPaymentDTO>();
        
        var query = @"SELECT T.IdTrip as TIdTrip, T.Name as TName,  T.Description as TDescription, T.DateFrom as TDateFrom, T.DateTo as TDateTo, T.MaxPeople as TMaxPeople, CT.PaymentDate as CTPaymentDate, CT.RegisteredAt as CTRegisteredAt FROM CLIENT C JOIN CLIENT_TRIP CT ON C.IdClient=CT.IdClient 
                        JOIN TRIP T ON T.IdTrip=CT.IdTrip WHERE C.IdClient=@IdClient";

        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            await sqlConnection.OpenAsync();
            
            
            using (var checkIdCmd = new SqlCommand("SELECT 1 FROM CLIENT WHERE IdClient = @IdClient", sqlConnection))
            {
                checkIdCmd.Parameters.AddWithValue("@IdClient", id);

                using (var reader = await checkIdCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        throw new ClientNotFoundException();
                    }
                }
            }


            using (var paymentTripsCmd = new SqlCommand(query, sqlConnection))
            {
                
                paymentTripsCmd.Parameters.AddWithValue("@IdClient", id);

                using (var reader = await paymentTripsCmd.ExecuteReaderAsync())
                {
                    int idTripOrdinal = reader.GetOrdinal("TIdTrip");
                    int idNameOrdinal = reader.GetOrdinal("TName");
                    int descriptionOrdinal = reader.GetOrdinal("TDescription");
                    int dateFromOrdinal = reader.GetOrdinal("TDateFrom");
                    int dateToOrdinal = reader.GetOrdinal("TDateTo");
                    int maxPeopleOrdinal = reader.GetOrdinal("TMaxPeople");
                    int paymentDateOrdinal = reader.GetOrdinal("CTPaymentDate");
                    int registeredAtOrdinal = reader.GetOrdinal("CTRegisteredAt");
                    
                    
                    while (await reader.ReadAsync())
                    {

                       var trip = new TripPaymentDTO()
                        {
                            IdTrip = reader.GetInt32(idTripOrdinal),
                            Name = reader.GetString(idNameOrdinal),
                            Description = reader.GetString(descriptionOrdinal),
                            DateFrom = reader.GetDateTime(dateFromOrdinal),
                            DateTo = reader.GetDateTime(dateToOrdinal),
                            MaxPeople = reader.GetInt32(maxPeopleOrdinal),
                            PaymentDate = reader.IsDBNull(paymentDateOrdinal) ? null : reader.GetInt32(paymentDateOrdinal),
                            RegisteredAt = reader.GetInt32(registeredAtOrdinal)
                        };
                        clientTrips.Add(trip);
                    }
                }
                return clientTrips;
            }
        }
    }

    
    public async Task<int> CreateClientAsync(CreateClientDTO client)
    {
        var query = @"INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                  VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
                  SELECT SCOPE_IDENTITY();
                ";


        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            await sqlConnection.OpenAsync();

            using (var createCmd = new SqlCommand(query, sqlConnection))
            {
                createCmd.Parameters.AddWithValue("@FirstName", client.FirstName);
                createCmd.Parameters.AddWithValue("@LastName", client.LastName);
                createCmd.Parameters.AddWithValue("@Email", client.Email);
                createCmd.Parameters.AddWithValue("@Telephone", client.Telephone);
                createCmd.Parameters.AddWithValue("@Pesel", client.Pesel);
                
                var result = await createCmd.ExecuteScalarAsync();
                var clientId = Convert.ToInt32(result);
                return clientId;
            }
        }
    }

    public async Task RegisterClientForTripAsync(int id, int tripId)
    {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            await sqlConnection.OpenAsync();


            using (var checkIdCmd = new SqlCommand("SELECT 1 FROM CLIENT WHERE IdClient = @IdClient",sqlConnection))
            {
                checkIdCmd.Parameters.AddWithValue("@IdClient", id);

                using (var reader = await checkIdCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        throw new ClientNotFoundException();
                }
            }
            
            
            using (var checkTripIdCmd = new SqlCommand("select count(*),T.MaxPeople from Client_Trip CT join TRIP T on T.IdTrip=CT.IdTrip where T.IdTrip=@IdTrip group by T.MaxPeople", sqlConnection))
            {
                checkTripIdCmd.Parameters.AddWithValue("@IdTrip", tripId);

                using (var reader = await checkTripIdCmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int registeredPeople = reader.GetInt32(0);
                        int maxPeople = reader.GetInt32(1);

                        if (registeredPeople + 1 > maxPeople)
                        {
                            throw new MaxPeopleExceededException();
                        }
                    }
                    else
                    {
                       throw new TripNotFoundException();
                    }
                }
            }

            
            using (var registerClientCmd = new SqlCommand("INSERT INTO Client_Trip(IdClient,IdTrip,RegisteredAt,PaymentDate) Values (@IdClient, @IdTrip, @CurrentDate, null)", sqlConnection))
            {
                DateTime currentDate = DateTime.Now;
                int dateInt = int.Parse(currentDate.ToString("yyyyMMdd"));
                
                registerClientCmd.Parameters.AddWithValue("@IdClient", id);
                registerClientCmd.Parameters.AddWithValue("@IdTrip", tripId);
                registerClientCmd.Parameters.AddWithValue("@CurrentDate", dateInt);
                
                int res = await registerClientCmd.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task DeleteClientFromTripAsync(int id, int tripId)
    {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            await sqlConnection.OpenAsync();
            
            using (var checkCmd = new SqlCommand("SELECT 1 FROM Client_Trip where IdClient=@IdClient and IdTrip=@IdTrip;", sqlConnection))
            {
                checkCmd.Parameters.AddWithValue("@IdClient", id);
                checkCmd.Parameters.AddWithValue("@IdTrip", tripId);

                using (var reader = await checkCmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        throw new TripWithClientNotFoundException();
                    }
                }
            }

            using (var deleteCmd = new SqlCommand("DELETE CLIENT_TRIP WHERE IdClient=@IdClient AND IdTrip=@IdTrip", sqlConnection))
                {
                    deleteCmd.Parameters.AddWithValue("@IdClient", id);
                    deleteCmd.Parameters.AddWithValue("@IdTrip", tripId);
                    
                    int res = await deleteCmd.ExecuteNonQueryAsync();
                }
        }    
    }
    
    
}