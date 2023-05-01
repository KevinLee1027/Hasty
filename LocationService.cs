using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Locations;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Locations;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class LocationService : ILocationService
    {
        IDataProvider _data = null;
        public LocationService(IDataProvider data)
        {
            _data = data;
        }
        public void Delete(int id)
        {
            string procName = "[dbo].[Locations_Delete_ById]";

            _data.ExecuteNonQuery(procName,
               inputParamMapper: delegate (SqlParameterCollection col)
               {
                   col.AddWithValue("@Id", id);

               },
               returnParameters: null
            );

        }
        public Location Get(int id)
        {
            string procName = "[dbo].[Locations_Select_ById]";

            Location location = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection parameterCollection)
            {            
                parameterCollection.AddWithValue("@id", id);
            }, delegate (IDataReader reader, short set) 
            {
                location = MapSingleLocation(reader);
            }
            );

            return location;
        }
        public void Update(LocationUpdateRequest model, int userId)
        {
            string procname = "[dbo].[Locations_Update]";
            _data.ExecuteNonQuery(procname,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);                   
                    col.AddWithValue("@Id", model.Id);
                    col.AddWithValue("@ModifiedBy", userId);

                },
                returnParameters: null);
        }
        public int Add(LocationAddRequest model, int userId)
        {
            int id = 0;

            string procname = "[dbo].[Locations_Insert]";
            _data.ExecuteNonQuery(procname,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@CreatedBy", userId);                  

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;

                    int.TryParse(oId.ToString(), out id);

                });

            return id;
        }
        public Paged<Location> SelectAllPaginated(int pageIndex, int pageSize)
        {
            Paged<Location> pagedList = null;
            List<Location> list = null;
            int totalCount = 0;
            string procname = "[dbo].[Locations_SelectAll]";

            _data.ExecuteCmd(procname,
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);             
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    Location location = MapSingleLocation(reader);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<Location>();
                    }
                    list.Add(location);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<Location>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public Paged<Location> SelectByCreatedByPaginated(int pageIndex, int pageSize, int createdById)
        {
            Paged<Location> pagedList = null;
            List<Location> list = null;
            int totalCount = 0;
            string procname = "[dbo].[Locations_SelectByCreatedBy]";

            _data.ExecuteCmd(procname,
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@CreatedById", createdById);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    Location location = MapSingleLocation(reader);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<Location>();
                    }
                    list.Add(location);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<Location>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        private static Location MapSingleLocation(IDataReader reader)
        {
            Location aLocation = new Location();
            aLocation.LocationType = new LookUp();
            aLocation.State = new LookUp3Col();
            aLocation.CreatedBy = new User();
            aLocation.ModifiedBy = new User();
            int startingIndex = 0;

            aLocation.Id = reader.GetSafeInt32(startingIndex++);            
            aLocation.LocationType.Id = reader.GetSafeInt32(startingIndex++);
            aLocation.LocationType.Name = reader.GetSafeString(startingIndex++);
            aLocation.LineOne = reader.GetSafeString(startingIndex++);
            aLocation.LineTwo = reader.GetSafeString(startingIndex++);
            aLocation.City = reader.GetSafeString(startingIndex++);
            aLocation.Zip = reader.GetSafeString(startingIndex++);          
            aLocation.State.Id = reader.GetSafeInt32(startingIndex++);
            aLocation.State.Code = reader.GetSafeString(startingIndex++);
            aLocation.State.Name = reader.GetSafeString(startingIndex++);
            aLocation.Latitude = reader.GetSafeDouble(startingIndex++);
            aLocation.Longitude = reader.GetSafeDouble(startingIndex++);
            aLocation.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aLocation.DateModified = reader.GetSafeDateTime(startingIndex++);
            aLocation.CreatedBy.Id = reader.GetSafeInt32(startingIndex++);
            aLocation.CreatedBy.Email = reader.GetSafeString(startingIndex++);
            aLocation.CreatedBy.FirstName = reader.GetSafeString(startingIndex++);
            aLocation.CreatedBy.LastName = reader.GetSafeString(startingIndex++);
            aLocation.CreatedBy.Mi = reader.GetSafeString(startingIndex++);
            aLocation.CreatedBy.AvatarUrl = reader.GetSafeString(startingIndex++);          
            aLocation.CreatedBy.isConfirmed = reader.GetSafeBool(startingIndex++);
            aLocation.CreatedBy.StatusId = reader.GetSafeInt32(startingIndex++);
            aLocation.CreatedBy.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aLocation.CreatedBy.DateModified = reader.GetSafeDateTime(startingIndex++);
            aLocation.ModifiedBy.Id = reader.GetSafeInt32(startingIndex++);
            aLocation.ModifiedBy.Email = reader.GetSafeString(startingIndex++);
            aLocation.ModifiedBy.FirstName = reader.GetSafeString(startingIndex++);
            aLocation.ModifiedBy.LastName = reader.GetSafeString(startingIndex++);
            aLocation.ModifiedBy.Mi = reader.GetSafeString(startingIndex++);
            aLocation.ModifiedBy.AvatarUrl = reader.GetSafeString(startingIndex++);          
            aLocation.ModifiedBy.isConfirmed = reader.GetSafeBool(startingIndex++);
            aLocation.ModifiedBy.StatusId = reader.GetSafeInt32(startingIndex++);
            aLocation.ModifiedBy.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aLocation.ModifiedBy.DateModified = reader.GetSafeDateTime(startingIndex++);
            return aLocation;
        }
        private static void AddCommonParams(LocationAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@LocationTypeId", model.LocationTypeId);
            col.AddWithValue("@LineOne", model.LineOne);
            col.AddWithValue("@LineTwo", model.LineTwo);
            col.AddWithValue("@City", model.City);
            col.AddWithValue("@Zip", model.Zip);
            col.AddWithValue("@StateId", model.StateId);
            col.AddWithValue("@Latitude", model.Latitude);
            col.AddWithValue("@Longitude", model.Longitude);             
        }
    }
}
