using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Locations;
using Sabio.Models.Requests.Locations;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationApiController : BaseApiController
    {
        private ILocationService _service = null;
        private IAuthenticationService<int> _authService = null;
        public LocationApiController(ILocationService service
            , ILogger<LocationApiController> logger
            , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);
        }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Location>> GetById(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Location location = _service.Get(id);

                if (location == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Friend not found.");
                }
                else
                {
                    response = new ItemResponse<Location>() { Item = location };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);
        }
        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(LocationUpdateRequest model)
        {
            int iCode = 200;
            BaseResponse response = null;
            int userId = _authService.GetCurrentUserId();
            IUserAuthData user = _authService.GetCurrentUser();

            try
            {
                _service.Update(model, userId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);
        }
        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(LocationAddRequest model)
        {

            int userId = _authService.GetCurrentUserId();          

            ObjectResult result = null;

            try
            {
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Location>>> SelectAll(int pageIndex, int pageSize)
        {

            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Paged<Location> paged = _service.SelectAllPaginated(pageIndex, pageSize);

                if (paged == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Location not found.");
                }
                else
                {
                    iCode = 200;
                    response = new ItemResponse<Paged<Location>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message.ToString());
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(iCode, response);
        }
        [HttpGet("createdById/paginate")]
        public ActionResult<ItemResponse<Paged<Location>>> SelectByCreatedBy(int pageIndex, int pageSize, int createdById)
        {

            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Paged<Location> paged = _service.SelectByCreatedByPaginated(pageIndex, pageSize, createdById);

                if (paged == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Location not found.");
                }
                else
                {
                    iCode = 200;
                    response = new ItemResponse<Paged<Location>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message.ToString());
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(iCode, response);
        }
    }
}
