using CommonLayer.Models;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ManagerLayer.Interface;
using RepositoryLayer.Context;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RepositoryLayer.Entity;
using System.Text;
using System.Threading.Tasks;
using FundooNotesApp.Helper;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelManager manager;
        private readonly FunDooDBContext context;
        private readonly ILogger<LabelController> logger;
        public readonly IDistributedCache distributedCache;

        public LabelController(ILabelManager manager, FunDooDBContext context, ILogger<LabelController> logger, IDistributedCache distributedCache)
        {
            this.manager = manager;
            this.context = context;
            this.logger = logger;
            this.distributedCache = distributedCache;
        }

        [Authorize]
        [HttpPost("AddLabel")]
        public IActionResult AddLabel( LabelModel model)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.AddLabel(Convert.ToInt32(data), model);
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<Label> { Success = true, Message = "New Label added", Data = response });
            }
            catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Label> { Success = false, Message = "Unable to find user/notes" });
            }
        }

        [Authorize]
        [HttpGet("GetLabel")]
        public IActionResult GetLabels()
        {
            try 
            { 
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.GetLabel(Convert.ToInt32(data));
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<List<Label>> { Success = true, Message = "Fetched all labels", Data = response });
            }
            catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<List<Label>> { Success = false, Message = "Unable to find user/notes" });
            }
        }

        [Authorize]
        [HttpPut("UpdateLabel")]
        public IActionResult UpdateLabel(LabelModel model)
        {
            try {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.UpdateLabel(Convert.ToInt32(data), model.LabelId, model);

                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<Label> { Success = true, Message = "label Updated", Data = response });
            
            }
            catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<Label> { Success = false, Message = "Unable to find user/notes" });
            }
        }

        [Authorize]
        [HttpDelete("DeleteLabel")]
        public IActionResult DeleteLabel(int LabelId)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.DeleteLabel(Convert.ToInt32(data), LabelId);

                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "label Deleted", Data = response });
                
            }
           catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Unable to find user/notes" });
            }
        }

        [HttpGet("UsingRedisCacheLabel")]

        public async Task<IActionResult> GetAllLabelsUsingRedisCache()
        {
            var cacheKey = "LabelsList";
            string SerializedLabelsLst;
            var LabelsList = new List<Label>();
            var RedisLabelsList = await distributedCache.GetAsync(cacheKey);
            if (RedisLabelsList != null)
            {
                SerializedLabelsLst = Encoding.UTF8.GetString(RedisLabelsList);
                LabelsList = JsonConvert.DeserializeObject<List<Label>>(SerializedLabelsLst);
            }

            else
            {
                LabelsList = context.Labels.ToList();
                SerializedLabelsLst = JsonConvert.SerializeObject(LabelsList);
                RedisLabelsList = Encoding.UTF8.GetBytes(SerializedLabelsLst);
                var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(25))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                await distributedCache.SetAsync(cacheKey, RedisLabelsList, options);
            }
            return Ok(LabelsList);

        }
    }
}
