using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Data.Entities.Chat;
using User.API.Data.Repository;
using User.API.Hubs;
using User.API.Models;

namespace User.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = ServiceCollectionExtensions.JWTAuthScheme)]
    public class UsersController : BaseController
    {
        private readonly UserManager<User.API.Data.Entities.Identity.User> _userManager;
        private readonly IRepository<Message> _repositoryMessage;
        private readonly IHubContext<ChatHub, IChatHub> _chatHubContext;

        public UsersController(UserManager<Data.Entities.Identity.User> userManager,
            IRepository<Message> repositoryMessage,
            IHubContext<ChatHub, IChatHub> chatHubContext)
        {
            _userManager = userManager;
            _repositoryMessage = repositoryMessage;
            _chatHubContext = chatHubContext;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            var model = _userManager.Users.Select(x => new UserModel
            {
                Id = x.Id,
                Name = x.Name,
                UserName = x.UserName,
                Email = x.Email
            }).ToList();

            return new ObjectResult(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] UserModel model)
        {
            var errors = new List<ReturnErrorModel>();
            if (ModelState.IsValid)
            {
                var user = new Data.Entities.Identity.User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    PhoneNumber = "1234567890",
                    UserName = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    NormalizedUserName = model.Email.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new Return
                    {
                        Success = true, 
                        Code = 200,
                        Data = user
                    });
                }

                errors.AddRange(result.Errors.Select(error => new ReturnErrorModel
                {
                    Key = error.Code,
                    Message = error.Description
                }));
            }


            errors.AddRange(ModelState.Where(x => x.Value.Errors.Count > 0)
                .Select(error => new ReturnErrorModel
                {
                    Key = error.Key,
                    Message = string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                }));

            return Ok(new ReturnError
            {
                Success = false,
                Code = StatusCodes.Status400BadRequest,
                Errors = errors,
                Message = "Bad Request"
            });
        }

        [HttpPost("SendMessage")]
        public virtual async Task<IActionResult> SendMessage([FromBody] MessageModel model)
        {
            Return @return = null;

            try
            {
                if (ModelState.IsValid)
                {
                    string fromUserId = base.GetCurrentUserId;
                    var entity = new Message
                    {
                        FromUserId = fromUserId,
                        ToUserId = model.ToUserId,
                        Content = model.Content,
                        CreatedOnUtc = DateTime.UtcNow,
                        IsOpened = false
                    };
                    _repositoryMessage.Insert(entity);

                    if (entity.Id > 0)
                    {
                        model.FromUserId = fromUserId;
                        model.MessageId = entity.Id;
                        model.Date = entity.CreatedOnUtc;
                        model.FormatedDate = entity.CreatedOnUtc.ToString("HH:mm:ss dddd, MMMM d, yyyy");

                        var toConnectionIds = ChatHub.Connections.GetConnections(model.ToUserId);
                        if (toConnectionIds.Any())
                        {
                            await _chatHubContext.Clients.Clients(toConnectionIds).UserMessageReceived(fromUserId, model);
                        }
                    }
                }

                @return = new Return
                {
                    Success = true,
                    Code = 200,
                    Data = model
                };
            }
            catch (Exception ex)
            {
                @return = new ReturnError
                {
                    Success = false,
                    Code = StatusCodes.Status500InternalServerError,
                    Message = ex.Message
                };
            }

            return Ok(@return);
        }

        [HttpGet("{userId}/Messages")]
        public virtual async Task<IActionResult> GetMessages(string userId)
        {
            Return @return = null;

            try
            {
                string currentUserId = base.GetCurrentUserId;
                var messages = _repositoryMessage.Table
                    .Where(x => (x.FromUserId == currentUserId && x.ToUserId == userId)
                        || (x.FromUserId == userId && x.ToUserId == currentUserId)
                    )
                    //.OrderByDescending(x => x.CreatedOnUtc)
                    .Select(x => new MessageModel
                    {
                        MessageId = x.Id,
                        FromUserId = x.FromUserId,
                        ToUserId = x.ToUserId,
                        Content = x.Content,
                        Date = x.CreatedOnUtc
                    })
                    .ToList();

                messages.ForEach(x => x.FormatedDate = x.Date.ToString("HH:mm:ss dddd, MMMM d, yyyy"));

                @return = new Return
                {
                    Success = true,
                    Code = 200,
                    Data = messages
                };
            }
            catch (Exception ex)
            {
                @return = new ReturnError
                {
                    Success = false,
                    Code = StatusCodes.Status500InternalServerError,
                    Message = ex.Message
                };
            }

            return Ok(@return);
        }

        [HttpGet("ActiveUsers")]
        public virtual IActionResult ActiveUsers()
        {
            var connections = ChatHub.Connections.GetAllConnections().Select(x => new
            {
                userId = x.Key,
                connectionIds = x.Value.ToList()
            }).ToList();

            return Ok(new Return
            {
                Success = true,
                Code = 200,
                Data = connections
            });
        }
    }
}
